using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRebounceWallInteraction : MonoBehaviour
{
    public float depthVelocity;
    public float xAcceleration;

    private ITargetSelector targetSelector;
    private NoBounceMagicReturn nBMagicReturn;

    private BallPhysicBehaviour ballPhysicBehaviour;
    private BallInfo ballInfo;

    private Coroutine IgnoreCollisionCoroutine;


    private void Awake()
    {
        ballPhysicBehaviour = GetComponent<BallPhysicBehaviour>();
        ballInfo = GetComponent<BallInfo>();

        nBMagicReturn = new NoBounceMagicReturn(depthVelocity, ballPhysicBehaviour.baseGravity, xAcceleration);
        targetSelector = GetComponent<ITargetSelector>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!BallManager.instance.IsBallPaused && other.gameObject.tag == "RebounceWall")
        {
            ReturnInteration();
            IgnoreCollisionCoroutine = StartCoroutine(IgnoreCollision());
            // Sound Magnitude TO BE FIX !!!
            AudioManager.instance.PlaySound("Bouncing_Back", other.GetContact(0).point, RacketManager.instance.LocalRacketPhysicInfo.GetVelocity().magnitude);

            BallEventManager.instance.OnBallCollision("RebounceWall", other);
        }
    }

    #region ReturnInteraction

    private void ReturnInteration()
    {
        RandomReturnWithoutBounce();
    }

    private void RandomReturnWithoutBounce()
    {
        Vector3 targetPosition = targetSelector.GetNewTargetPosition();
        Vector3 newVelocity = nBMagicReturn.CalculateNewVelocity(transform.position, targetPosition);

        ballPhysicBehaviour.OverrideRawVelocity(newVelocity, (int)SpeedState.SLOW, true);
    }

    private IEnumerator IgnoreCollision()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), true);

        float timer = 0f;
        while (timer < 0.1f)
        {
            yield return new WaitForFixedUpdate();
            if (!BallManager.instance.IsBallPaused)
            {
                timer += Time.fixedDeltaTime;
            }
        }

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Floor"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ball"), LayerMask.NameToLayer("Wall"), false);
    }

    #endregion
}
