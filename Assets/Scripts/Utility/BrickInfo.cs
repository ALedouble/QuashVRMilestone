using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickInfo : MonoBehaviour
{
    public static int brickCount = 0;
    private int brickID;
    public int BrickID { get => brickID; }

    public int scoreValue;
    public int armorValue;
    public int colorID;
    public int wallID;
    public bool isBonus;
    public bool isMalus;

    public void SetBrickID()
    {
        brickID = brickCount++;
    }

    public static void ResetBrickCount()
    {
        brickCount = 0;
    }

    //private int _scoreValue;
    //private int _armorValue;
    //private int _colorID;
    //private int _wallID;
    //private bool _isBonus;
    //private bool _isMalus;


    //private void Start()
    //{
    //    _scoreValue = scoreValue;
    //    _armorValue = armorValue;
    //    _colorID = colorID;
    //    _wallID = wallID;
    //    _isBonus = isBonus;
    //    _isMalus = isMalus;
    //}


    //Transform transform;

    //public BrickInfo(int p_score, int p_armor, int p_colorID, int p_wallID, bool p_isBonus, bool p_isMalus, Transform p_transform)
    //{
    //    ScoreValue = p_score;
    //    ArmorValue = p_armor;
    //    ColorID = p_colorID;
    //    WallID = p_wallID;
    //    IsBonus = p_isBonus;
    //    IsMalus = p_isMalus;
    //    Transform = p_transform;
    //}

    //public int ScoreValue { get => _scoreValue; private set => _scoreValue = value; }
    //public int ArmorValue { get => _armorValue; private set => _armorValue = value; }
    //public int ColorID { get => _colorID; private set => _colorID = value; }
    //public int WallID { get => _wallID; private set => _wallID = value; }
    //public bool IsBonus { get => _isBonus; private set => _isBonus = value; }
    //public bool IsMalus { get => _isMalus; private set => _isMalus = value; }
    //public Transform Transform { get => transform; private set => transform = value; }
}
