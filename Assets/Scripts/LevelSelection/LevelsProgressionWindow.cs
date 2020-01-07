using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class LevelsProgressionWindow : EditorWindow
{
    Texture2D levelText;


    [MenuItem("Window/Custom/Level Selection")]
    public static void OpenProgressionWindow()
    {
        LevelsProgressionWindow window = EditorWindow.GetWindow(typeof(LevelsProgressionWindow)) as LevelsProgressionWindow;

        window.Show();
    }

    void OnGUI()
    {

        ProgressionSettingGUI();

        Repaint();

    }

    private void Update()
    {
        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            GraphicGUI();
        }
    }

    public void InitLevelProgression()
    {

    }

    void GraphicGUI()
    {

    }

    void ProgressionSettingGUI()
    {

    }
}
