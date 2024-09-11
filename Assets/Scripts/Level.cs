[System.Serializable]
public class LevelData
{
    public int levelNumber;    // The current level number
    public int duration;       // Level duration in seconds
    public SlotData[] slots;   // Array of slots that contain items
    public LevelType levelType; // Enum to define the level's difficulty (Normal, Hard, SuperHard)
}

[System.Serializable]
public class SlotData
{
    public int row;            // Slot row position
    public int column;         // Slot column position
    public string[] items;     // Items in this slot (could be layered, i.e., multiple items per slot)
}

public enum LevelType
{
    Normal,
    Hard,
    SuperHard
}
