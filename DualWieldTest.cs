using UnityEngine;

/// <summary>
/// Test the hidden dual wield feature.
/// </summary>
public class DualWieldTest : MonoBehaviour
{
    [SerializeField] private Player testPlayer;

    private void Update()
    {
        // F1: Create Swordsman with main sword
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (testPlayer == null)
                testPlayer = FindObjectOfType<Player>();

            Debug.Log("\n=== TEST 1: Create Swordsman ===");
            testPlayer.InitializePlayer("Swordsman", "");

            // Give them a main sword
            Equipment equipment = testPlayer.GetComponent<Equipment>();
            Equipment.EquipmentItem mainSword = new Equipment.EquipmentItem
            {
                itemId = "iron_sword",
                itemName = "Iron Sword",
                description = "A standard iron sword",
                rarity = ItemRarity.Uncommon,
                damage = 15f,
                isDualWieldSword = false
            };
            equipment.EquipRightHand(mainSword);
        }

        // F2: Give them a hidden dual wield sword
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (testPlayer == null)
                testPlayer = FindObjectOfType<Player>();

            Debug.Log("\n=== TEST 2: Receive Hidden Dual Wield Sword ===");
            Equipment equipment = testPlayer.GetComponent<Equipment>();
            
            Equipment.EquipmentItem dualWieldSword = new Equipment.EquipmentItem
            {
                itemId = "frost_edge_dual",
                itemName = "Frost Edge",
                description = "A blade of pure ice. Lighter than it should be, as if meant to be wielded alongside another blade...",
                rarity = ItemRarity.Epic,
                damage = 20f,
                isDualWieldSword = true  // HIDDEN!
            };
            
            equipment.AddItemToInventory(dualWieldSword);
        }

        // F3: Equip the dual wield sword to left hand
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (testPlayer == null)
                testPlayer = FindObjectOfType<Player>();

            Debug.Log("\n=== TEST 3: Equip Dual Wield Sword ===");
            Equipment equipment = testPlayer.GetComponent<Equipment>();
            
            var inventory = equipment.GetInventory();
            if (inventory.Count > 0)
            {
                var dualWieldSword = inventory[0];
                if (equipment.EquipLeftHand(dualWieldSword))
                {
                    DualWieldDiscovery.DiscoverDualWield(testPlayer, dualWieldSword);
                    
                    // Activate dual wield system
                    DualWieldSystem dualWield = testPlayer.GetComponent<DualWieldSystem>();
                    if (dualWield == null)
                    {
                        dualWield = testPlayer.gameObject.AddComponent<DualWieldSystem>();
                        dualWield.Initialize();
                    }
                }
            }
        }

        // F4: Test combo hits
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (testPlayer == null)
                testPlayer = FindObjectOfType<Player>();

            DualWieldSystem dualWield = testPlayer.GetComponent<DualWieldSystem>();
            if (dualWield != null && dualWield.IsDualWieldActive)
            {
                Debug.Log("\n=== TEST 4: Registering 5 Combo Hits ===");
                for (int i = 0; i < 5; i++)
                {
                    dualWield.RegisterHit();
                }
            }
        }

        // F5: Execute final strike
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (testPlayer == null)
                testPlayer = FindObjectOfType<Player>();

            DualWieldSystem dualWield = testPlayer.GetComponent<DualWieldSystem>();
            if (dualWield != null)
            {
                Debug.Log("\n=== TEST 5: Execute Final Strike ===");
                float damage = dualWield.ExecuteFinalStrike();
            }
        }
    }
}