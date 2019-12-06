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
        textMesh.text = "";

        List<DirtyButton> dirties = new List<DirtyButton>();

        //Create button list
        for (int i = 0; i < buttons.Length; i++)
        {
            dirties.Add(buttons[i]);
            buttons[i].gameObject.SetActive(false);
        }

        Debug.Log("DIRTIES = " + dirties.Count);

        //Select Random number
        for (int i = 0; i < maxRan; i++)
        {
            int ran = Random.Range(0, dirties.Count);
            Debug.Log("ran =" + ran);
            CreateButton(i, dirties[ran]);
            dirties.Remove(dirties[ran]);
        }
    }

    void CreateButton(int index,DirtyButton dirtyButton)
    {
        dirtyButton.gameObject.SetActive(true);
        if (index == 0)
        {
            targetWord = RandomWord();
            textMeshController.text = targetWord;
            dirtyButton.SetButton(true, Color.red, targetWord);
        }
        else
        {
            string word = RandomWord();
            dirtyButton.SetButton(word == targetWord, Color.red, word);
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
