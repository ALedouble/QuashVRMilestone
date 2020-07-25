using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Steamworks;


[System.Serializable]
public class SavedObject
{
    public string saveGameVersion;
    public List<SavedValues> savedObjects;

    public SavedObject()
    {
        saveGameVersion = "";
        savedObjects = new List<SavedValues>();
    }
}

[System.Serializable]
public class SavedValues
{
    public bool unlock;
    public bool done;
    public int bestScore;
    public int bestCombo;
    public int bestTime;

    public SavedValues()
    {
        unlock = false;
        done = false;
        bestScore = 0;
        bestCombo = 0;
        bestTime = 0;
    }
}


public class JSON : MonoBehaviour
{
    public static JSON instance;
    private string currentGameVersion = "0.9.1";
    public LevelsScriptable currentLevelFocused;
    public bool isGoingStraightToCampaign = false;
    public bool devMode;

    [SerializeField] List<LevelsScriptable> levelsToSave = new List<LevelsScriptable>();
    string saveFileName = "/QuashSave.sav";

    public string GetFilePath()
    {
        return Application.persistentDataPath + saveFileName;
    }

    public string GetFilePathWithSteamID()
    {
        if (devMode)
        {
            return GetFilePath();
        }
        else
        {
            if (SteamManager.Initialized)
                return Application.persistentDataPath + "/" + SteamUser.GetSteamID() + saveFileName;
            else
                return GetFilePath();
        }

    }

