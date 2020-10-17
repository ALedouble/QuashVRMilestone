using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;

    private List<GameObject> currentExplosions;
    public float[] PlayersExplosionRadius { get; set; }

    public LayerMask layerMask;
    public AnimationCurve impactCurve;
    private float minRadius = 0.1f;
    
    [Header("New")]
    public float initialBurstRelativeSize;
    public float mainExplosionDelay;
    public float mainExplosionDuration;

    public float ImpactDuration { get => mainExplosionDelay + mainExplosionDuration; }

    private PhotonView photonView;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        currentExplosions = new List<GameObject>();

        photonView = GetComponent<PhotonView>();
    }

    public void CreateExplosion(Vector3 origin, int playerID)
    {
        SendFeedback(origin, playerID);

        GameObject newExplosion = PoolManager.instance.SpawnFromPool("ExplosionLogic", Vector3.zero, Quaternion.identity);
        currentExplosions.Add(newExplosion);

        Explosion newEwplosionScript = newExplosion.GetComponent<Explosion>();
        newEwplosionScript.Setup(origin, playerID);
        newEwplosionScript.StartExplosionLogic();
    }

    public void EndExplosion(GameObject explosion)
    {
        if (currentExplosions.Contains(explosion))
        {
            currentExplosions.Remove(explosion);
            explosion.SetActive(false);
        }
    }

    private void SendFeedback(Vector3 origin, int playerID)
    {
        if (GameManager.Instance.offlineMode)
            PlayExplosionFeedback(origin, playerID);
        else
            photonView.RPC("PlayExplosionFeedback", RpcTarget.All, origin, playerID);
    }

    [PunRPC]
    private void PlayExplosionFeedback(Vector3 origin, int playerID)
    {
        FXManager.Instance.PlayExplosionFX(origin, playerID);
        AudioManager.instance.PlaySound("Explosion", origin);
    }
}
