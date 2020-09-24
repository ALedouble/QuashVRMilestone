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
    public LevelsScriptable currentLevelFocused;
    public bool isGoingStraightToCampaign = false;
    public bool devMode;

    private string currentGameVersion = "0.9.5";
    private int key = 324;


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
            //Debug.Log("N 'EXISTE PAS");
            if (!Directory.Exists(GetDirectory()))
                Directory.CreateDirectory(GetDirectory());

            SaveDATA(null);
        }

        string savedString = File.ReadAllText(GetFilePathWithSteamID());
        SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

        if (levelsToSave.Count != loadObject.savedObjects.Count)
        {
            Debug.Log("Loading NEW CAMPAIGN CONFIG with " + (levelsToSave.Count - loadObject.savedObjects.Count));
            RefreshDataCount(loadObject);
        }

        LoadDATA();
    }

    private void RefreshDataCount(SavedObject oldSave)
    {
        SavedObject newDATA = new SavedObject() { };
        newDATA.saveGameVersion = oldSave.saveGameVersion;

        //Debug.Log("Evolve at " + newDATA.saveGameVersion);

        string json = "";

        if(levelsToSave.Count > oldSave.savedObjects.Count)
        {
            for (int i = 0; i < oldSave.savedObjects.Count; i++)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = oldSave.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = oldSave.savedObjects[i].done;

                newDATA.savedObjects[i].bestScore = oldSave.savedObjects[i].bestScore;
                newDATA.savedObjects[i].bestCombo = oldSave.savedObjects[i].bestCombo;
                newDATA.savedObjects[i].bestTime = oldSave.savedObjects[i].bestTime;
            }

            for (int i = oldSave.savedObjects.Count; i < levelsToSave.Count; i++)
            {
                newDATA.savedObjects.Add(new SavedValues());
            }
        }
        else
        {
            for (int i = 0; i < levelsToSave.Count; i++)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = oldSave.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = oldSave.savedObjects[i].done;

                newDATA.savedObjects[i].bestScore = oldSave.savedObjects[i].bestScore;
                newDATA.savedObjects[i].bestCombo = oldSave.savedObjects[i].bestCombo;
                newDATA.savedObjects[i].bestTime = oldSave.savedObjects[i].bestTime;
            }
        }
        

        json = JsonUtility.ToJson(newDATA);
        File.WriteAllText(GetFilePathWithSteamID(), json);
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
            //Debug.LogError("NO DATA TO SAVE !");
            return;
        }

        //New Virgin Save DATAs
        SavedObject newDATA = new SavedObject() { };
        newDATA.saveGameVersion = currentGameVersion;

        //DATA presented
        if (levelValues == null)
        {
            //Debug.LogError("NO VALUES TO SAVE");
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
                //Debug.Log("Data is Saved for level : " + levelsToSave[i]);
                newDATA.savedObjects.Add(new SavedValues());
                newDATA.savedObjects[i] = levelValues;
            }
        }


        string json = JsonUtility.ToJson(newDATA);

        //SaveHash(json);

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
            //Debug.Log("RESET DATA");
            presentedDATA = new SavedObject() { };
            presentedDATA.saveGameVersion = currentGameVersion;

            for (int i = 0; i < levelsToSave.Count; i++)
            {
                presentedDATA.savedObjects.Add(new SavedValues());
                //Unlock first level
                if (i == levelsToSave.Count - 1)
                    presentedDATA.savedObjects[i].unlock = true;
                else
                    presentedDATA.savedObjects[i].unlock = false;

                presentedDATA.savedObjects[i].done = false;

                presentedDATA.savedObjects[i].bestScore = 0;
                presentedDATA.savedObjects[i].bestCombo = 0;
                presentedDATA.savedObjects[i].bestTime = 0;
            }
        }

        string json = "";

        //If there's already DATA on file
        if (File.Exists(GetFilePathWithSteamID()))
        {
            string loadString = File.ReadAllText(GetFilePathWithSteamID());
            SavedObject loadDATA = JsonUtility.FromJson<SavedObject>(loadString);
            //JsonUtility.FromJsonOverwrite(loadString, loadDATA);    BETTER FOR NEXT TIME

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

            newDATA.saveGameVersion = loadDATA.saveGameVersion;

            json = JsonUtility.ToJson(newDATA);
        }
        else //NO DATA on file
        {
            json = JsonUtility.ToJson(presentedDATA);
        }

        //SaveHash(json);

        File.WriteAllText(GetFilePathWithSteamID(), json);
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

        string savedString = File.ReadAllText(GetFilePathWithSteamID());
        ////PlayerPrefs.SetString("HASH", "No_Hash_Generated");
        //if (!VerifyHash(savedString))
        //{
        //    Debug.LogError("Invalid Hash, Data has been modified. You're bad... very bad... very very bad");
        //}
        //else
        //{
        //    Debug.LogWarning("Valid hash or no hash yet");
        //    SaveHash(savedString);
        //}

        SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

        CheckGameVersion(loadObject);
    }

    public SavedObject GetData()
    {
        if (!File.Exists(GetFilePathWithSteamID()))
        {
            Debug.Log("NO DATA TO GET");
            return null;
        }

        string savedString = File.ReadAllText(GetFilePathWithSteamID());
        SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

        return loadObject;
    }


    /// <summary>
    /// Generate Hash from save
    /// </summary>
    /// <param name="json"></param>
    private void SaveHash(string json)
    {
        string hashValue = SecureData.Hash(json);
        PlayerPrefs.SetString("HASH", hashValue);
    }

    /// <summary>
    /// Check if the save version is unchanged from last load/save
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    private bool VerifyHash(string json)
    {
        string defaultValue = "No_Hash_Generated";
        string hashValue = SecureData.Hash(json);
        string hashStored = PlayerPrefs.GetString("HASH");

        return hashValue == hashStored || hashStored == defaultValue;
    }


    void ChangeToNextGameVersion(string nextGameVersion)
    {
        switch (nextGameVersion)
        {
            case ("0.9.1"):
                {
                    SavedObject newDATA = new SavedObject() { };
                    newDATA.saveGameVersion = "0.9.2";

                    //Debug.Log("Evolve at " + newDATA.saveGameVersion);

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

                    CheckGameVersion(newDATA);

                    break;
                }

            //Invert levels data
            case ("0.9.2"):
                {
                    SavedObject newDATA = new SavedObject() { };
                    newDATA.saveGameVersion = "0.9.3";

                    //Debug.Log("Evolve at " + newDATA.saveGameVersion);

                    string savedString = File.ReadAllText(GetFilePathWithSteamID());
                    SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

                    string json = "";


                    for (int i = 0; i < levelsToSave.Count; i++)
                    {
                        newDATA.savedObjects.Add(new SavedValues());
                    }


                    for (int z = 0; z < levelsToSave.Count; z++)
                    {
                        //Debug.Log("BEFORE SET     !" + "index : " + z + "     number : " + levelsToSave[z].level.levelProgression.levelNumber + "     combo : " + loadObject.savedObjects[z].bestCombo.ToString());


                        newDATA.savedObjects[z].unlock = loadObject.savedObjects[(levelsToSave.Count - 1) - z].unlock;
                        newDATA.savedObjects[z].done = loadObject.savedObjects[(levelsToSave.Count - 1) - z].done;

                        newDATA.savedObjects[z].bestScore = loadObject.savedObjects[(levelsToSave.Count - 1) - z].bestScore;
                        newDATA.savedObjects[z].bestCombo = loadObject.savedObjects[(levelsToSave.Count - 1) - z].bestCombo;
                        newDATA.savedObjects[z].bestTime = loadObject.savedObjects[(levelsToSave.Count - 1) - z].bestTime;

                        //Debug.Log("FROM NOW ON     !" + "index : " + z + "     number : " + levelsToSave[z].level.levelProgression.levelNumber + "     combo : " + newDATA.savedObjects[z].bestCombo.ToString());

                    }

                    json = JsonUtility.ToJson(newDATA);
                    File.WriteAllText(GetFilePathWithSteamID(), json);

                    CheckGameVersion(newDATA);

                    break;
                }

            //Exchange some levels datas
            case ("0.9.3"):
                {
                    SavedObject newDATA = new SavedObject() { };
                    newDATA.saveGameVersion = "0.9.4";

                    //Debug.Log("Evolve at " + newDATA.saveGameVersion);

                    string savedString = File.ReadAllText(GetFilePathWithSteamID());
                    SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

                    string json = "";


                    for (int i = 0; i < levelsToSave.Count; i++)
                    {
                        newDATA.savedObjects.Add(new SavedValues());
                    }


                    for (int i = 0; i < levelsToSave.Count; i++)
                    {
                        if (i == 11)
                        {
                            int temp = i + 1;

                            //12 <= 13
                            newDATA.savedObjects[i].unlock = loadObject.savedObjects[temp].unlock;
                            newDATA.savedObjects[i].done = loadObject.savedObjects[temp].done;

                            newDATA.savedObjects[i].bestScore = loadObject.savedObjects[temp].bestScore;
                            newDATA.savedObjects[i].bestCombo = loadObject.savedObjects[temp].bestCombo;
                            newDATA.savedObjects[i].bestTime = loadObject.savedObjects[temp].bestTime;

                            //13 <= 12
                            newDATA.savedObjects[temp].unlock = loadObject.savedObjects[i].unlock;
                            newDATA.savedObjects[temp].done = loadObject.savedObjects[i].done;

                            newDATA.savedObjects[temp].bestScore = loadObject.savedObjects[i].bestScore;
                            newDATA.savedObjects[temp].bestCombo = loadObject.savedObjects[i].bestCombo;
                            newDATA.savedObjects[temp].bestTime = loadObject.savedObjects[i].bestTime;

                            //i = 12
                            i = 12;

                            continue;
                        }
                        else if (i == 17)
                        {
                            //18 <= 19
                            newDATA.savedObjects[i].unlock = loadObject.savedObjects[i + 1].unlock;
                            newDATA.savedObjects[i].done = loadObject.savedObjects[i + 1].done;

                            newDATA.savedObjects[i].bestScore = loadObject.savedObjects[i + 1].bestScore;
                            newDATA.savedObjects[i].bestCombo = loadObject.savedObjects[i + 1].bestCombo;
                            newDATA.savedObjects[i].bestTime = loadObject.savedObjects[i + 1].bestTime;

                            //19 <= 20
                            newDATA.savedObjects[i + 1].unlock = loadObject.savedObjects[i + 2].unlock;
                            newDATA.savedObjects[i + 1].done = loadObject.savedObjects[i + 2].done;

                            newDATA.savedObjects[i + 1].bestScore = loadObject.savedObjects[i + 2].bestScore;
                            newDATA.savedObjects[i + 1].bestCombo = loadObject.savedObjects[i + 2].bestCombo;
                            newDATA.savedObjects[i + 1].bestTime = loadObject.savedObjects[i + 2].bestTime;

                            //20 <= 18
                            newDATA.savedObjects[i + 2].unlock = loadObject.savedObjects[i].unlock;
                            newDATA.savedObjects[i + 2].done = loadObject.savedObjects[i].done;

                            newDATA.savedObjects[i + 2].bestScore = loadObject.savedObjects[i].bestScore;
                            newDATA.savedObjects[i + 2].bestCombo = loadObject.savedObjects[i].bestCombo;
                            newDATA.savedObjects[i + 2].bestTime = loadObject.savedObjects[i].bestTime;

                            //i = 19
                            i = 19;
                            continue;
                        }
                        else if (i == 23)
                        {
                            //24 <= 25
                            newDATA.savedObjects[i].unlock = loadObject.savedObjects[i + 1].unlock;
                            newDATA.savedObjects[i].done = loadObject.savedObjects[i + 1].done;

                            newDATA.savedObjects[i].bestScore = loadObject.savedObjects[i + 1].bestScore;
                            newDATA.savedObjects[i].bestCombo = loadObject.savedObjects[i + 1].bestCombo;
                            newDATA.savedObjects[i].bestTime = loadObject.savedObjects[i + 1].bestTime;

                            //25 <= 24
                            newDATA.savedObjects[i + 1].unlock = loadObject.savedObjects[i].unlock;
                            newDATA.savedObjects[i + 1].done = loadObject.savedObjects[i].done;

                            newDATA.savedObjects[i + 1].bestScore = loadObject.savedObjects[i].bestScore;
                            newDATA.savedObjects[i + 1].bestCombo = loadObject.savedObjects[i].bestCombo;
                            newDATA.savedObjects[i + 1].bestTime = loadObject.savedObjects[i].bestTime;

                            //i = 24
                            i = 24;
                            continue;
                        }
                        else
                        {
                            //Debug.Log("i = " + i);
                            newDATA.savedObjects[i].unlock = loadObject.savedObjects[i].unlock;
                            newDATA.savedObjects[i].done = loadObject.savedObjects[i].done;

                            newDATA.savedObjects[i].bestScore = loadObject.savedObjects[i].bestScore;
                            newDATA.savedObjects[i].bestCombo = loadObject.savedObjects[i].bestCombo;
                            newDATA.savedObjects[i].bestTime = loadObject.savedObjects[i].bestTime;
                        }
                    }

                    json = JsonUtility.ToJson(newDATA);
                    File.WriteAllText(GetFilePathWithSteamID(), json);

                    CheckGameVersion(newDATA);

                    break;
                }

            case "":
                {
                    //Debug.Log("FROM NOTHING");
                    SavedObject newDATA = new SavedObject() { };
                    newDATA.saveGameVersion = "0.9.1";



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

                    CheckGameVersion(newDATA);

                    break;
                }

            default:
                {
                    break;
                }
        }
    }

    public void CheckGameVersion(SavedObject saveVersion)
    {
        if (saveVersion.saveGameVersion != currentGameVersion)
        {
            //Debug.Log("Save VERSION OLDLLDLDLLDLDLDLDL = " + saveVersion.saveGameVersion);

            //Changes with new save version
            ChangeToNextGameVersion(saveVersion.saveGameVersion);
        }
        else
        {
            //Debug.Log("GOOD VERSION");

            for (int i = 0; i < levelsToSave.Count; i++)
            {
                //Debug.Log("   AFTER GET    " + "index : " + i + "     number : " + levelsToSave[i].level.levelProgression.levelNumber + "     combo : "+ saveVersion.savedObjects[i].bestCombo.ToString());

                levelsToSave[i].level.levelProgression.isUnlocked = saveVersion.savedObjects[i].unlock;
                levelsToSave[i].level.levelProgression.isDone = saveVersion.savedObjects[i].done;
                levelsToSave[i].level.levelProgression.maxScore = saveVersion.savedObjects[i].bestScore;
                levelsToSave[i].level.levelProgression.maxCombo = saveVersion.savedObjects[i].bestCombo;
                levelsToSave[i].level.levelProgression.minTiming = saveVersion.savedObjects[i].bestTime;
            }
        }
    }
}