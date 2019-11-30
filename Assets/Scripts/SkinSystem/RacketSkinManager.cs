using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketSkinManager : MonoBehaviour
{
    [SerializeField] SkinSlot[] skinSlots;
    public static RacketSkinManager instance;

    public SkinSlot[] SkinSlots { get => skinSlots; private set => skinSlots = value; }

    private void Awake()
    {
        //load skin slots data here

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); //Set the instance on DontDestroyOnLoad
    }
}

[System.Serializable]
public class SkinSlot
{
    [SerializeField] RacketSkinAsset racketSkinAsset;
    [SerializeField, Tooltip("The skin is locked or not (a locked skin cannot be equiped)")] bool isLocked = false;

    public RacketSkinAsset RacketSkinAsset { get => racketSkinAsset; private set => racketSkinAsset = value; }
    public bool IsLocked { get => isLocked; private set => isLocked = value; }
}
