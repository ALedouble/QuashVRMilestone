using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketSkin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    private RacketSkinManager skinManager;

    [Header("Skin data")]
    [SerializeField] int currentSkinID;
    [SerializeField] RacketSkinAsset racketSkinAsset;

    [Header("Racket skin settings")]
    [SerializeField] bool loadSkinOnStart = false;

    private void OnEnable()
    {
        //load player skin data here
        skinManager = RacketSkinManager.instance;
    }

    private void Start()
    {
        if (loadSkinOnStart) ResetSkin();
    }

    public void ResetSkin()
    {
        LoadSkin(currentSkinID);
    }

    /// <summary>
    /// Load a skin with an ID that is corresponding to a cell in the SkinSlot's array in the RacketSkinManager
    /// </summary>
    /// <param name="skinID"></param>
    public void LoadSkin(int skinID)
    {
        if (!skinManager)
        {
            Debug.LogError("Skin Manager Not Found");
            return;
        }

        if(skinID >= skinManager.SkinSlots.Length)
        {
            Debug.LogError("Wrong ID, the ID (" + skinID.ToString() + ") is bigger than the Skinslot's array length");
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

    /// <summary>
    /// Change the mesh and the materials to match the data contained in the asset
    /// </summary>
    /// <param name="skinAsset"></param>
    public void SetSkin(RacketSkinAsset skinAsset)
    {
        meshFilter.mesh = skinAsset.SkinMesh;
        meshRenderer.materials = skinAsset.SkinMaterials;
    }
}
