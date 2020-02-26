using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;




public class JSON : MonoBehaviour
{
    public static JSON instance;

    List<LevelsScriptable> levelsToSave;


    [System.Serializable]
    public class SavedObject
    {
        public List<SavedValues> savedObjects;

        public SavedObject()
        {
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

        if (!File.Exists(Application.persistentDataPath + "/SavedByTheQuash"))
        {
            //Get Steam DATAs

            //If no DATA then
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
        SavedValues levelValue = new SavedValues { unlock = true, done = true };


        for (int i = 0; i < level.level.levelProgression.numberOfAdditionalConditions; i++)
        {
            if (level.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
            {
                switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                {
                    case CompleteConditionType.Score:
                        if (score > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (score > level.level.levelProgression.maxScore)
                                levelValue.bestScore = score;
                            else
                                levelValue.bestScore = level.level.levelProgression.maxScore;
                        }
                        else
                            levelValue.bestScore = level.level.levelProgression.maxScore;

                        break;

                    case CompleteConditionType.Combo:
                        if (combo > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (combo > level.level.levelProgression.maxCombo)
                                levelValue.bestCombo = combo;
                            else
                                levelValue.bestCombo = level.level.levelProgression.maxCombo;
                        }
                        else
                            levelValue.bestCombo = level.level.levelProgression.maxCombo;

                        break;

                    case CompleteConditionType.Timing:
                        if (time < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (time < level.level.levelProgression.minTiming)
                                levelValue.bestTime = time;
                            else
                                levelValue.bestTime = level.level.levelProgression.minTiming;
                        }
                        else
                            levelValue.bestTime = level.level.levelProgression.minTiming;

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
                        else
                            levelValue.bestScore = level.level.levelProgression.maxScore;

                        break;

                    case CompleteConditionType.Combo:
                        if (combo < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (combo > level.level.levelProgression.maxCombo)
                                levelValue.bestCombo = combo;
                            else
                                levelValue.bestCombo = level.level.levelProgression.maxCombo;
                        }
                        else
                            levelValue.bestCombo = level.level.levelProgression.maxCombo;

                        break;

                    case CompleteConditionType.Timing:
                        if (time > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                        {
                            if (time < level.level.levelProgression.minTiming)
                                levelValue.bestTime = time;
                            else
                                levelValue.bestTime = level.level.levelProgression.minTiming;
                        }
                        else
                            levelValue.bestTime = level.level.levelProgression.minTiming;

                        break;
                }
            }
        }

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


        //DATA presented
        if (levelValues == null)
        {
            Debug.LogError("NO VALUES TO SAVE");
            return;
        }

        string loadString = File.ReadAllText(Application.persistentDataPath + "/SavedByTheQuash");
        SavedObject loadDATA = JsonUtility.FromJson<SavedObject>(loadString);

        for (int i = 0; i < levelsToSave.Count; i++)
        {
            if (i != levelIndex)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = loadDATA.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = loadDATA.savedObjects[i].unlock;

                //Check BEST score
                if (loadDATA.savedObjects[i].bestScore > loadDATA.savedObjects[i].bestScore)
                    newDATA.savedObjects[i].bestScore = loadDATA.savedObjects[i].bestScore;
                else
                    newDATA.savedObjects[i].bestScore = loadDATA.savedObjects[i].bestScore;

                //Check BEST Combo
                if (loadDATA.savedObjects[i].bestCombo > loadDATA.savedObjects[i].bestCombo)
                    newDATA.savedObjects[i].bestCombo = loadDATA.savedObjects[i].bestCombo;
                else
                    newDATA.savedObjects[i].bestCombo = loadDATA.savedObjects[i].bestCombo;

                //Check BEST Time
                if (loadDATA.savedObjects[i].bestTime < loadDATA.savedObjects[i].bestTime)
                    newDATA.savedObjects[i].bestTime = loadDATA.savedObjects[i].bestTime;
                else
                    newDATA.savedObjects[i].bestTime = loadDATA.savedObjects[i].bestTime;
            }
            else
            {
                newDATA.savedObjects[i] = levelValues;
            }
        }


        string json = JsonUtility.ToJson(newDATA);

        File.WriteAllText(Application.persistentDataPath + "/SavedByTheQuash", json);
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


        //DATA presented
        if (presentedDATA == null)
        {
            presentedDATA = new SavedObject() { };

            for (int i = 0; i < levelsToSave.Count; i++)
            {
                presentedDATA.savedObjects.Add(new SavedValues());

                presentedDATA.savedObjects[i].unlock = levelsToSave[i].level.levelProgression.isUnlocked;
                presentedDATA.savedObjects[i].done = levelsToSave[i].level.levelProgression.isDone;

                presentedDATA.savedObjects[i].bestScore = levelsToSave[i].level.levelProgression.maxScore;
                presentedDATA.savedObjects[i].bestCombo = levelsToSave[i].level.levelProgression.maxCombo;
                presentedDATA.savedObjects[i].bestTime = levelsToSave[i].level.levelProgression.minTiming;
            }
        }


        //If there's already DATA on file
        if (File.Exists(Application.persistentDataPath + "/SavedByTheQuash"))
        {
            string loadString = File.ReadAllText(Application.persistentDataPath + "/SavedByTheQuash");
            SavedObject loadDATA = JsonUtility.FromJson<SavedObject>(loadString);

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
        }
        else //NO DATA on file
        {
            for (int i = 0; i < levelsToSave.Count; i++)
            {
                newDATA.savedObjects.Add(new SavedValues());

                newDATA.savedObjects[i].unlock = presentedDATA.savedObjects[i].unlock;
                newDATA.savedObjects[i].done = presentedDATA.savedObjects[i].unlock;

                //Check BEST score
                if (presentedDATA.savedObjects[i].bestScore > newDATA.savedObjects[i].bestScore)
                    newDATA.savedObjects[i].bestScore = presentedDATA.savedObjects[i].bestScore;

                //Check BEST Combo
                if (presentedDATA.savedObjects[i].bestCombo > newDATA.savedObjects[i].bestCombo)
                    newDATA.savedObjects[i].bestCombo = presentedDATA.savedObjects[i].bestCombo;

                //Check BEST Time
                if (presentedDATA.savedObjects[i].bestTime < newDATA.savedObjects[i].bestTime)
                    newDATA.savedObjects[i].bestTime = presentedDATA.savedObjects[i].bestTime;
            }
        }

        string json = JsonUtility.ToJson(newDATA);

        File.WriteAllText(Application.persistentDataPath + "/SavedByTheQuash", json);
    }

    /// <summary>
    /// Back up on Steam
    /// </summary>
    public void SaveDATAonSteam()
    {
        string savedString = File.ReadAllText(Application.persistentDataPath + "/SavedByTheQuash");
        SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

        //TO DO
    }

    /// <summary>
    /// Get Steam DATA et Set the GAME ones
    /// </summary>
    public void LoadDATAfromSteam()
    {
        //TO DO
    }

    /// <summary>
    /// LOADING DATAs
    /// </summary>
    public void LoadDATA()
    {
        if (!File.Exists(Application.persistentDataPath + "/SavedByTheQuash"))
            return;

        string savedString = File.ReadAllText(Application.persistentDataPath + "/SavedByTheQuash");
        SavedObject loadObject = JsonUtility.FromJson<SavedObject>(savedString);

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