    private string GetDirectory()
    {
        if (devMode)
        {
            return Application.persistentDataPath;
        }
        else
        {
            return Application.persistentDataPath + "/" + SteamUser.GetSteamID();
        }
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Set Up Datas
    /// </summary>
    public void SetUpDATAs()
    {
        levelsToSave = Campaign.instance.levelsImplemented;

        if (!File.Exists(GetFilePathWithSteamID()))
        {
            Debug.Log("N 'EXISTE PAS");
            if (!Directory.Exists(GetDirectory()))
                Directory.CreateDirectory(GetDirectory());

            SaveDATA(null);
        }

        LoadDATA();
    }

    /// <summary>
    /// Check the data that are to be sent
    /// </summary>
    /// <param name="level">Level concerned by the submit</param>
    /// <param name="score">Player score</param>
    /// <param name="combo">Player combo</param>
    /// <param name="time">Player Time</param>
    public void SubmitDATA(LevelsScriptable level, int score, int combo, int time)
    {
        //Debug.Log("Submiting DATA");
        SavedValues levelValue = new SavedValues
        {
            unlock = true,
            done = true,
            bestCombo = level.level.levelProgression.maxCombo,
            bestScore = level.level.levelProgression.maxScore,
            bestTime = level.level.levelProgression.minTiming
        };

        bool isThereComboCondition = false;
        bool isThereScoreCondition = false;
        bool isThereTimeCondition = false;


        //Debug.Log("score : " + score);
        //Debug.Log("combo : " + combo);
        //Debug.Log("time : " + time);

        for (int i = 0; i < level.level.levelProgression.numberOfAdditionalConditions; i++)
        {
            if (level.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
            {
                switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                {
                    case CompleteConditionType.Score:
                        //Debug.Log("Checking Score");
                        if (score > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (score > level.level.levelProgression.maxScore)
                                levelValue.bestScore = score;
                            else
                                levelValue.bestScore = level.level.levelProgression.maxScore;
                        }
                        else if (score > level.level.levelProgression.maxScore)
                            levelValue.bestScore = score;
                        else
                            levelValue.bestScore = level.level.levelProgression.maxScore;

                        isThereScoreCondition = true;
                        break;

                    case CompleteConditionType.Combo:
                        if (combo > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (combo > level.level.levelProgression.maxCombo)
                                levelValue.bestCombo = combo;
                            else
                                levelValue.bestCombo = level.level.levelProgression.maxCombo;
                        }
                        else if (combo > level.level.levelProgression.maxCombo)
                            levelValue.bestCombo = combo;
                        else
                            levelValue.bestCombo = level.level.levelProgression.maxCombo;

                        isThereComboCondition = true;
                        break;

                    case CompleteConditionType.Timing:
                        if (!level.level.levelProgression.isDone)
                        {
                            levelValue.bestTime = time;
                            break;
                        }

                        if (time > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (time > level.level.levelProgression.minTiming)
                            {
                                levelValue.bestTime = time;
                            }
                            else
                            {
                                levelValue.bestTime = level.level.levelProgression.minTiming;
                            }
                        }
                        else
                            levelValue.bestTime = level.level.levelProgression.minTiming;

                        isThereTimeCondition = true;
                        break;
                }
            }
            else
            {
                switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                {
                    case CompleteConditionType.Score:
                        if (score < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (score > level.level.levelProgression.maxScore)
                                levelValue.bestScore = score;
                            else
                                levelValue.bestScore = level.level.levelProgression.maxScore;
                        }
                        else if (score > level.level.levelProgression.maxScore)
                            levelValue.bestScore = score;
                        else
                            levelValue.bestScore = level.level.levelProgression.maxScore;

                        isThereScoreCondition = true;
                        break;

                    case CompleteConditionType.Combo:
                        if (combo < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (combo > level.level.levelProgression.maxCombo)
                                levelValue.bestCombo = combo;
                            else
                                levelValue.bestCombo = level.level.levelProgression.maxCombo;
                        }
                        else if (combo > level.level.levelProgression.maxCombo)
                            levelValue.bestCombo = combo;
                        else
                            levelValue.bestCombo = level.level.levelProgression.maxCombo;

                        isThereComboCondition = true;
                        break;

                    case CompleteConditionType.Timing:
                        if (level.level.levelProgression.isDone)
                        {
                            if (time < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                            {
                                if (time < level.level.levelProgression.minTiming)
                                    levelValue.bestTime = time;
                                else
                                    levelValue.bestTime = level.level.levelProgression.minTiming;
                            }
                            else if (time < level.level.levelProgression.minTiming)
                                levelValue.bestTime = time;
                            else
                                levelValue.bestTime = level.level.levelProgression.minTiming;
                        }
                        else
                        {
                            levelValue.bestTime = time;
                        }


                        isThereTimeCondition = true;
                        break;
                }
            }
        }

        //Debug.Log("isThereComboCondition : " + isThereComboCondition);
        //Debug.Log("isThereScoreCondition : " + isThereScoreCondition);
        //Debug.Log("isThereTimeCondition : " + isThereTimeCondition);

        if (!isThereComboCondition)
        {
            //Debug.Log("NO COMBO CONDITION");

            if (combo > level.level.levelProgression.maxCombo)
                levelValue.bestCombo = combo;
            else
                levelValue.bestCombo = level.level.levelProgression.maxCombo;
        }

        if (!isThereTimeCondition)
        {
            //Debug.Log("NO SCORE CONDITION");
            if (level.level.levelProgression.isDone)
            {
                if (time < level.level.levelProgression.minTiming)
                    levelValue.bestTime = time;
                else
                    levelValue.bestTime = level.level.levelProgression.minTiming;
            }
            else
            {
                levelValue.bestTime = time;
            }

        }

        if (!isThereScoreCondition)
        {
            //Debug.Log("NO TIME CONDITION");

            if (score > level.level.levelProgression.maxScore)
                levelValue.bestScore = score;
            else
                levelValue.bestScore = level.level.levelProgression.maxScore;
        }

        //Debug.Log("levelValue.bestTime : " + levelValue.bestTime);
        //Debug.Log("levelValue.bestCombo : " + levelValue.bestCombo);
        //Debug.Log("levelValue.bestScore : " + levelValue.bestScore);
        //Debug.Log("levelValue.unlock : " + levelValue.unlock);
        //Debug.Log("levelValue.done : " + levelValue.done);

        SaveLevelDATA(levelValue, level.level.levelProgression.LevelIndex);
    }

    /// <summary>
    /// Save DATA 4 one level
    /// </summary>
    /// <param name="levelValues">New DATAs "SavedValues" for the level</param>
    /// <param name="levelIndex">Level index from the level (you can found it in LevelProgression.levelIndex) </param>
    public void SaveLevelDATA(SavedValues levelValues, int levelIndex)
    {
        if (levelsToSave.Count == 0)
        {
            Debug.LogError("NO DATA TO SAVE ! ASSHOLE");
            return;
        }

        //New Virgin Save DATAs
        SavedObject newDATA = new SavedObject() { };
        newDATA.saveGameVersion = currentGameVersion;

        //DATA presented
        if (levelValues == null)
        {
            Debug.LogError("NO VALUES TO SAVE");
            return;
        }

        string loadString = File.ReadAllText(GetFilePathWithSteamID());
        SavedObject loadDATA = JsonUtility.FromJson<SavedObject>(loadString);

        for (int i = 0; i < levelsToSave.Count; i++)
        {
            if (i != levelIndex)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = loadDATA.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = loadDATA.savedObjects[i].unlock;

                newDATA.savedObjects[i].bestScore = loadDATA.savedObjects[i].bestScore;
                newDATA.savedObjects[i].bestCombo = loadDATA.savedObjects[i].bestCombo;
                newDATA.savedObjects[i].bestTime = loadDATA.savedObjects[i].bestTime;
            }
            else
            {
                Debug.Log("Data is Saved for level : " + levelsToSave[i]);
                newDATA.savedObjects.Add(new SavedValues());
                newDATA.savedObjects[i] = levelValues;
            }
        }


        string json = JsonUtility.ToJson(newDATA);

        File.WriteAllText(GetFilePathWithSteamID(), json);
    }

    /// <summary>
    /// Saving DATAs
    /// </summary>
    public void SaveDATA(SavedObject presentedDATA)
    {
        if (levelsToSave.Count == 0)
        {
            Debug.LogError("NO DATA TO SAVE ! ASSHOLE");
            return;
        }

        //New Virgin Save DATAs
        SavedObject newDATA = new SavedObject() { };


        //Reset DATAs
        if (presentedDATA == null)
        {
            Debug.Log("RESET DATA");
            presentedDATA = new SavedObject() { };
            presentedDATA.saveGameVersion = currentGameVersion;

            for (int i = 0; i < levelsToSave.Count; i++)
            {
                presentedDATA.savedObjects.Add(new SavedValues());
                if (i == levelsToSave.Count - 1)
                    presentedDATA.savedObjects[i].unlock = true;
                else
                    presentedDATA.savedObjects[i].unlock = false;

                presentedDATA.savedObjects[i].done = false;

                presentedDATA.savedObjects[i].bestScore = 0;
                presentedDATA.savedObjects[i].bestCombo = 0;
                //presentedDATA.savedObjects[i].bestTime = (int)levelsToSave[i].level.levelSpec.timeForThisLevel;
                presentedDATA.savedObjects[i].bestTime = 0;
            }
        }

        string json = "";

        //If there's already DATA on file
        if (File.Exists(GetFilePathWithSteamID()))
        {
            //Debug.Log("DATA IS HERE");

            string loadString = File.ReadAllText(GetFilePathWithSteamID());
            SavedObject loadDATA = JsonUtility.FromJson<SavedObject>(loadString);

            CheckGameVersion(loadDATA);

            for (int i = 0; i < levelsToSave.Count; i++)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = presentedDATA.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = presentedDATA.savedObjects[i].unlock;

                //Check BEST score
                if (presentedDATA.savedObjects[i].bestScore > loadDATA.savedObjects[i].bestScore)
                    newDATA.savedObjects[i].bestScore = presentedDATA.savedObjects[i].bestScore;
                else
                    newDATA.savedObjects[i].bestScore = loadDATA.savedObjects[i].bestScore;

                //Check BEST Combo
                if (presentedDATA.savedObjects[i].bestCombo > loadDATA.savedObjects[i].bestCombo)
                    newDATA.savedObjects[i].bestCombo = presentedDATA.savedObjects[i].bestCombo;
                else
                    newDATA.savedObjects[i].bestCombo = loadDATA.savedObjects[i].bestCombo;

                //Check BEST Time
                if (presentedDATA.savedObjects[i].bestTime < loadDATA.savedObjects[i].bestTime)
                    newDATA.savedObjects[i].bestTime = presentedDATA.savedObjects[i].bestTime;
                else
                    newDATA.savedObjects[i].bestTime = loadDATA.savedObjects[i].bestTime;
            }

            json = JsonUtility.ToJson(newDATA);
        }
        else //NO DATA on file
        {
            Debug.Log("NO DATA");

            json = JsonUtility.ToJson(presentedDATA);
        }

        File.WriteAllText(GetFilePathWithSteamID(), json);
    }


    public void CheckGameVersion(SavedObject saveVersion)
    {
        if (saveVersion.saveGameVersion != currentGameVersion)
        {
            Debug.Log("Save VERSION OLDLLDLDLLDLDLDLDL");


            //Changes with new save version

            SavedObject newDATA = new SavedObject() { };
            newDATA.saveGameVersion = currentGameVersion;

            string savedString = File.ReadAllText(GetFilePathWithSteamID());
            SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

            string json = "";

            for (int i = 0; i < levelsToSave.Count; i++)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = loadObject.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = loadObject.savedObjects[i].done;

                newDATA.savedObjects[i].bestScore = loadObject.savedObjects[i].bestScore;
                newDATA.savedObjects[i].bestCombo = loadObject.savedObjects[i].bestCombo;
                newDATA.savedObjects[i].bestTime = loadObject.savedObjects[i].bestTime;

            }

            json = JsonUtility.ToJson(newDATA);
            File.WriteAllText(GetFilePathWithSteamID(), json);

            Debug.Log(saveVersion.saveGameVersion);
        }
        else
        {
            Debug.Log("GOOD VERSION");
        }

    }

    /// <summary>
    /// LOADING DATAs
    /// </summary>
    public void LoadDATA()
    {
        if (!File.Exists(GetFilePathWithSteamID()))
        {
            Debug.Log("NO FILE TO LOAD");
            return;
        }

        Debug.Log("LOADING DATAS");

        string savedString = File.ReadAllText(GetFilePathWithSteamID());
        SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);


        CheckGameVersion(loadObject);

        for (int i = 0; i < levelsToSave.Count; i++)
        {
            levelsToSave[i].level.levelProgression.isUnlocked = loadObject.savedObjects[i].unlock;
            levelsToSave[i].level.levelProgression.isDone = loadObject.savedObjects[i].done;
            levelsToSave[i].level.levelProgression.maxScore = loadObject.savedObjects[i].bestScore;
            levelsToSave[i].level.levelProgression.maxCombo = loadObject.savedObjects[i].bestCombo;
            levelsToSave[i].level.levelProgression.minTiming = loadObject.savedObjects[i].bestTime;
        }
    }
}