using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIFirstTimeMenu : MonoBehaviour
{
    public static GUIFirstTimeMenu Instance { get; private set; }

    private int currentSelectedHand;

    public GameObject rightHandedButton;
    public GameObject leftHandedButton;

    public GameObject MainScreen;
    public Animator CanvasFirstTimeAnimator;

    private GUIFirstTimeDominantHandButton rightHandedButtonScript;
    private GUIFirstTimeDominantHandButton leftHandedButtonScript;

    private GUISounds guiSounds;

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        currentSelectedHand = -1;

        rightHandedButtonScript = rightHandedButton.GetComponent<GUIFirstTimeDominantHandButton>();
        leftHandedButtonScript = leftHandedButton.GetComponent<GUIFirstTimeDominantHandButton>();

        guiSounds = GetComponent<GUISounds>();
    }

    public void Start()
    {
        UpdateHandButtonsDisplay();
    }

    public void SelectDominantHand(PlayerHand selectedHand)
    {
        currentSelectedHand = (int)selectedHand;
        UpdateHandButtonsDisplay();
    }

    public void ConfirmChoice()
    {
        if(currentSelectedHand == -1)
        {
            //Nothing Selected feedBack?
            return;  
        }

        PlayerSettings.Instance.PlayerDominantHand = (PlayerHand)currentSelectedHand;
        MoveToMainMenu();
    }

    private void UpdateHandButtonsDisplay()
    {
        rightHandedButtonScript.IsSelected = ( currentSelectedHand == (int)PlayerHand.RIGHT );
        leftHandedButtonScript.IsSelected = ( currentSelectedHand == (int)PlayerHand.LEFT );
    }

    private void MoveToMainMenu()
    {
        guiSounds.PlayValidateEntry();
        CanvasFirstTimeAnimator.Play("A_Window_Disappear");
        MainScreen.SetActive(true);
    }
}
