using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignSetUp : MonoBehaviour
{
    public Transform CampaignTrans;
    public List<LevelsScriptable> levelsToCheck;
    //private LevelsScriptable[] levelsImplemented;
    public LevelRecap levelRecapValues;

    private LevelsScriptable levelToPlay;

    public Sprite lockedStarSprite;
    public Sprite unlockedStarSprite;

    [ColorUsage(true, true)]Color lockedLineColor;
    [ColorUsage(true, true)]Color unlockedLineColor;
    Material lockedLineMaterial;
    Material unlockedLineMaterial;

    private int totalOfStars;

    private void Start()
    {
        SetUpCampaign();
    }

    void InitLineMaterials()
    {
        lockedLineColor = new Color(1f, 1f, 1f) * 0.5f;
        unlockedLineColor = new Color(1f, 1f, 1f) * 0.9f;

        lockedLineMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));
        unlockedLineMaterial = new Material(Shader.Find("Lightweight Render Pipeline/Lit"));

        lockedLineMaterial.EnableKeyword("_EMISSION");
        unlockedLineMaterial.EnableKeyword("_EMISSION");

        //lockedLineMaterial.SetColor("_BaseColor", lockedLineColor);
        //unlockedLineMaterial.SetColor("_BaseColor", unlockedLineColor);

        lockedLineMaterial.SetColor("_EmissionColor", lockedLineColor);
        unlockedLineMaterial.SetColor("_EmissionColor", unlockedLineColor);
    }

    public void SetUpCampaign()
    {
        InitLineMaterials();

        for (int i = 0; i < levelsToCheck.Count; i++)
        {
            //Check if the level is implemented in the campaign
            if (levelsToCheck[i].level.levelProgression.isImplemented)
            {
                //Spawn Level ICON 
                LevelButton level = PoolManager.instance.SpawnFromPool("LevelButton", Vector3.zero, Quaternion.identity).GetComponent<LevelButton>();
                level.transform.parent = CampaignTrans;

                //Transpose editor position into campaign position
                float xPos = (levelsToCheck[i].level.levelProgression.levelPos.x * 0.5f) * 0.01f;
                float yPos = ((levelsToCheck[i].level.levelProgression.levelPos.y * 0.5f) - 1000) * -0.01f;

                Vector2 startPos = new Vector2(xPos, yPos);

                //Set LEVEL icon position
                level.rectTransform.anchoredPosition3D = new Vector3(xPos, yPos, 0);

                //Set Level Title
                level.text.text = levelsToCheck[i].name;

                //Set onClick Event to reload values on the Level Panel
                LevelsScriptable lvl = levelsToCheck[i];
                level.button.onClick.AddListener(() => SetUpLevelRecapValues(lvl));



                for (int y = 0; y < levelsToCheck[i].level.levelProgression.unlockConditions.Count; y++)
                {
                    ////////////    DRAW Line renderer for CONNECTION   ////////////

                    //Spawn a LineRenderer from the Pool
                    LineRenderer line = PoolManager.instance.SpawnFromPool("Connection", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
                    line.transform.parent = CampaignTrans;
                    line.gameObject.transform.localPosition = Vector3.zero;

                    //Get concerned level pos
                    float xUPos = (levelsToCheck[i].level.levelProgression.unlockConditions[y].level.levelProgression.levelPos.x * 0.5f) * 0.01f;
                    float yUPos = ((levelsToCheck[i].level.levelProgression.unlockConditions[y].level.levelProgression.levelPos.y * 0.5f) - 1000) * -0.01f;

                    Vector2 lastPos = new Vector2(xUPos, yUPos);

                    //Retargeting vectors on the edge of the level
                    Vector2 direction1 = lastPos - startPos;
                    direction1 = direction1.normalized;
                    Vector2 startDiffPos = startPos + direction1 * 0.31f;


                    Vector2 direction = startPos - lastPos;
                    direction = direction.normalized;
                    Vector2 endDiffPos = lastPos + direction * 0.31f;

                    //Set line positions
                    line.SetPosition(0, new Vector3(startDiffPos.x, startDiffPos.y, 0));
                    line.SetPosition((y + 1), new Vector3(endDiffPos.x, endDiffPos.y, 0));


                    ////////////    CHECK UN/LOCK CONDITIONS    ////////////

                    if (!levelsToCheck[i].level.levelProgression.isUnlocked)
                    {
                        line.sharedMaterial = lockedLineMaterial;
                        level.button.interactable = false;

                        for (int x = 0; x < level.lockImages.Count; x++)
                        {
                            level.lockImages[x].SetActive(true);
                            level.unlockImages[x].SetActive(false);
                        }

                        if (levelsToCheck[i].level.levelProgression.unlockConditions[y].level.levelProgression.isDone && totalOfStars < levelsToCheck[i].level.levelProgression.starsRequired)
                        {
                            levelsToCheck[i].level.levelProgression.isUnlocked = true;
                            level.button.interactable = true;
                            line.sharedMaterial = unlockedLineMaterial;

                            for (int x = 0; x < level.lockImages.Count; x++)
                            {
                                level.lockImages[x].SetActive(false);
                                level.unlockImages[x].SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        line.sharedMaterial = unlockedLineMaterial;
                        level.button.interactable = true;

                        for (int x = 0; x < level.lockImages.Count; x++)
                        {
                            level.lockImages[x].SetActive(false);
                            level.unlockImages[x].SetActive(true);
                        }
                    }
                }



            }
        }
    }

    public void SetUpLevelRecapValues(LevelsScriptable selectedLevel)
    {
        levelToPlay = selectedLevel;

        if (selectedLevel.level.levelSpec.levelName != null)
            levelRecapValues.levelTitle.text = selectedLevel.level.levelSpec.levelName;
        else
            levelRecapValues.levelTitle.text = "NO NAME";

        levelRecapValues.conditionType[0].text = selectedLevel.level.levelProgression.conditionsToComplete.ToString();
        levelRecapValues.conditionType[1].text = selectedLevel.level.levelProgression.conditionsToComplete.ToString();

        levelRecapValues.conditionReachedAt[0].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionType.ToString();
        levelRecapValues.conditionReachedAt[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt.ToString();

        levelRecapValues.bestTime.text = selectedLevel.level.levelProgression.minTiming.ToString();
        levelRecapValues.highScore.text = selectedLevel.level.levelProgression.maxScore.ToString();
        levelRecapValues.bestCombo.text = selectedLevel.level.levelProgression.maxCombo.ToString();

        /////////////   STARS CONDITIONS CHECK  /////////////

        if (selectedLevel.level.levelProgression.isDone)
        {
            levelRecapValues.stars[0].sprite = unlockedStarSprite;

            for (int i = 0; i < 2; i++)
            {
                if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
                {
                    switch (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (selectedLevel.level.levelProgression.maxScore < selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Combo:
                            if (selectedLevel.level.levelProgression.maxCombo < selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Timing:
                            if (selectedLevel.level.levelProgression.minTiming < selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;
                    }
                }
                else
                {
                    switch (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt < selectedLevel.level.levelProgression.maxScore)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Combo:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt < selectedLevel.level.levelProgression.maxCombo)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Timing:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt < selectedLevel.level.levelProgression.minTiming)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < levelRecapValues.stars.Length; i++)
            {
                levelRecapValues.stars[i].sprite = lockedStarSprite;
            }
        }
    }

    public void PlayLevel()
    {
        if (levelToPlay == null)
        {
            Debug.LogError("NO LEVEL SELECTED");
            return;
        }

        int indexOfLevel = levelsToCheck.IndexOf(levelToPlay);
        CampaignLevel.Instance.SelectLevel(indexOfLevel);
    }

    public void MoveCampaign()
    {

    }
}
