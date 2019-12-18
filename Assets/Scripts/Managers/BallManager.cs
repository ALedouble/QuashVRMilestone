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

    public BallColorBehaviour GetBallColorBehaviour()
    {
        return ballColorBehaviour;
    }
     public int GetBallColorID()
    {
        return ballColorBehaviour.GetBallColor();
    }

    public BallPhysicBehaviour GetBallPhysicsBehaviour()
    {
        return GetBallPhysicsBehaviour();
    }

    public PhysicInfo GetBallPhysicInfo()
    {
        return ballPhysicInfo;
    }
}
