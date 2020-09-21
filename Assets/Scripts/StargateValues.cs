using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StargateValues : MonoBehaviour
{
    public GameObject[] leftToRightGo;
    public Image[] leftToRightImage;
    public TextMeshProUGUI[] leftToRightText;

    public StargateValues()
    {
        leftToRightGo = new GameObject[0];
        leftToRightImage = new Image[0];
        leftToRightText = new TextMeshProUGUI[0];
    }
}
