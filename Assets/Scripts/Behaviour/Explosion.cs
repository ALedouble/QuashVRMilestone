﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private AnimationCurve impactCurve;
    private LayerMask layerMask;
    private Vector3 position;

    private float impactDuration;
    private float impactCurentTime;
    private float impactPercent;
    private float maxRadius;
    private float minRadius;
    
    private int numberOfDivision;
    private float raycastOffset;
    private bool isExploding;

    private bool isOld;

    public Explosion(Vector3 position, int playerID)
    {
        this.position = position;

        impactCurve = ExplosionManager.Instance.impactCurve;
        layerMask = ExplosionManager.Instance.layerMask;

        impactDuration = ExplosionManager.Instance.impactDuration;

        impactCurentTime = 0f;
        impactPercent = 0f;
        maxRadius = ExplosionManager.Instance.PlayersExplosionRadius[playerID];
        minRadius = 0.1f;

        numberOfDivision = ExplosionManager.Instance.numberOfDivision;
        raycastOffset = ExplosionManager.Instance.raycastOffset;
        isExploding = false;

        isOld = true;
    }

    public void StartExplosionLogic()
    {
        if (isOld)
        {
            isExploding = true;
            StartOldExplosionCoroutine();
        }
    }

    public void StopExplosion()
    {
        if (isOld)
            isExploding = false;
    }

    private void StartExplosionCoroutine()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return 0;
    }

    #region Old Explosion

    private void StartOldExplosionCoroutine()
    {
        StartCoroutine(OldExplosionCoroutine());
    }

    private IEnumerator OldExplosionCoroutine()
    {
        impactCurentTime = 0;
        impactPercent = 0;
        
        while ((impactCurentTime < impactDuration) && isExploding)
        {
            if(!GameManager.Instance.IsGamePaused)
            {
                RadialRaycast(position, new Vector2(0, 1), new Vector2(1f / (float)numberOfDivision, -1f / (float)numberOfDivision), raycastOffset);
                RadialRaycast(position, new Vector2(1, 0), new Vector2(-1f / (float)numberOfDivision, -1f / (float)numberOfDivision), raycastOffset);
                RadialRaycast(position, new Vector2(0, -1), new Vector2(-1f / (float)numberOfDivision, 1f / (float)numberOfDivision), raycastOffset);
                RadialRaycast(position, new Vector2(-1, 0), new Vector2(1f / (float)numberOfDivision, 1f / (float)numberOfDivision), raycastOffset);

                impactPercent = minRadius + ((maxRadius - minRadius) * impactCurve.Evaluate(impactCurentTime / impactDuration));

                impactCurentTime += Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }

        EndExplosion();
    }

    void RadialRaycast(Vector3 originPosition, Vector2 destination, Vector2 evolution, float zOffset = 0.0f)
    {
        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPosition, new Vector3((destination.x + evolution.x * j) - originPosition.x, (destination.y + evolution.y * j) - originPosition.y, zOffset).normalized * impactPercent, Color.blue);

            RaycastHit hit;
            BrickInfo brickInfo;

            if (Physics.Raycast(originPosition, new Vector3((destination.x + evolution.x * j) - originPosition.x, (destination.y + evolution.y * j) - originPosition.y, zOffset).normalized,
                out hit, impactPercent, layerMask))
            {
                if (brickInfo = hit.collider.gameObject.GetComponent<BrickInfo>())
                {
                    if (brickInfo.colorID == 0 || brickInfo.colorID == BallManager.instance.GetBallColorID())
                    {
                        BrickManager.Instance.HitBrickByID(brickInfo.BrickID);
                    }
                    else
                    {
                        StartCoroutine(DeactivateBrickCollider(hit.collider, impactDuration));
                    }
                }
            }
        }
    }

    private IEnumerator DeactivateBrickCollider(Collider brickCollider, float duration)
    {
        brickCollider.enabled = false;
        yield return new WaitForSeconds(duration);
        brickCollider.enabled = true;
    }

    #endregion

    private void EndExplosion()
    {
        ExplosionManager.Instance.EndExplosion(this);
        //Disposable?
    }
}