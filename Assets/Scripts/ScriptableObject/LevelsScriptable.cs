using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;
using System;


public enum CompleteConditionType { Score, Combo, Timing }
public enum CompleteConditionComparator { Min, Max }

/////////////////////////////  LEVEL  ////////////////////////////////

[CreateAssetMenu(fileName = "SC_Level_", menuName = "Custom/Level", order = 120)]
[System.Serializable]
public class LevelsScriptable : ScriptableObject, IComparable<LevelsScriptable>, IEquatable<LevelsScriptable>
{
    public LevelSettings level;

    public int CompareTo(LevelsScriptable secondLevel)
    {
        if (secondLevel.level.levelProgression.levelPos.y > level.levelProgression.levelPos.y)
            return -1;
        else if (secondLevel.level.levelProgression.levelPos.y == level.levelProgression.levelPos.y)
            return 0;
        else
            return 1;
    }

    public bool Equals(LevelsScriptable secondLevel)
    {
        return CompareTo(secondLevel) == 0;
    }

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
public class LevelSpecifics
{
    public string levelName;
    public float timeForThisLevel;
    public float impactRadiusForThisLevel;
    public float balleSpeedForThisLevel;

    public ColorSwitchBehaviour switchColorBehaviourForThisLevel;
    public bool suddenDeath;
    public bool timeAttack;
    public bool noWallsMode;
    public bool mandatoryBounce;
    public AudioClip musicForThisLevel;

    public LevelSpecifics()
    {
        levelName = "NO NAME";

        timeForThisLevel = 0f;
        impactRadiusForThisLevel = 1.7f;
        balleSpeedForThisLevel = 1f;

        switchColorBehaviourForThisLevel = ColorSwitchBehaviour.NORMAL;
        suddenDeath = false;
        noWallsMode = false;
        mandatoryBounce = false;
        musicForThisLevel = null;
    }
}

///////////////////////////  PROGRESSION  //////////////////////////////
[System.Serializable]
public class ProgressionSettings
{
    public string buttonName;

    public int LevelIndex;

    public bool isDone;
    public bool isUnlocked;
    public int starsRequired;
    public bool isImplemented;

    public int maxScore;
    public int maxCombo;
    public int minTiming;

    public int numberOfAdditionalConditions;
    public List<LevelsScriptable> unlockConditions;

    public Vector2 levelPos;
    public int levelID;

    public LevelConditions[] conditionsToComplete;
    public int numberOfConditionCompleted;

    public ProgressionSettings()
    {
        buttonName = "00";
        LevelIndex = 0;
        isDone = false;
        isUnlocked = false;
        starsRequired = 0;
        isImplemented = false;

        unlockConditions = new List<LevelsScriptable>();

        numberOfAdditionalConditions = 2;
        conditionsToComplete = new LevelConditions[numberOfAdditionalConditions];
    }
}

[System.Serializable]
public struct LevelConditions
{
    public CompleteConditionType conditionType;
    public CompleteConditionComparator conditionComparator;
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
