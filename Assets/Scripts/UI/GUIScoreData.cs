using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScoreData : GUIComponent
{
    public GUIScoreAnim anim;
    public bool cannotPlayAnim;

    public override void UpdateText(string newText)
    {
        base.UpdateText(newText);

        if (!cannotPlayAnim)
            anim.PlayAnimScoreIncrease();
    }

    public override void UpdateTextColor(Color32 newColor)
    {
        base.UpdateTextColor(newColor);
    }
}
