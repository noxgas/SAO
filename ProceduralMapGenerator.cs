using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedural map generator.
/// Produces a 2-mile × 2-mile (~3218 m × 3218 m) terrain with hills, valleys
/// and plains using layered Perlin noise.  Trees and rocks are scattered across
/// the surface as stand-in resource objects.  The terrain is chunked for Quest 2
/// performance: only chunks near the player are kept in memory.
/// </summary>
public class ProceduralMapGenerator : MonoBehaviour
{
    // ── Map dimensions ──────────────────────────────────────────────────────
    private const float MilesInMeters = 1609.34f;

    [Header("Map Size")]
    [SerializeField] private float mapSizeMeters = 2f * MilesInMeters; // ~3218 m
    [SerializeField] private int chunkResolution = 65;   // vertices per chunk side (power-of-2 + 1)
    [SerializeField] private float chunkWorldSize = 200f; // metres per chunk
    [SerializeField] private int activeChunkRadius = 2;   // chunks around the player to keep loaded

    [Header("Terrain Height")]
    [SerializeField] private float maxHeight = 80f;
    [SerializeField] private float noiseScale = 0.003f;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private Vector2 noiseOffset;

    [Header("Resources")]
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private float treeSpawnChance = 0.15f;
    [SerializeField] private float rockSpawnChance = 0.05f;
    [Tooltip("Grid spacing (metres) between resource sample points within a chunk.")]
    [SerializeField] private float resourceGridSpacing = 10f;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    // Runtime
    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    private int seed;
    private Material plainsMaterial;
    private Material forestMaterial;

    private void Start()
    {
        seed = Random.Range(0, 100000);
        noiseOffset = new Vector2(seed * 0.1f, seed * 0.1f);

        // Pre-build shared biome materials (one allocation per biome)
        plainsMaterial = new Material(Shader.Find("Standard"));
        plainsMaterial.color = new Color(0.4f, 0.7f, 0.2f);
        forestMaterial = new Material(Shader.Find("Standard"));
        forestMaterial.color = new Color(0.1f, 0.4f, 0.1f);

        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        UpdateChunks();
    }

    private void Update()
    {
        UpdateChunks();
    }

    // ── Chunk management ────────────────────────────────────────────────────

