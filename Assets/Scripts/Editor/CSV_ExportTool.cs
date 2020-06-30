using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public static class CSV_ExportTool
{
    private static DefaultAsset[] saveFiles;
    private static SavedObject[] saveDatas;
    private static string loadDirectoryPath = "Assets/Editor/Saves_To_Compare";

    private static string saveDirectoryName = "CSV";
    private static string saveFileName = "comparingQuashValues.csv";
    private static string separator = ";";

    private static string[] reportHeaders = new string[5]
    {
        "Joueur",
        "Level",
        "Score",
        "Combo",
        "Best Time"
    };


    [MenuItem("Tools/Export Save File in CSV %F1")]
    static void CSV()
    {
        GetAllSaveFiles();
    }


    static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            CreateReport();
        }
        else
        {
            File.Delete(file);
            CreateReport();
        }
    }

    static string GetDirectoryPath()
    {
        return Application.dataPath + "/" + saveDirectoryName;
    }

    static string GetFilePath()
    {
        return GetDirectoryPath() + "/" + saveFileName;
    }

    static void GetAllSaveFiles()
    {
        saveDatas = new SavedObject[0];
        saveFiles = new DefaultAsset[0];

        VerifyDirectory();
        VerifyFile();

        if (AssetDatabase.IsValidFolder(loadDirectoryPath))
        {
            string[] filesPaths = AssetDatabase.FindAssets("t:DefaultAsset", new[] { loadDirectoryPath });
            saveDatas = new SavedObject[filesPaths.Length];
            saveFiles = new DefaultAsset[filesPaths.Length];
            LoadEveryDatas(filesPaths);
        }
        else
        {
            saveDatas = new SavedObject[0];
            AssetDatabase.CreateFolder("Assets/Editor", "Saves_To_Compare");
            Debug.LogError("NO folder THUS NO save files");
        }
    }

    static void LoadEveryDatas(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            string savedString = File.ReadAllText(AssetDatabase.GUIDToAssetPath(paths[i]));

            DefaultAsset loadObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(DefaultAsset)) as DefaultAsset;
            saveFiles[i] = loadObject;

            SavedObject loadData = JsonUtility.FromJson<SavedObject>(savedString);
            saveDatas[i] = loadData;
        }

        for (int i = 0; i < saveDatas.Length; i++)
        {
            for (int y = saveDatas[i].savedObjects.Count - 1; y >= 0 ; y--)
            {
                int levelNumber = saveDatas[i].savedObjects.Count - y;

                float minutes = (int)saveDatas[i].savedObjects[y].bestTime / 60;
                float seconds = (int)saveDatas[i].savedObjects[y].bestTime - (minutes * 60);

                string bestTimeDisplay = "";

                if (minutes < 10)
                {
                    if (seconds < 10)
                    {
                        bestTimeDisplay = "0" + minutes + ":" + "0" + seconds;
                    }
                    else
                    {
                        bestTimeDisplay = "0" + minutes + ":" + seconds;
                    }
                }
                else
                {
                    if (seconds < 10)
                    {
                        bestTimeDisplay = minutes + ":" + "0" + seconds;
                    }
                    else
                    {
                        bestTimeDisplay = minutes + ":" + seconds;
                    }
                }

                string[] line = new string[5]
                {
                saveFiles[i].name,
                "Level_" + levelNumber.ToString(),
                saveDatas[i].savedObjects[y].bestScore.ToString(),
                saveDatas[i].savedObjects[y].bestCombo.ToString(),
                saveDatas[i].savedObjects[y].bestTime.ToString() + " <=> " + bestTimeDisplay
                };

                AppendToReport(line);
            }
        }
    }



    static void CreateReport()
    {
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += separator;
                }
                finalString += reportHeaders[i];
            }
            sw.WriteLine(finalString);
        }
    }

    static void AppendToReport(string[] strings)
    {
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += separator;
                }
                finalString += strings[i];
            }
            sw.WriteLine(finalString);
        }
    }
}
