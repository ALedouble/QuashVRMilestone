using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Old")]
    public float impactDuration;
    public int numberOfDivision;
    public float raycastOffset = 0;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        currentExplosions = new List<GameObject>();
    }

    public void CreateExplosion(Vector3 origin, int playerID)
    {
        FXManager.Instance.PlayExplosionFX(origin, playerID);
        AudioManager.instance.PlaySound("Explosion", Vector3.zero);

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
}
