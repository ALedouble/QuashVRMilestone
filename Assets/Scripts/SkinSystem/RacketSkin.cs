using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RacketSkin : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] MeshFilter meshFilter;
    [SerializeField, Required] MeshRenderer meshRenderer;

    private RacketSkinManager skinManager;

    [Header("Skin data")]
    [SerializeField] int currentSkinID;
    [SerializeField, ReadOnly] RacketSkinAsset racketSkinAsset;

    [Header("Racket skin settings")]
    [SerializeField] bool loadSkinOnStart = false;

    [Header("Debug")]
    [SerializeField] bool debugMode;
    [SerializeField, ShowIf("IsInDebugMode")] int debugID;
    [SerializeField, ShowIf("IsInDebugMode")] RacketSkinAsset debugSkinAsset;

    bool IsInDebugMode()
    {
        return debugMode;
    }

    [Button("Get References")]
    private void GetReferences()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        //load player skin data here
        skinManager = RacketSkinManager.instance;
    }

    private void Start()
    {
        if (loadSkinOnStart) ResetSkin();
    }

    #region DEBUG METHODS
    [Button("DEBUG SetSkin")]
    void DebugSetSkin()
    {
        if (!IsInDebugMode())
        {
            Debug.LogWarning("Debug Method only works in debug mode");
            return;
        }
        SetSkin(debugSkinAsset);
    }

    [Button("DEBUG LoadSkin")]
    void DebugLoadSkin()
    {
        if (!IsInDebugMode())
        {
            Debug.LogWarning("Debug Method only works in debug mode");
            return;
        }
        LoadSkin(debugID);
    }
    #endregion

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

        if (skinID >= skinManager.SkinSlots.Length)
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
