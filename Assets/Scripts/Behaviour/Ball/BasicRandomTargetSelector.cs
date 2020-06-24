using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRandomTargetSelector : MonoBehaviour, ITargetSelector
{
    public float minRange;
    public float maxRange;
    public float angleSpread;
    public Vector3 offset = Vector3.up;

    private QPlayer currentTarget;

    public GameObject targetTestIndicator;
    public Vector3 CurrentTargetPlayerPosition {get {return GameManager.Instance.PlayerSpawn[(int)currentTarget % GameManager.Instance.PlayerSpawn.Length].position;}}           // Pour eviter les plantages... Le mieux Serait de verifier playerPosition.Length == PlayerID.Count - 1

    private Vector3 CenterOffset { get; set; }
    private float MinRadius { get; set; }
    private float MaxRadius { get; set; }

    private void Awake()
    {
        CenterOffset = offset;
        //CenterOffset = new Vector3(0, PlayerSettings.Instance.PlayerShoulderHeight, 0); //Add left/right handed offset
        MinRadius = minRange;
        MaxRadius = maxRange;
    }

    public void SwitchTarget()
    {
        currentTarget = (QPlayer)( ((int)currentTarget + 1) % LevelManager.instance.numberOfPlayers);
    }

    public Vector3 GetTargetPlayerPosition()
    {
        return CurrentTargetPlayerPosition;
    }

    public Vector3 GetNewTargetPosition()
    {
        Vector3 newTarget = CurrentTargetPlayerPosition + GetRandomRelativeTargetPoint() + CenterOffset;

        return newTarget;
    }

    public void SetCurrentTarget(QPlayer newTarget)
    {
        currentTarget = newTarget;
    }

    public QPlayer GetCurrentTarget()
    {
        return currentTarget;
    }

    public QPlayer GetPreviousTarget()
    {
        return (QPlayer)(((int)currentTarget + LevelManager.instance.numberOfPlayers - 1) % LevelManager.instance.numberOfPlayers);
    }

    private Vector3 GetRandomRelativeTargetPoint()
    {
        float randomRadius = Random.Range(MinRadius, MaxRadius);
        float randomAngle = Random.Range(0f, angleSpread);

        randomAngle = ((randomAngle - angleSpread / 2f) + 90f) * Mathf.PI / 180f;

        return new Vector3(randomRadius * Mathf.Cos(randomAngle), randomRadius * Mathf.Sin(randomAngle), 0);
    }
}