    private void UpdateChunks()
    {
        if (playerTransform == null) return;

        Vector2Int playerChunk = WorldToChunk(playerTransform.position);

        // Load chunks within radius
        for (int dx = -activeChunkRadius; dx <= activeChunkRadius; dx++)
        {
            for (int dz = -activeChunkRadius; dz <= activeChunkRadius; dz++)
            {
                Vector2Int coord = new Vector2Int(playerChunk.x + dx, playerChunk.y + dz);

                if (!IsChunkInBounds(coord)) continue;

                if (!loadedChunks.ContainsKey(coord))
                    loadedChunks[coord] = GenerateChunk(coord);
            }
        }

        // Unload distant chunks
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kv in loadedChunks)
        {
            if (Mathf.Abs(kv.Key.x - playerChunk.x) > activeChunkRadius + 1 ||
                Mathf.Abs(kv.Key.y - playerChunk.y) > activeChunkRadius + 1)
            {
                Destroy(kv.Value);
                toRemove.Add(kv.Key);
            }
        }
        foreach (var key in toRemove)
            loadedChunks.Remove(key);
    }

    private Vector2Int WorldToChunk(Vector3 worldPos)
    {
        int cx = Mathf.FloorToInt(worldPos.x / chunkWorldSize);
        int cz = Mathf.FloorToInt(worldPos.z / chunkWorldSize);
        return new Vector2Int(cx, cz);
    }

    private bool IsChunkInBounds(Vector2Int coord)
    {
        int totalChunks = Mathf.CeilToInt(mapSizeMeters / chunkWorldSize);
        return coord.x >= 0 && coord.x < totalChunks &&
               coord.y >= 0 && coord.y < totalChunks;
    }

    // ── Chunk generation ────────────────────────────────────────────────────

    private GameObject GenerateChunk(Vector2Int chunkCoord)
    {
        GameObject chunkGO = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
        chunkGO.transform.SetParent(transform);
        chunkGO.transform.position = new Vector3(
            chunkCoord.x * chunkWorldSize,
            0f,
            chunkCoord.y * chunkWorldSize);

        // Build mesh
        Mesh mesh = BuildTerrainMesh(chunkCoord);

        MeshFilter mf = chunkGO.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = chunkGO.AddComponent<MeshRenderer>();
        mr.material = GetBiomeMaterial(chunkCoord);

        MeshCollider mc = chunkGO.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;

        // Place resources
        PlaceResources(chunkGO.transform, chunkCoord);

        return chunkGO;
    }

    private Mesh BuildTerrainMesh(Vector2Int chunkCoord)
    {
        int vCount = chunkResolution;
        float step = chunkWorldSize / (vCount - 1);

        Vector3[] vertices  = new Vector3[vCount * vCount];
        Vector2[] uvs       = new Vector2[vCount * vCount];
        int[]     triangles = new int[(vCount - 1) * (vCount - 1) * 6];

        float worldOffsetX = chunkCoord.x * chunkWorldSize;
        float worldOffsetZ = chunkCoord.y * chunkWorldSize;

        for (int z = 0; z < vCount; z++)
        {
            for (int x = 0; x < vCount; x++)
            {
                float wx = worldOffsetX + x * step;
                float wz = worldOffsetZ + z * step;
                float h  = SampleHeight(wx, wz);

                int idx = z * vCount + x;
                vertices[idx] = new Vector3(x * step, h, z * step);
                uvs[idx]      = new Vector2((float)x / (vCount - 1), (float)z / (vCount - 1));
            }
        }

        int t = 0;
        for (int z = 0; z < vCount - 1; z++)
        {
            for (int x = 0; x < vCount - 1; x++)
            {
                int bl = z * vCount + x;
                int br = bl + 1;
                int tl = bl + vCount;
                int tr = tl + 1;

                triangles[t++] = bl; triangles[t++] = tl; triangles[t++] = tr;
                triangles[t++] = bl; triangles[t++] = tr; triangles[t++] = br;
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices  = vertices;
        mesh.uv        = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // ── Noise ───────────────────────────────────────────────────────────────

    private float SampleHeight(float worldX, float worldZ)
    {
        float amplitude  = 1f;
        float frequency  = 1f;
        float noiseValue = 0f;
        float maxAmp     = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = (worldX * noiseScale + noiseOffset.x) * frequency;
            float sampleZ = (worldZ * noiseScale + noiseOffset.y) * frequency;
            noiseValue += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
            maxAmp    += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return (noiseValue / maxAmp) * maxHeight;
    }

    // ── Biome ────────────────────────────────────────────────────────────────

    private Material GetBiomeMaterial(Vector2Int chunkCoord)
    {
        int totalChunks = Mathf.CeilToInt(mapSizeMeters / chunkWorldSize);
        float nx = chunkCoord.x / (float)totalChunks;
        float nz = chunkCoord.y / (float)totalChunks;
        float distFromCentre = Vector2.Distance(new Vector2(nx, nz), Vector2.one * 0.5f);
        return distFromCentre < 0.25f ? plainsMaterial : forestMaterial;
    }

    // ── Resources ────────────────────────────────────────────────────────────

    private void PlaceResources(Transform chunkParent, Vector2Int chunkCoord)
    {
        System.Random rng = new System.Random(seed + chunkCoord.x * 1000 + chunkCoord.y);
        int steps = Mathf.Max(1, Mathf.FloorToInt(chunkWorldSize / resourceGridSpacing));

        for (int z = 0; z <= steps; z++)
        {
            for (int x = 0; x <= steps; x++)
            {
                // Local position within the chunk
                float lx = (x / (float)steps) * chunkWorldSize;
                float lz = (z / (float)steps) * chunkWorldSize;

                // Add a little jitter so resources don't sit on a perfect grid
                lx += (float)(rng.NextDouble() - 0.5) * resourceGridSpacing * 0.8f;
                lz += (float)(rng.NextDouble() - 0.5) * resourceGridSpacing * 0.8f;

                float worldX = chunkParent.position.x + lx;
                float worldZ = chunkParent.position.z + lz;
                float height = SampleHeight(worldX, worldZ);
                Vector3 worldPos = new Vector3(worldX, height, worldZ);

                float r = (float)rng.NextDouble();
                if (r < treeSpawnChance && treePrefab != null)
                {
                    Instantiate(treePrefab, worldPos,
                        Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0), chunkParent)
                        .tag = "Resource_Tree";
                }
                else if (r < treeSpawnChance + rockSpawnChance && rockPrefab != null)
                {
                    Instantiate(rockPrefab, worldPos,
                        Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0), chunkParent)
                        .tag = "Resource_Rock";
                }
            }
        }
    }
}
