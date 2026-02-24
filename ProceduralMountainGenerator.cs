using UnityEngine;

/// <summary>
/// Generates procedural mountain terrain using fractal Voronoi patterns combined with
/// hydraulic and thermal erosion simulations.
///
/// Usage example:
/// <code>
///   // Attach to a GameObject that also has a Terrain component.
///   // Tweak parameters in the Inspector, then call GenerateTerrain() from a button or Start().
///
///   ProceduralMountainGenerator gen = GetComponent&lt;ProceduralMountainGenerator&gt;();
///   gen.seed = 42;
///   gen.GenerateTerrain();
///
///   // Export the raw heightmap for external use:
///   float[,] heights = gen.ExportHeightmap();
/// </code>
/// </summary>
public class ProceduralMountainGenerator : MonoBehaviour
{
    // ─── Terrain target ──────────────────────────────────────────────────────

    /// <summary>Unity Terrain to write the heightmap into. If null, no Terrain is modified.</summary>
    [Header("Target")]
    [SerializeField] private Terrain targetTerrain;

    // ─── Resolution ──────────────────────────────────────────────────────────

    /// <summary>
    /// Heightmap resolution (width and height in samples).
    /// Must be a power of two plus one for Unity terrains (e.g. 129, 257, 513).
    /// </summary>
    [Header("Resolution")]
    [SerializeField] private int resolution = 257;

    // ─── Seed ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Seed value for the random-number generator.
    /// Changing this produces a different but fully reproducible layout.
    /// </summary>
    [Header("Seed")]
    public int seed = 0;

    // ─── Fractal Voronoi ─────────────────────────────────────────────────────

    /// <summary>Controls the overall zoom level of the Voronoi base pattern.</summary>
    [Header("Fractal Voronoi")]
    [SerializeField] private float voronoiScale = 0.015f;

    /// <summary>Number of fractal octaves layered on top of each other.</summary>
    [SerializeField, Range(1, 8)] private int octaves = 5;

