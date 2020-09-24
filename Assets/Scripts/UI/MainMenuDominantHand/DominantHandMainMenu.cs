using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominantHandMainMenu : MonoBehaviour
{
    public static DominantHandMainMenu Instance;

    public GameObject rightHandedButton;
    public GameObject leftHandedButton;

    public OptionMenuDominantHandButton rightHandedButtonScript;
    public OptionMenuDominantHandButton leftHandedButtonScript;

    private void Awake()
    {
        Instance = this;


        rightHandedButtonScript = rightHandedButton.GetComponent<OptionMenuDominantHandButton>();
        leftHandedButtonScript = leftHandedButton.GetComponent<OptionMenuDominantHandButton>();
    }

    private void Start()
    {
        UpdateDominantHandButton();
    }

    public void UpdateDominantHandButton()
    {
        rightHandedButtonScript.UpdateBackground();
        leftHandedButtonScript.UpdateBackground();
    }
}
