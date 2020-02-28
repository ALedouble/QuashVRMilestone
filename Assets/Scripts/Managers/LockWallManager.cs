using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWallManager : MonoBehaviour
{
    public static LockWallManager Instance { get; private set; }

    public GameObject wallLinesPrefab;
    public float closingDelay = 0.5f;

    private GameObject wallLines;
    private WallLinesManager wallLinesManager;
    public bool IsClosing { get; private set; }
    private Coroutine closingCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        wallLines = Instantiate(wallLinesPrefab, Vector3.zero, Quaternion.identity);
        IsClosing = false;
        wallLinesManager = wallLines.GetComponent<WallLinesManager>();

        BallManager.instance.OnFirstBounce += ExitProtectionState;
        BallManager.instance.OnReturnStart += EnterProtectionState;
    }

    private void EnterProtectionState()
    {
        closingCoroutine = StartCoroutine(CloseLinesCoroutine());
    }

    private IEnumerator CloseLinesCoroutine()
    {
        IsClosing = true;
        yield return new WaitForSeconds(closingDelay);
        wallLinesManager.CloseLines();
        IsClosing = false;
    }

    private void ExitProtectionState()
    {
        if (IsClosing)
        {
            StopCoroutine(closingCoroutine);
            IsClosing = false;
        }

        wallLinesManager.OpenLines();
    }


}
