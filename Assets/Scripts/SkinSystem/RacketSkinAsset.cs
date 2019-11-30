using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skin_00", menuName = "Custom/SkinAsset", order = 0)]
public class RacketSkinAsset : ScriptableObject
{
    [Header("Skin Info")]
    [SerializeField] string skinName = "Skin Name";
    [SerializeField] string skinDescription = "Skin's description here";

    [Header("Skin")]
    [SerializeField] Mesh skinMesh;
    [Tooltip("Array of the skin's materials. !!! MAKE SURE TO PUT THE MATERIALS IN THE RIGHT ORDER INSIDE THE ARRAY !!!")]
    [SerializeField] Material[] skinMaterials;

    [Header("UI")]
    [SerializeField] Sprite skinSprite;

    public string SkinName { get => skinName; private set => skinName = value; }
    public string SkinDescription { get => skinDescription; private set => skinDescription = value; }
    public Mesh SkinMesh { get => skinMesh; private set => skinMesh = value; }
    public Material[] SkinMaterials { get => skinMaterials; private set => skinMaterials = value; }
    public Sprite SkinSprite { get => skinSprite; private set => skinSprite = value; }
}
