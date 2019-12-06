using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallColorBehaviour : MonoBehaviour
{
    Renderer renderer;

    [Header("Color Settings")]
    /*public Material[] materials;  */
    public Color[] colors;

    private int colorID = 0;

    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
    }

    public int GetBallsColor()
    {
        return colorID;
    }

    public void SetBallColor(int colorID)
    {
        this.colorID = colorID;
        //renderer.material = materials[colorID];
        renderer.material.color = colors[colorID];
    }

    private void SwitchColor()
    {
        //ColorManager.instance.SwitchBallColor();
        colorID = (colorID + 1) % colors.Length;
        renderer.material.color = colors[colorID];
    }
}
