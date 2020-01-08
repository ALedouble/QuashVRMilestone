using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;


/////////////////////////////  LEVEL  ////////////////////////////////

[CreateAssetMenu(fileName = "SC_Level_", menuName = "Custom/Level", order = 120)]
[System.Serializable]
public class LevelsScriptable : ScriptableObject
{
    public LevelSettings level;
    public float timeForThisLevel;

    public LevelsScriptable()
    {
        level = new LevelSettings();
    }
}


[System.Serializable]
public class LevelSettings
{
    public WallBuilds levelWallBuilds;
    public ProgressionSettings levelProgression;

    public LevelSettings()
    {
        levelWallBuilds = new WallBuilds();
        levelProgression = new ProgressionSettings();
    }
}



///////////////////////////  PROGRESSION  //////////////////////////////

[System.Serializable]
public struct ProgressionSettings
{
    public bool isDone;
    public bool isUnlocked;

    public LevelsScriptable[] unlockConditions;

    public Vector2 levelPos;
    public int score;
    public int stagePos;
    public int levelID;
}



/////////////////////////////  BRICKS  ////////////////////////////////

[System.Serializable]
/// <summary>
/// Every walls for One level
/// </summary>
public class WallBuilds
{
    public Wall[] walls;

    public WallBuilds()
    {
        walls = new Wall[500];
    }
}

[System.Serializable]
/// <summary>
/// One Wall Composition of bricks
/// </summary>
public class Wall
{
    public List<BrickSettings> wallBricks;

    public Wall(int numberOfBricks)
    {

        wallBricks = new List<BrickSettings>();

        for (int i = 0; i < numberOfBricks; i++)
        {
            wallBricks.Add(new BrickSettings());
        }
    }
}

[System.Serializable]
/// <summary>
/// Brick Parameters
/// </summary>
public struct BrickSettings
{
    public bool isBrickHere;
    public string brickID;

    //public GameObject prefab;
    public Vector3 brickPosition;

    public bool isMoving;
    public float speed;
    public float smoothTime;
    public List<Vector3> waypointsStorage;

    public int brickColorPreset;
    public int brickTypePreset;
    public bool isMalus;
    public bool isBonus;
}