    /// <summary>
    /// Amplitude multiplier applied to each successive octave.
    /// Values in (0, 1) make higher octaves contribute less detail.
    /// </summary>
    [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;

    /// <summary>
    /// Frequency multiplier applied to each successive octave.
    /// Values greater than 1 add finer detail at each octave.
    /// </summary>
    [SerializeField] private float lacunarity = 2.0f;

    // ─── Hydraulic erosion ───────────────────────────────────────────────────

    /// <summary>Number of hydraulic erosion simulation passes.</summary>
    [Header("Hydraulic Erosion")]
    [SerializeField, Range(0, 500)] private int hydraulicIterations = 100;

    /// <summary>Amount of rain deposited per cell per iteration.</summary>
    [SerializeField] private float rainAmount = 0.01f;

    /// <summary>
    /// Maximum sediment a unit of water can carry relative to the slope.
    /// Higher values create more pronounced erosion channels.
    /// </summary>
    [SerializeField] private float sedimentCapacity = 0.1f;

    /// <summary>Fraction of entrained sediment that is deposited each step.</summary>
    [SerializeField, Range(0f, 1f)] private float depositionRate = 0.3f;

    /// <summary>Fraction of surface water that evaporates each step.</summary>
    [SerializeField, Range(0f, 1f)] private float evaporationRate = 0.05f;

    /// <summary>Fraction of cell water (and its sediment) moved to the lowest neighbour each step.</summary>
    [SerializeField, Range(0f, 1f)] private float flowFraction = 0.5f;

    // ─── Thermal erosion ─────────────────────────────────────────────────────

    /// <summary>Number of thermal erosion simulation passes.</summary>
    [Header("Thermal Erosion")]
    [SerializeField, Range(0, 500)] private int thermalIterations = 80;

    /// <summary>
    /// Maximum stable slope (in height units per cell).
    /// Material on slopes exceeding this angle flows downhill as talus.
    /// </summary>
    [SerializeField] private float talusAngle = 0.05f;

    /// <summary>Fraction of excess material moved downhill each iteration.</summary>
    [SerializeField, Range(0f, 1f)] private float thermalAmount = 0.5f;

    // ─── Post-processing ─────────────────────────────────────────────────────

    /// <summary>When true, the heightmap is normalised to the [0, 1] range after generation.</summary>
    [Header("Post-processing")]
    [SerializeField] private bool normalizeHeight = true;

    /// <summary>Number of 3×3 box smoothing passes applied after erosion.</summary>
    [SerializeField, Range(0, 5)] private int smoothingPasses = 1;

    private float[,] heightmap;
    private System.Random rng;

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Generates a full terrain heightmap and optionally applies it to <see cref="targetTerrain"/>.
    /// Call this whenever you want to (re-)generate the terrain, e.g. from a button or Start().
    /// </summary>
    public void GenerateTerrain()
    {
        rng = new System.Random(seed);

        heightmap = GenerateVoronoiFractal(resolution, resolution);

        if (hydraulicIterations > 0)
            ApplyHydraulicErosion();

        if (thermalIterations > 0)
            ApplyThermalErosion();

        if (normalizeHeight)
            NormalizeHeightmap();

        for (int i = 0; i < smoothingPasses; i++)
            SmoothHeightmap();

        if (targetTerrain != null)
            ApplyToTerrain();
    }

    /// <summary>
    /// Returns a copy of the generated heightmap as a 2-D array of floats in [0, 1].
    /// Returns null if <see cref="GenerateTerrain"/> has not been called yet.
    /// </summary>
    public float[,] ExportHeightmap()
    {
        if (heightmap == null)
            return null;

        int w = heightmap.GetLength(0);
        int h = heightmap.GetLength(1);
        float[,] copy = new float[w, h];
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                copy[x, y] = heightmap[x, y];
        return copy;
    }

    // ─── Terrain integration ──────────────────────────────────────────────────

    /// <summary>Writes the internal heightmap to the Unity Terrain component.</summary>
    private void ApplyToTerrain()
    {
        TerrainData data = targetTerrain.terrainData;

        // Unity expects heights in [0, 1] and the array indexed [row, col]
        int res = Mathf.Min(resolution, data.heightmapResolution);
        float[,] unityHeights = new float[res, res];
        for (int z = 0; z < res; z++)
            for (int x = 0; x < res; x++)
                unityHeights[z, x] = heightmap[x, z];

        data.SetHeights(0, 0, unityHeights);
    }

    // ─── Fractal Voronoi generation ───────────────────────────────────────────

    /// <summary>
    /// Builds a heightmap using fractal (multi-octave) Voronoi noise.
    /// Each octave computes the distance to the nearest random feature point
    /// at a different frequency and amplitude, then sums the contributions.
    /// </summary>
    private float[,] GenerateVoronoiFractal(int width, int height)
    {
        float[,] map = new float[width, height];

        // Pre-generate per-octave offsets so different octaves sample different regions
        Vector2[] offsets = new Vector2[octaves];
        for (int o = 0; o < octaves; o++)
            offsets[o] = new Vector2((float)rng.NextDouble() * 10000f,
                                     (float)rng.NextDouble() * 10000f);

        float maxAmplitude = 0f;
        float amplitude = 1f;
        for (int o = 0; o < octaves; o++)
        {
            maxAmplitude += amplitude;
            amplitude *= persistence;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = x / (float)(width - 1);
                float ny = y / (float)(height - 1);

                float value = 0f;
                amplitude = 1f;
                float frequency = 1f;

                for (int o = 0; o < octaves; o++)
                {
                    float sx = (nx * frequency * voronoiScale * width) + offsets[o].x;
                    float sy = (ny * frequency * voronoiScale * height) + offsets[o].y;

                    // F1 Voronoi: distance to nearest feature point on a regular jittered grid
                    value += VoronoiF1(sx, sy) * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Invert so mountains are peaks (high F1 → flat, low F1 → ridge)
                map[x, y] = 1f - (value / maxAmplitude);
            }
        }

        return map;
    }

    /// <summary>
    /// Computes the F1 Voronoi value (distance to nearest feature point) at position (px, py).
    /// Uses a jittered grid so that each integer cell contains exactly one random feature point.
    /// </summary>
    private float VoronoiF1(float px, float py)
    {
        int ix = Mathf.FloorToInt(px);
        int iy = Mathf.FloorToInt(py);

        float minDist = float.MaxValue;

        // Search the 3×3 neighbourhood of grid cells to find the nearest feature point
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int cx = ix + dx;
                int cy = iy + dy;

                // Deterministic jitter derived from cell coordinates using a hash
                float fx = cx + Hash(cx, cy, 0);
                float fy = cy + Hash(cx, cy, 1);

                float distSq = (px - fx) * (px - fx) + (py - fy) * (py - fy);
                if (distSq < minDist)
                    minDist = distSq;
            }
        }

