using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIRuleBisLevel : MonoBehaviour
{
    [Header("Text values settings")]
    [SerializeField,TextArea(3,10)] string ruleText = "Rule";

    [Header("References")]
    [SerializeField] TextMeshPro textMeshRule;

    protected virtual void OnEnable()
    {
        SetRuleText();
    }

    public virtual void SetRuleText()
    {
        textMeshRule.text = ruleText;
    }
}
