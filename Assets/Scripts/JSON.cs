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

        //SetUpDATAs();
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


    public void SubmitDATA()
    {

    }

    public void SaveLevelDATA(SavedValues levelValues)
    {

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
            presentedDATA = new SavedObject(){};

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