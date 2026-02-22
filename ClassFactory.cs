using UnityEngine;

/// <summary>
/// Factory pattern for creating character classes.
/// Handles playstyle names for Swordsman.
/// </summary>
public static class ClassFactory
{
    public static ICharacterClass CreateClass(string className, string playstyleName = "")
    {
        switch (className.ToLower())
        {
            case "swordsman":
                // Default to single sword if no playstyle specified
                SwordsmanPlaystyle playstyle = SwordsmanPlaystyle.SingleSword;

                if (!string.IsNullOrEmpty(playstyleName))
                {
                    playstyle = playstyleName.ToLower() switch
                    {
                        "singlesword" => SwordsmanPlaystyle.SingleSword,
                        "dualwield" => SwordsmanPlaystyle.DualWield,
                        _ => SwordsmanPlaystyle.SingleSword
                    };
                }

                return new Swordsman(playstyle);

            case "tank":
                // Tank logic...
                return new Tank(TankSubclass.ShieldTank);

            case "mage":
                // Mage logic...
                return new Mage(MageSubclass.AttackMage);
        }

        Debug.LogError($"Class not found: {className}");
        return null;
    }
}