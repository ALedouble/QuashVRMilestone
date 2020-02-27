using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIWindow : MonoBehaviour
{
    [SerializeField] GameObject window;

    public void ActivateWindow()
    {
        window.SetActive(true);
    }

    public void DisactivateWindow()
    {
        window.SetActive(false);
    }
}
