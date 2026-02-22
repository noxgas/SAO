// Add this to Player.cs in Awake or Start

private ComboSystem comboSystem;

private void InitializeComboSystem()
{
    comboSystem = GetComponent<ComboSystem>();
    if (comboSystem == null)
        comboSystem = gameObject.AddComponent<ComboSystem>();
}

// Call this in InitializePlayer:
public void InitializePlayer(string className, string subclassName)
{
    // ... existing code ...
    
    // Initialize combo system for Swordsmen
    if (className.ToLower() == "swordsman")
    {
        InitializeComboSystem();
        ComboDatabase.Initialize();
    }
}