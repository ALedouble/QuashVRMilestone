using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIFirstTimeConfirmationButton : MonoBehaviour
{
    public void OnButtonPress()
    {
        GUIFirstTimeMenu.Instance.ConfirmChoice();
    }
}
