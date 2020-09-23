using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicRandomTargetSelector : MonoBehaviour, ITargetSelector
{
    public float minRange;
    public float maxRange;
    public float angleSpread;
    public Vector3 offset = Vector3.up;

    private QPlayer currentTargetPlayer;
    public QPlayer CurrentTargetPlayer
    {
        get => currentTargetPlayer;
        private set
        {
            currentTargetPlayer = value;
            
            if (!GameManager.Instance.offlineMode && BallMultiplayerBehaviour.Instance.IsBallOwner)
            {
                photonView.RPC("SetCurrentTargetPlayer", RpcTarget.Others, value);
            }
        }
    }
    public Vector3 CurrentTargetPlayerPosition { get { return GameManager.Instance.PlayerSpawn[(int)currentTargetPlayer % GameManager.Instance.PlayerSpawn.Length].position; } }

    private Vector3 CenterOffset { get; set; }
    private float MinRadius { get; set; }
    private float MaxRadius { get; set; }

    private PhotonView photonView;

    private void Awake()
    {
        CenterOffset = offset;
        //CenterOffset = new Vector3(0, PlayerSettings.Instance.PlayerShoulderHeight, 0); //Add left/right handed offset
        MinRadius = minRange;
        MaxRadius = maxRange;

        photonView = GetComponent<PhotonView>();
    }

    public void SwitchTarget()
    {
        CurrentTargetPlayer = (QPlayer)( ((int)currentTargetPlayer + 1) % LevelManager.instance.numberOfPlayers);
    }

    [PunRPC]
    public void SetCurrentTargetPlayer(QPlayer newTarget)
    {
        CurrentTargetPlayer = newTarget;
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

    public QPlayer GetPreviousTargetPlayer()
    {
        return (QPlayer)(((int)currentTargetPlayer + LevelManager.instance.numberOfPlayers - 1) % LevelManager.instance.numberOfPlayers);
    }

    private Vector3 GetRandomRelativeTargetPoint()
    {
        float randomRadius = Random.Range(MinRadius, MaxRadius);
        float randomAngle = Random.Range(0f, angleSpread);

        randomAngle = ((randomAngle - angleSpread / 2f) + 90f) * Mathf.PI / 180f;

        return new Vector3(randomRadius * Mathf.Cos(randomAngle), randomRadius * Mathf.Sin(randomAngle), 0);
    }
}
