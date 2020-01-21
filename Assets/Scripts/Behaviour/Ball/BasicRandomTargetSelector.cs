using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRandomTargetSelector : MonoBehaviour, ITargetSelector
{
    public float minRange;
    public float maxRange;
    public float angleSpread;

    public Transform[] playerPositions;
    private QPlayer currentTarget;

    public GameObject targetTestIndicator;
    public Vector3 CurrentTargetPlayerPosition {get {return playerPositions[(int)currentTarget % playerPositions.Length].position;}}           // Pour eviter les plantages... Le mieux Serait de verifier playerPosition.Length == PlayerID.Count - 1

    //public BasicRandomTargetSelector(float minRange, float maxRange, float angleSpread)
    //{
    //    this.minRange = minRange;
    //    this.maxRange = maxRange;
    //    this.angleSpread = angleSpread;
    //}

    public void SwitchTarget()
    {
        //currentTarget = (QPlayer)( ((int)currentTarget + 1) % ( (System.Enum.GetNames(typeof(QPlayer)).Length) - 1 ) );
        currentTarget = (QPlayer)( ((int)currentTarget + 1) % LevelManager.instance.numberOfPlayers);         
        Debug.Log("Switch Target - newCurrentPlayer: " + (int)currentTarget);
    }

    public Vector3 GetTargetPlayerPosition()
    {
        return CurrentTargetPlayerPosition;
    }

    public Vector3 GetNewTargetPosition()
    {
        Vector3 newTarget = CurrentTargetPlayerPosition + GetRandomRelativeTargetPoint() + Vector3.up;  //Beurk c'est pas beau....
        targetTestIndicator.transform.position = newTarget;
        Debug.Log("PlayerTargetPosition: " + CurrentTargetPlayerPosition);
        Debug.Log("TargetPosition: " + newTarget);

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

    private Vector3 GetRandomRelativeTargetPoint()
    {
        float randomRange = Random.Range(minRange, maxRange);
        float randomAngle = Random.Range(0f, angleSpread);

        randomAngle = ((randomAngle - angleSpread / 2f) + 90f) * Mathf.PI / 180f;

        return new Vector3(randomRange * Mathf.Cos(randomAngle), randomRange * Mathf.Sin(randomAngle), 0);
    }
}
