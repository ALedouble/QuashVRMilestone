using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickInfo : MonoBehaviour
{
    private int brickID;
    public int BrickID { get => brickID; }
    private int playerID;
    private int PlayerID { get => playerID; }

    public int scoreValue;
    public int armorValue;
    public int colorID;
    public int wallID;
    public bool isBonus;
    public bool isMalus;

    public void SetBrickID(int brickID, int playerID)
    {
        this.brickID = brickID;
        this.playerID = playerID;
    }
}
