using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShapeInstanceData
{
    public string shapeType;
    public bool rotated;

    // This property converts the string to an enum
    public ShapeType TypeEnum => (ShapeType)Enum.Parse(typeof(ShapeType), shapeType, true);
}

[Serializable]
public class LevelData
{
    public int levelNumber;
    public int fillerStarCount;
    public List<ShapeInstanceData> shapes;
}

[Serializable]
public class LevelCollection
{
    public List<LevelData> levels;

    // Create a LevelCollection object from a JSON string
    public static LevelCollection CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LevelCollection>(jsonString);
    }
}