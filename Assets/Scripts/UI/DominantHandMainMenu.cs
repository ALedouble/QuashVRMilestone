using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominantHandMainMenu : MonoBehaviour
{
    public static DominantHandMainMenu Instance;

    public GameObject rightHandedButton;
    public GameObject leftHandedButton;

    private DominantHandButton rightHandedButtonScript;
    private DominantHandButton leftHandedButtonScript;

    private void Awake()
    {
        Instance = this;

        rightHandedButtonScript = rightHandedButton.GetComponent<DominantHandButton>();
        leftHandedButtonScript = leftHandedButton.GetComponent<DominantHandButton>();
    }

    private void Start()
    {
        UpdateDominantHandButton();
    }

    public void UpdateDominantHandButton()
    {
        Debug.Log("UpdateDominantHandButton");
        rightHandedButtonScript.SetBackground(PlayerSettings.Instance.PlayerDominantHand == PlayerHand.RIGHT);
        leftHandedButtonScript.SetBackground(PlayerSettings.Instance.PlayerDominantHand == PlayerHand.LEFT);
    }
}
