using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private AnimationCurve impactCurve;
    private LayerMask layerMask;
    private Vector3 position;

    private int playerID;
    private int ballColorID;

    private float maxRadius;
    private float minRadius;

    private float initialBurstRelativeSize;
    private float mainExplosionDelay;
    private float mainExplosionDuration;

    public void Setup(Vector3 position, int playerID)
    {
        this.position = position;

        this.playerID = playerID;

        impactCurve = ExplosionManager.Instance.impactCurve;
        layerMask = ExplosionManager.Instance.layerMask;

        maxRadius = ExplosionManager.Instance.PlayersExplosionRadius[playerID];
        minRadius = 0.01f;

        initialBurstRelativeSize = ExplosionManager.Instance.initialBurstRelativeSize;
        mainExplosionDelay = ExplosionManager.Instance.mainExplosionDelay;
        mainExplosionDuration = ExplosionManager.Instance.mainExplosionDuration;
    }

    public void StartExplosionLogic()
    {
        //Debug.Log("Start Explosion Logic");
        ballColorID = BallManager.instance.BallColorBehaviour.GetBallColor();
        
        StartExplosionCoroutine();
    }

    private void StartExplosionCoroutine()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        ExplosionSpherecast(initialBurstRelativeSize * maxRadius);

        yield return new WaitForSeconds(mainExplosionDelay);

        float timer = 0f;

        while(timer < mainExplosionDuration)
        {
            if(!GameManager.Instance.IsGamePaused)
            {
                timer += Time.fixedDeltaTime;
                ExplosionSpherecast(minRadius + ((maxRadius - minRadius) * impactCurve.Evaluate(timer / mainExplosionDuration)));
            }

            yield return new WaitForFixedUpdate();
        }

        EndExplosion();
    }

    private void ExplosionSpherecast(float radius)
    {
        RaycastHit[] hitsInfo = Physics.SphereCastAll(position, radius, new Vector3(0,0,1), 0f, layerMask);

        if(hitsInfo.Length != 0)
        {
            BrickInfo[] hitBrickInfo = new BrickInfo[hitsInfo.Length];

            for(int i = 0; i < hitsInfo.Length; i++)
            {
                hitBrickInfo[i] = hitsInfo[i].collider.gameObject.GetComponent<BrickInfo>();
            }

            BrickDestructionManager.Instance.HitBricksByID(hitBrickInfo, playerID, ballColorID);
        }
    }

    private void EndExplosion()
    {
        ExplosionManager.Instance.EndExplosion(gameObject);
        //Disposable?
    }
}
