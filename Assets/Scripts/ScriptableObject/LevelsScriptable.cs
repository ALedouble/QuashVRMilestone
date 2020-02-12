﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;


public enum CompleteConditionType { Score, Combo, Timing }

/////////////////////////////  LEVEL  ////////////////////////////////

[CreateAssetMenu(fileName = "SC_Level_", menuName = "Custom/Level", order = 120)]
[System.Serializable]
public class LevelsScriptable : ScriptableObject
{
    public LevelSettings level;

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
    public LevelSpecifics levelSpec;

    public LevelSettings()
    {
        levelWallBuilds = new WallBuilds();
        levelProgression = new ProgressionSettings();
        levelSpec = new LevelSpecifics();
    }
}

///////////////////////////  PARAMETRES du level  //////////////////////////////

[System.Serializable]
public struct LevelSpecifics
{
    public float timeForThisLevel;
    public float impactRadiusForThisLevel;
}

///////////////////////////  PROGRESSION  //////////////////////////////
[System.Serializable]
public class ProgressionSettings
{
    public bool isDone;
    public bool isUnlocked;
    public int starsRequired;
    public bool isImplemented;

    public int maxScore;
    public int maxCombo;
    public int maxTiming;

    public List<LevelsScriptable> unlockConditions;

    public Vector2 levelPos;
    public int levelID;

    public LevelConditions[] conditionsToComplete;
    public int numberOfConditionCompleted;

    public ProgressionSettings()
    {
        conditionsToComplete = new LevelConditions[3];
    }
}


[System.Serializable]
public struct LevelConditions
{
    public CompleteConditionType conditionType;
    public int conditionReachedAt;
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
