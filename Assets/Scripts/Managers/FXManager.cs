using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FXManager : MonoBehaviour
{
    [Header("Deflagration Animation")]
    public AnimationCurve impactCurve;
    public float impactMaxTime;
    private float impactCurentTime;
    private float impactPercent;
    private float minRadius = 0.1f;

    [Header("Deflagration zone")]
    private GameObject impactGo;
    private List<ParticleSystem> ps;
    [HideInInspector] public float[] playersRadius;
    private float maxRadius;

    //public SphereCollider sphereCol;


    [Header("Deflagration Parameters")]
    private Vector3 originPos;
    public LayerMask layerMask;
    public int numberOfDivision;
    public float raycastOffset = 0;
    public float intensityModifier;
    bool isExplosion;



    public static FXManager Instance;
    private PhotonView photonView;                                  // A enlever


    private void Awake()
    {
        Instance = this;
        ps = new List<ParticleSystem>();
        photonView = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (isExplosion)
        {
            if (impactCurentTime >= impactMaxTime)
            {
                impactCurentTime = 0;
                impactPercent = 0;
                isExplosion = false;

                impactGo.SetActive(false);
                return;
            }
            else
            {
                impactCurentTime += Time.deltaTime;

                RadialRaycast(originPos, new Vector2(0, 1), new Vector2(1f / (float)numberOfDivision, -1f / (float)numberOfDivision), raycastOffset);
                RadialRaycast(originPos, new Vector2(1, 0), new Vector2(-1f / (float)numberOfDivision, -1f / (float)numberOfDivision), raycastOffset);
                RadialRaycast(originPos, new Vector2(0, -1), new Vector2(-1f / (float)numberOfDivision, 1f / (float)numberOfDivision), raycastOffset);
                RadialRaycast(originPos, new Vector2(-1, 0), new Vector2(1f / (float)numberOfDivision, 1f / (float)numberOfDivision), raycastOffset);
            }

            impactPercent = minRadius + ((maxRadius - minRadius) * impactCurve.Evaluate(impactCurentTime));
        }


        #region MyRaycast old
        //RaycastHit hitback;

        //Debug.DrawRay(originPos,
        //        transform.TransformDirection(transform.forward).normalized * 0.1f, Color.green);

        //if (Physics.Raycast(originPos, transform.TransformDirection(transform.forward).normalized, out hitback, 0.1f, layerMask))
        //{
        //    Debug.Log("HitSomething");

        //    BrickManager.Instance.DeadBrick(hitback.collider.gameObject, 1);
        //}




        //for (int j = 0; j < numberOfDivision; j++)
        //{
        //    Debug.DrawRay(originPos,
        //        transform.TransformDirection(new Vector3(0f + (1f / (float)numberOfDivision) * j, 1f - (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

        //    RaycastHit hit;

        //    if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(0f + (1f / (float)numberOfDivision) * j, 1f - (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
        //    {
        //        BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
        //    }
        //}



        //for (int j = 0; j < numberOfDivision; j++)
        //{
        //    Debug.DrawRay(originPos,
        //        transform.TransformDirection(new Vector3(1f - (1f / (float)numberOfDivision) * j, 0f - (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

        //    RaycastHit hit;

        //    if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(1f - (1f / (float)numberOfDivision) * j, 0f - (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
        //    {
        //        BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
        //    }
        //}

        //for (int j = 0; j < numberOfDivision; j++)
        //{
        //    Debug.DrawRay(originPos,
        //        transform.TransformDirection(new Vector3(0f - (1f / (float)numberOfDivision) * j, -1f + (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

        //    RaycastHit hit;

        //    if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(0f - (1f / (float)numberOfDivision) * j, -1f + (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
        //    {
        //        BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
        //    }
        //}

        //for (int j = 0; j < numberOfDivision; j++)
        //{
        //    Debug.DrawRay(originPos,
        //        transform.TransformDirection(new Vector3(-1f + (1f / (float)numberOfDivision) * j, 0f + (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

        //    RaycastHit hit;

        //    if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(-1f + (1f / (float)numberOfDivision) * j, 0f + (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
        //    {
        //        BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
        //    }
        //}
        #endregion
    }

    public void PlayWallBounceFX(Vector3 impactPosition, Vector3 normal)
    {
        //Vector3 forward = Vector3.Cross(collision.contacts[0].normal, collision.transform.up);
        PoolManager.instance.SpawnFromPool("BounceFX", impactPosition, Quaternion.LookRotation(normal, Vector3.up));
    }

    public float SetFXscale(float impulse)
    {
        float scale = impulse * intensityModifier;
        return scale;
    }

    public void PlayExplosion(Vector3 origin, int playerID)
    {
        //if(GameManager.Instance.offlineMode)
        //{
        //    PlayExplosionFX(origin, playerID);
        //}
        //else if(PhotonNetwork.IsMasterClient)
        //{
        //    photonView.RPC("PlayExplosionFX", RpcTarget.All, origin, playerID);
        //}

        // Beurk! Caca! A refaire!

        PlayExplosionFX(origin, playerID);
        isExplosion = true;
    }

    private void PlayExplosionFX(Vector3 origin, int playerID)
    {
        if(GameManager.Instance.offlineMode)
        {
            PlayExplosionFXRPC(origin, playerID);
        }
        else
        {
            photonView.RPC("PlayExplosionFXRPC", RpcTarget.All, origin, playerID);
        }
    }

    [PunRPC]
    private void PlayExplosionFXRPC(Vector3 origin, int playerID)
    {
        originPos = origin;
        Vector3 fxPos = new Vector3(origin.x, origin.y, -0.5f);
        //maxRadius = intensity * intensityModifier;
        maxRadius = playersRadius[playerID];


        switch (BallManager.instance.GetBallColorID())
        {
            case 0:
                impactGo = PoolManager.instance.SpawnFromPool("ImpactColor01", fxPos, Quaternion.identity);

                break;

            case 1:
                impactGo = PoolManager.instance.SpawnFromPool("ImpactColor02", fxPos, Quaternion.identity);

                break;
        }


        impactGo.transform.localScale = new Vector3(maxRadius, maxRadius, maxRadius);

        ps.Clear();

        if (impactGo.transform.childCount > 0)
        {
            for (int i = 0; i < impactGo.transform.childCount; i++)
            {
                ps.Add(impactGo.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>());
            }

            for (int i = 0; i < ps.Count; i++)
            {
                ps[i].transform.localScale = new Vector3(maxRadius, maxRadius, maxRadius);

                ps[i].Play();
            }
        }

        AudioManager.instance.PlaySound("Explosion", Vector3.zero);
    }

    void RadialRaycast(Vector3 originPosition, Vector2 destination, Vector2 evolution, float zOffset = 0.0f)
    {
        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPosition,
                transform.TransformDirection(new Vector3(destination.x + evolution.x * j, destination.y + evolution.y * j, zOffset)).normalized * impactPercent, Color.blue);

            RaycastHit hit;
            BrickInfo brickInfo;

            if (Physics.Raycast(originPosition, transform.TransformDirection(new Vector3(destination.x + evolution.x * j, destination.y + evolution.y * j, zOffset)).normalized,
                out hit, impactPercent, layerMask))
            {
                if (brickInfo = hit.collider.gameObject.GetComponent<BrickInfo>())
                {
                    if (brickInfo.colorID == 0 || brickInfo.colorID == BallManager.instance.GetBallColorID() + 1)
                    {
                        //hit.collider.gameObject.GetComponent<BrickBehaviours>().HitBrick();
                        BrickManager.Instance.HitBrickByID(hit.collider.gameObject.GetComponent<BrickBehaviours>().BrickID);
                    }
                    else
                    {
                        StartCoroutine(DeactivateBrickCollider(hit.collider, impactMaxTime));
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
}


