using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Database of available combos for each Swordsman class.
/// Defines combo sequences, damage multipliers, and requirements.
/// </summary>
public static class ComboDatabase
{
    [System.Serializable]
    public class Combo
    {
        public string comboId;
        public string comboName;
        public int minHits;
        public int maxHits;
        public float sloppyMultiplier;
        public float preciseMultiplier;
        public string requiredSkill;
        public bool requiresDualWield;
    }

    private static Dictionary<string, List<Combo>> combosPerClass = new Dictionary<string, List<Combo>>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;

        // Swordsman (Base) Combos
        List<Combo> swordsmanCombos = new List<Combo>
        {
            new Combo
            {
                comboId = "basic_2hit",
                comboName = "Double Slash",
                minHits = 2,
                maxHits = 2,
                sloppyMultiplier = 0.2f,
                preciseMultiplier = 0.5f,
                requiredSkill = "basic_slash",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "basic_3hit",
                comboName = "Triple Slash",
                minHits = 3,
                maxHits = 3,
                sloppyMultiplier = 0.4f,
                preciseMultiplier = 1.0f,
                requiredSkill = "power_slash",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "basic_4hit",
                comboName = "Quad Slash",
                minHits = 4,
                maxHits = 4,
                sloppyMultiplier = 0.6f,
                preciseMultiplier = 1.5f,
                requiredSkill = "whirlwind_slash",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "basic_5hit",
                comboName = "Ultimate Combo",
                minHits = 5,
                maxHits = 5,
                sloppyMultiplier = 0.8f,
                preciseMultiplier = 2.0f,
                requiredSkill = "whirlwind_slash",
                requiresDualWield = false
            }
        };

        combosPerClass["Swordsman"] = swordsmanCombos;

        // Duelist Combos (Rapier variants)
        List<Combo> duelistCombos = new List<Combo>
        {
            new Combo
            {
                comboId = "duelist_2hit",
                comboName = "Precision Stab",
                minHits = 2,
                maxHits = 2,
                sloppyMultiplier = 0.15f,
                preciseMultiplier = 0.6f, // Duelists reward precision more
                requiredSkill = "riposte",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "duelist_3hit",
                comboName = "Riposte Chain",
                minHits = 3,
                maxHits = 3,
                sloppyMultiplier = 0.35f,
                preciseMultiplier = 1.2f,
                requiredSkill = "lunge",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "duelist_4hit",
                comboName = "Blade Dance",
                minHits = 4,
                maxHits = 4,
                sloppyMultiplier = 0.55f,
                preciseMultiplier = 1.7f,
                requiredSkill = "fleche",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "duelist_5hit",
                comboName = "Thousand Cuts",
                minHits = 5,
                maxHits = 5,
                sloppyMultiplier = 0.75f,
                preciseMultiplier = 2.3f,
                requiredSkill = "fleche",
                requiresDualWield = false
            }
        };

        combosPerClass["Duelist"] = duelistCombos;

        // Vanguard Combos (Heavy weapon variants)
        List<Combo> vanguardCombos = new List<Combo>
        {
            new Combo
            {
                comboId = "vanguard_2hit",
                comboName = "Heavy Strike",
                minHits = 2,
                maxHits = 2,
                sloppyMultiplier = 0.3f, // Vanguard does more damage even if sloppy
                preciseMultiplier = 0.6f,
                requiredSkill = "overhead_smash",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "vanguard_3hit",
                comboName = "Crushing Blow",
                minHits = 3,
                maxHits = 3,
                sloppyMultiplier = 0.5f,
                preciseMultiplier = 1.1f,
                requiredSkill = "cleave",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "vanguard_4hit",
                comboName = "Earthshaker",
                minHits = 4,
                maxHits = 4,
                sloppyMultiplier = 0.7f,
                preciseMultiplier = 1.6f,
                requiredSkill = "armor_break",
                requiresDualWield = false
            },
            new Combo
            {
                comboId = "vanguard_5hit",
                comboName = "Apocalypse Strike",
                minHits = 5,
                maxHits = 5,
                sloppyMultiplier = 0.9f,
                preciseMultiplier = 2.1f,
                requiredSkill = "armor_break",
                requiresDualWield = false
            }
        };

        combosPerClass["Vanguard"] = vanguardCombos;

        // Swordmaster Combos (Dual Wield variants)
        List<Combo> smasterCombos = new List<Combo>
        {
            new Combo
            {
                comboId = "smaster_2hit",
                comboName = "Dual Slash",
                minHits = 2,
                maxHits = 2,
                sloppyMultiplier = 0.25f,
                preciseMultiplier = 0.55f,
                requiredSkill = "dual_slash",
                requiresDualWield = true
            },
            new Combo
            {
                comboId = "smaster_3hit",
                comboName = "Dual Combo",
                minHits = 3,
                maxHits = 3,
                sloppyMultiplier = 0.45f,
                preciseMultiplier = 1.05f,
                requiredSkill = "dual_combo",
                requiresDualWield = true
            },
            new Combo
            {
                comboId = "smaster_4hit",
                comboName = "Whirlwind Dance",
                minHits = 4,
                maxHits = 4,
                sloppyMultiplier = 0.65f,
                preciseMultiplier = 1.55f,
                requiredSkill = "dual_combo",
                requiresDualWield = true
            },
            new Combo
            {
                comboId = "smaster_5hit",
                comboName = "Final Strike",
                minHits = 5,
                maxHits = 5,
                sloppyMultiplier = 0.85f,
                preciseMultiplier = 2.05f,
                requiredSkill = "final_strike",
                requiresDualWield = true
            }
        };

        combosPerClass["Swordmaster"] = smasterCombos;

        initialized = true;
    }

    public static List<Combo> GetCombosForClass(string className)
    {
        Initialize();
        return combosPerClass.ContainsKey(className) ? combosPerClass[className] : new List<Combo>();
    }

    public static Combo GetCombo(string className, int hits)
    {
        Initialize();
        var combos = GetCombosForClass(className);

        foreach (var combo in combos)
        {
            if (combo.minHits == hits)
                return combo;
        }

        return null;
    }
}