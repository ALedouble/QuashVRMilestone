using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FXManager : MonoBehaviour
{

    public static FXManager Instance;

    private GameObject impactGo;
    private List<ParticleSystem> ps;


    private void Awake()
    {
        Instance = this;
        ps = new List<ParticleSystem>();
    }

    public void PlayWallBounceFX(Vector3 impactPosition, Vector3 normal)
    {
        PoolManager.instance.SpawnFromPool("BounceFX", impactPosition, Quaternion.LookRotation(normal, Vector3.up));
    }

    public void PlayExplosionFX(Vector3 origin, int playerID)
    {
        Vector3 fxPos = new Vector3(origin.x, origin.y, -0.5f);

        float maxRadius = ExplosionManager.Instance.PlayersExplosionRadius[playerID];


        switch (BallManager.instance.GetBallColorID())
        {
            case 0:
                impactGo = PoolManager.instance.SpawnFromPool("ImpactColor00", fxPos, Quaternion.identity);
                break;
            case 1:
                impactGo = PoolManager.instance.SpawnFromPool("ImpactColor01", fxPos, Quaternion.identity);
                break;
            case 2:
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
    }
}


