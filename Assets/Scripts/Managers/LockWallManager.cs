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
    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        wallLines = Instantiate(wallLinesPrefab, Vector3.zero, Quaternion.identity);
        IsClosing = false;
        wallLinesManager = wallLines.GetComponent<WallLinesManager>();

        BallManager.instance.BallInfo.OnFirstBounce += ExitProtectionState;
        BallManager.instance.BallInfo.OnReturnStart += EnterProtectionState;
    }

    public void EnterProtectionState()
    {
        if (isOpen)
            closingCoroutine = StartCoroutine(CloseLinesCoroutine());
    }

    private IEnumerator CloseLinesCoroutine()
    {
        isOpen = false;
        IsClosing = true;
        yield return new WaitForSeconds(closingDelay);
        wallLinesManager.CloseLines();
        IsClosing = false;
    }

    private void ExitProtectionState()
    {
        isOpen = true;
        if (IsClosing)
        {
            StopCoroutine(closingCoroutine);
            IsClosing = false;
        }

        wallLinesManager.OpenLines();
    }


}
