using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;

    private List<Explosion> currentExplosions;
    public float[] PlayersExplosionRadius { get; set; }

    [Header("Deflagration Animation")]
    public AnimationCurve impactCurve;
    public float impactDuration;
    private float minRadius = 0.1f;

    [Header("Deflagration Parameters")]
    public LayerMask layerMask;
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
    }

    public void CreateExplosion(Vector3 origin, int playerID)
    {
        FXManager.Instance.PlayExplosionFX(origin, playerID);
        AudioManager.instance.PlaySound("Explosion", Vector3.zero);

        Explosion newExplosion = new Explosion(origin, playerID);
        currentExplosions.Add(newExplosion);
        newExplosion.StartExplosionLogic();
    }

    public void EndExplosion(Explosion explosion)
    {
        if (currentExplosions.Contains(explosion))
            currentExplosions.Remove(explosion);
    }
}
