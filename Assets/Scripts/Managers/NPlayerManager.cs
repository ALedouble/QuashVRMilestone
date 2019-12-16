using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPlayerManager : MonoBehaviour
{
    #region Singleton

    public static NPlayerManager instance;

    private void Awake()
    {
        if (instance)
            Destroy(this);
        else
            instance = this;
    }
    #endregion

    //Player settings (Gaucher/Droitier, pouvoirs utilises, ...)



    public void RightHandActionCall()
    {

    }

    public void LeftHandActionCall()
    {

    }

    public void RightHandActionEnd()
    {

    }

    public void LeftHandActionEnd()
    {

    }

    //Ajouter les super?
}
