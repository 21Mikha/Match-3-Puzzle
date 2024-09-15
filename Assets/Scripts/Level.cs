using UnityEngine;

[System.Serializable]
public class Level
{
    public int level;
    public LevelType levelType;
    public float time;
    public int itemsNumber;
    public SlotData[] slots;

    [System.Serializable]
    public class SlotData
    {
        public PositionData position;
        public string specialEffect;
        public PlacesData places;
    }

    [System.Serializable]
    public class PositionData
    {
        public float x;
        public float y;
        public float z;

        // Convert the JSON position data -> Unity's Vector3
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class PlacesData
    {
        public int[] left;
        public int[] middle;
        public int[] right;
    }
}

public enum LevelType
{
    Normal,
    Hard,
    SuperHard
}
