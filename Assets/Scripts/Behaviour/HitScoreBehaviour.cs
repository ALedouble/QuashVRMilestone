using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HitScoreBehaviour : MonoBehaviour
{
    //Récupération des components
    private TextMeshProUGUI textMesh;
    public RectTransform scoreTextRect;


    [Header("Hit Animation")]
    public AnimationCurve textAnim;
    private float percent;
    private float currentTime;
    public float animSpeed = 1f;
    private float maxSize;
    private float minSize;

    bool isAnimationOver = true;
    bool isOnReverse = false;



    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }


    private void Update()
    {
        if (!isAnimationOver)
        {
            if (!isOnReverse)
            {
                if (currentTime < 1)
                {
                    currentTime += Time.deltaTime * animSpeed;


                    percent = textAnim.Evaluate(currentTime);

                    textMesh.fontSize = minSize + (percent * maxSize);
                    scoreTextRect.localPosition = new Vector3(0, percent * 0.05f, percent * -0.8f);
                }
                else
                {
                    isOnReverse = true;
                }
            }
            else
            {
                if (currentTime > 0)
                {
                    currentTime -= Time.deltaTime * animSpeed;


                    percent = textAnim.Evaluate(currentTime);

                    textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, percent * 1);
                }
                else
                {
                    isAnimationOver = true;
                    DisableObject();
                }
            }
        }
    }




    public void SetHitValues(float scoreValue, Color textColor)
    {
        textMesh.text = scoreValue.ToString();
        textMesh.color = textColor;

        float getSize = 0;
        float timeToGet = scoreValue / ScoreManager.Instance.maxScoreValue;
        minSize = ScoreManager.Instance.minTextSize;

        getSize = ScoreManager.Instance.textValues.Evaluate(timeToGet); //"return" une valeur entre 0 et 1
        maxSize = (getSize * (ScoreManager.Instance.maxTextSize - minSize));
        

        currentTime = 0;
        isOnReverse = false;
        isAnimationOver = false;
    }

    public void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
