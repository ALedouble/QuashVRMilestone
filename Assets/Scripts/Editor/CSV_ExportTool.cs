using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using Photon.Pun.Demo.SlotRacer.Utils;

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
        "",
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

        if (AssetDatabase.IsValidFolder(loadDirectoryPath))
        {
            string[] filesPaths = AssetDatabase.FindAssets("t:DefaultAsset", new[] { loadDirectoryPath });
            saveDatas = new SavedObject[filesPaths.Length];
            saveFiles = new DefaultAsset[filesPaths.Length];

            for (int i = 0; i < filesPaths.Length; i++)
            {
                string savedString = File.ReadAllText(AssetDatabase.GUIDToAssetPath(filesPaths[i]));

                DefaultAsset loadObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(filesPaths[i]), typeof(DefaultAsset)) as DefaultAsset;
                saveFiles[i] = loadObject;

                SavedObject loadData = JsonUtility.FromJson<SavedObject>(savedString);
                saveDatas[i] = loadData;
            }

            VerifyDirectory();
            VerifyFile();

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
        for (int i = 0; i < saveDatas.Length; i++)
        {
            List<string> line = new List<string>();
            line.Add(saveFiles[i].name);

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

                line.Add(saveDatas[i].savedObjects[y].bestScore.ToString());
                line.Add(saveDatas[i].savedObjects[y].bestCombo.ToString());
                line.Add(saveDatas[i].savedObjects[y].bestTime.ToString() + " == " + bestTimeDisplay);
            }
                AppendToReport(line);
        }
    }



    static void CreateReport()
    {
        if (saveDatas.Length == 0)
            return;

        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = saveDatas[0].savedObjects.Count - 1; i >= 0; i--)
            {
                int levelNumber = saveDatas[0].savedObjects.Count - i;

                if (finalString != "")
                {
                    finalString += separator;
                }
                finalString += separator;
                finalString += "Level_" + levelNumber.ToString();
                finalString += separator;
            }
            sw.WriteLine(finalString);

            finalString = separator;
            for (int i = saveDatas[0].savedObjects.Count - 1; i >= 0; i--)
            {
                finalString += "Score" + separator;
                finalString += "Combo" + separator;
                finalString += "Time" + separator;
            }
            sw.WriteLine(finalString);
        }
    }

    static void AppendToReport(List<string> strings)
    {
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < strings.Count; i++)
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
