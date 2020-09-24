using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Info : MonoBehaviour
{
    public GameObject mainScreenGo;
    public GameObject firstTimeGo;

    private void Start()
    {
        

        if (!PlayerSettings.Instance.HadDominantHandWarning)
        {
            firstTimeGo.SetActive(true);

            PlayerSettings.Instance.HadDominantHandWarning = true;
        }
        else
        {
            mainScreenGo.SetActive(true);
        }
    }
}
