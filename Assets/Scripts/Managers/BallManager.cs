using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallManager : MonoBehaviour
{
    #region Singleton
    public static BallManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject ball;

    private BallColorBehaviour ballColorBehaviour;
    private BallPhysicBehaviour ballPhysicBehaviour;
    private PhysicInfo ballPhysicInfo;

    private void Start()
    {
        ballColorBehaviour = ball.GetComponent<BallColorBehaviour>();
        ballPhysicBehaviour = ball.GetComponent<BallPhysicBehaviour>();
        ballPhysicInfo = ball.GetComponent<PhysicInfo>();
    }


    #region Color
    public BallColorBehaviour GetBallColorBehaviour()
    {
        return ballColorBehaviour;
    }

    public int GetBallColorID()
    {
        return ballColorBehaviour.GetBallColor();
    }

    public Material GetCurrentMaterial()
    {
        return ballColorBehaviour.GetCurrentMaterial();
    }

    public Material[] GetBallMaterials()
    {
        return ballColorBehaviour.GetBallMaterials();
    }

    #endregion


    #region Physics

    public BallPhysicBehaviour GetBallPhysicsBehaviour()
    {
        return GetBallPhysicsBehaviour();
    }

    public PhysicInfo GetBallPhysicInfo()
    {
        return ballPhysicInfo;
    }

    #endregion

    #region Utility

    public QPlayer GetLastPlayerWhoHitTheBall()
    {
        return ballPhysicBehaviour.GetLastPlayerWhoHitTheBall();
    }

    public void TransferEmpowerement()
    {
        if (ballColorBehaviour.colorSwitchTrigerType == ColorSwitchTrigerType.WALLBASED)
            ballColorBehaviour.TransferEmpowerement();
    }

    #endregion

}
