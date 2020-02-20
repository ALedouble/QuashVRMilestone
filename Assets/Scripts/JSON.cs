using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSON : MonoBehaviour
{
    public static JSON jsonSaveAndLoad;

    string folderPath = "";
    string filePath = "";
    List<LevelsScriptable> levelsToSave;

    bool hasBeenCreated = false;




    void Awake()
    {
        if (jsonSaveAndLoad == null)
        {
            jsonSaveAndLoad = this;
        }
        else if (jsonSaveAndLoad != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Create()
    {
        if (!hasBeenCreated)
        {

        }
    }

    public void Save(LevelsScriptable dataToSave)
    {

    }

    public void Load(LevelsScriptable dataToLoad)
    {

    }
}