        return Mathf.Sqrt(minDist);
    }

    /// <summary>
    /// Deterministic hash producing a value in [0, 1) for a given cell and component index.
    /// </summary>
    private float Hash(int x, int y, int component)
    {
        // Mix the seed, cell coordinates and component through a simple integer hash
        unchecked
        {
            int h = seed ^ (x * 374761393) ^ (y * 668265263) ^ (component * 2246822519);
            h = (h ^ (h >> 13)) * 1274126177;
            h ^= (h >> 16);
            return (float)((uint)h) / (float)uint.MaxValue;
        }
    }

    // ─── Hydraulic erosion ────────────────────────────────────────────────────

    /// <summary>
    /// Simulates water-based erosion by iteratively:
    /// 1. Depositing rain water on the surface.
    /// 2. Moving water (and entrained sediment) to the lowest neighbour.
    /// 3. Eroding or depositing sediment based on sediment capacity and slope.
    /// 4. Evaporating a fraction of the surface water.
    ///
    /// This produces valleys, gullies, and alluvial fans characteristic of
    /// water-carved landscapes.
    /// </summary>
    private void ApplyHydraulicErosion()
    {
        int w = heightmap.GetLength(0);
        int h = heightmap.GetLength(1);

        float[,] water    = new float[w, h];
        float[,] sediment = new float[w, h];

        for (int iter = 0; iter < hydraulicIterations; iter++)
        {
            // Step 1 – Rain
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    water[x, y] += rainAmount;

            // Steps 2-3 – Flow, erode, deposit
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float totalHeight = heightmap[x, y] + water[x, y];

                    // Find steepest downhill neighbour (4-connectivity)
                    int   bestNx = -1, bestNy = -1;
                    float bestDelta = 0f;

                    int[] dxArr = { -1, 1,  0, 0 };
                    int[] dyArr = {  0, 0, -1, 1 };
                    for (int d = 0; d < 4; d++)
                    {
                        int nx = x + dxArr[d];
                        int ny = y + dyArr[d];
                        if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;

                        float delta = totalHeight - (heightmap[nx, ny] + water[nx, ny]);
                        if (delta > bestDelta)
                        {
                            bestDelta = delta;
                            bestNx = nx;
                            bestNy = ny;
                        }
                    }

                    if (bestNx < 0) continue; // local minimum – no flow

                    // Capacity-based erosion / deposition
                    float capacity  = bestDelta * water[x, y] * sedimentCapacity;
                    float carryingS = sediment[x, y];

                    if (carryingS < capacity)
                    {
                        // Erode surface
                        float erosion = Mathf.Min((capacity - carryingS) * depositionRate,
                                                  heightmap[x, y]);
                        heightmap[x, y] -= erosion;
                        sediment[x, y]  += erosion;
                    }
                    else
                    {
                        // Deposit excess sediment
                        float deposit = (carryingS - capacity) * depositionRate;
                        heightmap[x, y] += deposit;
                        sediment[x, y]  -= deposit;
                    }

                    // Move water and sediment downhill
                    float flowWater    = water[x, y] * flowFraction;
                    float flowSediment = sediment[x, y] * flowFraction;

                    water[x, y]    -= flowWater;
                    sediment[x, y] -= flowSediment;
                    water[bestNx, bestNy]    += flowWater;
                    sediment[bestNx, bestNy] += flowSediment;
                }
            }

            // Step 4 – Evaporate
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    water[x, y] *= (1f - evaporationRate);

                    // Deposit sediment where water dries up
                    if (water[x, y] < 0.001f)
                    {
                        heightmap[x, y] += sediment[x, y];
                        sediment[x, y]   = 0f;
                        water[x, y]      = 0f;
                    }
                }
            }
        }
    }

    // ─── Thermal erosion ──────────────────────────────────────────────────────

    /// <summary>
    /// Simulates slope-driven mass wasting (talus formation) by:
    /// Moving a fraction of excess material from steep slopes to lower neighbours.
    ///
    /// This smoothes sharp ridges and fills valleys with scree, producing the
    /// natural repose-angle characteristic of mountain talus fields.
    /// </summary>
    private void ApplyThermalErosion()
    {
        int w = heightmap.GetLength(0);
        int h = heightmap.GetLength(1);

        int[] dxArr = { -1, 1,  0, 0 };
        int[] dyArr = {  0, 0, -1, 1 };

        for (int iter = 0; iter < thermalIterations; iter++)
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float current = heightmap[x, y];

                    // Accumulate total height difference to lower neighbours
                    float totalDelta = 0f;
                    for (int d = 0; d < 4; d++)
                    {
                        int nx = x + dxArr[d];
                        int ny = y + dyArr[d];
                        if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;

                        float diff = current - heightmap[nx, ny];
                        if (diff > talusAngle)
                            totalDelta += diff;
                    }

                    if (totalDelta <= 0f) continue;

                    // Distribute material proportionally to height differences
                    for (int d = 0; d < 4; d++)
                    {
                        int nx = x + dxArr[d];
                        int ny = y + dyArr[d];
                        if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;

                        float diff = current - heightmap[nx, ny];
                        if (diff <= talusAngle) continue;

                        float move = thermalAmount * (diff - talusAngle) * (diff / totalDelta);
                        heightmap[x, y]   -= move;
                        heightmap[nx, ny] += move;
                    }
                }
            }
        }
    }

    // ─── Post-processing ──────────────────────────────────────────────────────

    /// <summary>Rescales the heightmap so all values fall within [0, 1].</summary>
    private void NormalizeHeightmap()
    {
        int w = heightmap.GetLength(0);
        int h = heightmap.GetLength(1);

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                if (heightmap[x, y] < min) min = heightmap[x, y];
                if (heightmap[x, y] > max) max = heightmap[x, y];
            }

        float range = max - min;
        if (range < 1e-6f) return;

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                heightmap[x, y] = (heightmap[x, y] - min) / range;
    }

    /// <summary>
    /// Applies a single pass of 3×3 box smoothing to remove high-frequency artefacts.
    /// </summary>
    private void SmoothHeightmap()
    {
        int w = heightmap.GetLength(0);
        int h = heightmap.GetLength(1);
        float[,] smoothed = new float[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float sum   = 0f;
                int   count = 0;
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;
                        sum += heightmap[nx, ny];
                        count++;
                    }
                }
                smoothed[x, y] = sum / count;
            }
        }

        heightmap = smoothed;
    }

    // ─── Unity lifecycle ──────────────────────────────────────────────────────

    /// <summary>Automatically generates terrain on scene start if a target Terrain is assigned.</summary>
    private void Start()
    {
        if (targetTerrain != null)
            GenerateTerrain();
    }
}
