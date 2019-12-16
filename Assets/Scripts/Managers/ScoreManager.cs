using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    [Header("Score")]
    public float[] score;
    public TextMeshProUGUI[] displayedScore;
    public GameObject scoreDisplayedPrefab;
    [Range(-1, -8)] public float scoreWallDistance;

    public static ScoreManager Instance;




    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    public void IncrementScore(int brickValue, int playerID)
    {
        Debug.Log("Incrémentation de " + brickValue + " pour le joueur " + playerID);
        score[playerID] += brickValue;

        string textScore = score[playerID].ToString();
        displayedScore[playerID].text = textScore;
    }
}
