using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketSkin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    private RacketSkinManager skinManager;

    [SerializeField] int currentSkinID;
    [SerializeField] RacketSkinAsset racketSkinAsset;

    private void OnEnable()
    {
        skinManager = RacketSkinManager.instance;
    }

    public void ResetSkin()
    {
        LoadSkin(currentSkinID);
    }

    public void LoadSkin(int skinID)
    {
        if (!skinManager)
        {
            Debug.LogError("Skin Manager Not Found");
            return;
        }

        if (skinManager.SkinSlots[skinID].IsLocked)
        {
            Debug.LogError("The skin you're trying to load, " + skinManager.SkinSlots[skinID].RacketSkinAsset.SkinName + ", is locked");
            return;
        }
        else
        {
            RacketSkinAsset l_skinAsset = skinManager.SkinSlots[skinID].RacketSkinAsset; //get skin asset
            SetSkin(l_skinAsset);

            //Save loaded skin values
            currentSkinID = skinID;
            racketSkinAsset = l_skinAsset;
        }
    }

    public void SetSkin(RacketSkinAsset skinAsset)
    {
        meshFilter.mesh = skinAsset.SkinMesh;
        meshRenderer.materials = skinAsset.SkinMaterials;
    }
}
