using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class DirtyPlaytestManager : MonoBehaviour
{
    public DirtyButton[] buttons;
    public TextMeshProUGUI textMesh;
    public TextMeshPro textMeshController;
    public string good = "GOOD";
    public string bad = "ERROR";
    public int maxRan = 6;

    public string targetWord;

    public string[] words;

    private void Start()
    {
        buttons = GetComponentsInChildren<DirtyButton>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetButtons();
        }
    }

    [Button]
    public void SetButtons()
    {
        int ranNbr = Random.Range(0, maxRan);
        textMesh.text = "";

        for (int i = 0; i < ranNbr; i++)
        {
            CreateButton(i, buttons[i]);
        }
    }

    void CreateButton(int index,DirtyButton dirtyButton)
    {
        if(index == 0)
        {
            targetWord = RandomWord();
            textMeshController.text = targetWord;
            dirtyButton.SetButton(true, Color.red, targetWord);
        }
        else
        {
            dirtyButton.SetButton(false, Color.red, RandomWord());
        }
    }

    string RandomWord()
    {
        return words[Random.Range(0, words.Length)];
    }

    public void GoodAnswer()
    {
        textMesh.text = good;
    }

    public void BadAnswer()
    {
        textMesh.text = bad;
    }
}
