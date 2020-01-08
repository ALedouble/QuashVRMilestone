using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<GameObject> ps;
    [SerializeField] private float maxRadius;
    //public SphereCollider sphereCol;


    [Header("Deflagration Parameters")]
    private Vector3 originPos;
    public LayerMask layerMask;
    public int numberOfDivision;
    public float raycastOffset = 0;
    public float intensityModifier;
    bool isExplosion;



    public static FXManager Instance;



    private void Awake()
    {
        Instance = this;
        ps = new List<GameObject>();
    }


    public float SetFXscale(float impulse)
    {
        float scale = impulse * intensityModifier;
        return scale;
    }

    public void SetExplosion(Vector3 origin, float intensity)
    {
        

        originPos = origin;
        //maxRadius = intensity * intensityModifier;
        maxRadius = 0.7f;

        impactGo = PoolManager.instance.SpawnFromPool("ImpactFX", originPos, Quaternion.identity);

        ps.Clear();

        if (impactGo.transform.childCount > 0)
        {
            for (int i = 0; i < impactGo.transform.childCount; i++)
            {
                ps.Add(impactGo.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < ps.Count; i++)
            {
                ps[i].transform.localScale = new Vector3(maxRadius, maxRadius, maxRadius);
            }

            for (int i = 0; i < ps.Count; i++)
            {
                ps[i].GetComponent<ParticleSystem>().Play();
            }
        }

        isExplosion = true;
    }

    private void Update()
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

    void RadialRaycast(Vector3 originPosition, Vector2 destination, Vector2 evolution, float zOffset = 0.0f)
    {
        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPosition,
                transform.TransformDirection(new Vector3(destination.x + evolution.x * j, destination.y + evolution.y * j, zOffset)).normalized * impactPercent, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(originPosition, transform.TransformDirection(new Vector3(destination.x + evolution.x * j, destination.y + evolution.y * j, zOffset)).normalized,
                out hit, impactPercent, layerMask))
            {
                if (hit.collider.TryGetComponent<IBrick>(out IBrick brick))
                {
                    if (brick.GetBrickInfo().ColorID != 0)
                    {
                        if (brick.GetBrickInfo().ColorID == BallManager.instance.GetBallColorID() + 1)
                        {
                            hit.collider.gameObject.GetComponent<BrickBehaviours>().HitBrick();
                        }
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<BrickBehaviours>().HitBrick();
                    }
                }
            }
        }
    }
}
