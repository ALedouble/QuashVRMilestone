using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignSetUp : MonoBehaviour
{
    public Transform CampaignTrans;
    public LevelsScriptable[] levelsToCheck;
    private LevelsScriptable[] levelsImplemented;
    public LevelRecap levelRecapValues;

    private void Start()
    {
        SetUpCampaign();
    }

    public void SetUpCampaign()
    {
        for (int i = 0; i < levelsToCheck.Length; i++)
        {
            if (levelsToCheck[i].level.levelProgression.isImplemented)
            {
                //spawn 
                LevelButton level = PoolManager.instance.SpawnFromPool("LevelButton", Vector3.zero, Quaternion.identity).GetComponent<LevelButton>();
                level.transform.parent = CampaignTrans;

                float xPos = (levelsToCheck[i].level.levelProgression.levelPos.x * 0.5f) * 0.01f;
                float yPos = ((levelsToCheck[i].level.levelProgression.levelPos.y * 0.5f) - 1000) * -0.01f;

                Vector2 startPos = new Vector2(xPos, yPos);


                level.rectTransform.anchoredPosition3D = new Vector3(xPos, yPos, 0);


                level.text.text = levelsToCheck[i].name;

                LevelsScriptable lvl = levelsToCheck[i];
                level.button.onClick.AddListener(() => SetUpLevelRecapValues(lvl));

                for (int y = 0; y < levelsToCheck[i].level.levelProgression.unlockConditions.Count; y++)
                {
                    LineRenderer line = PoolManager.instance.SpawnFromPool("Connection", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
                    line.transform.parent = CampaignTrans;
                    line.gameObject.transform.localPosition = Vector3.zero;


                    for (int x = 0; x < levelsToCheck[i].level.levelProgression.unlockConditions.Count; x++)
                    {
                        //Get concerned level pos
                        float xUPos = (levelsToCheck[i].level.levelProgression.unlockConditions[x].level.levelProgression.levelPos.x * 0.5f) * 0.01f;
                        float yUPos = ((levelsToCheck[i].level.levelProgression.unlockConditions[x].level.levelProgression.levelPos.y * 0.5f) - 1000) * -0.01f;

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
                        line.SetPosition((x + 1), new Vector3(endDiffPos.x, endDiffPos.y, 0));
                    }
                }
            }
        }
    }

    public void SetUpLevelRecapValues(LevelsScriptable selectedLevel)
    {
        levelRecapValues.levelTitle.text = selectedLevel.level.levelSpec.levelName;

        levelRecapValues.conditionType[0].text = selectedLevel.level.levelProgression.conditionsToComplete.ToString();
        levelRecapValues.conditionType[1].text = selectedLevel.level.levelProgression.conditionsToComplete.ToString();

        levelRecapValues.conditionReachedAt[0].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionType.ToString();
        levelRecapValues.conditionReachedAt[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt.ToString();

        levelRecapValues.bestTime.text = selectedLevel.level.levelProgression.minTiming.ToString();
        levelRecapValues.highScore.text = selectedLevel.level.levelProgression.maxScore.ToString();
        levelRecapValues.bestCombo.text = selectedLevel.level.levelProgression.maxCombo.ToString();

        if (selectedLevel.level.levelProgression.isDone)
        {
            levelRecapValues.stars[0].color = Color.yellow;

            for (int i = 0; i < 2; i++)
            {
                if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
                {
                    switch (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (selectedLevel.level.levelProgression.maxScore < selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].color = Color.yellow;
                            else
                                levelRecapValues.stars[i + 1].color = Color.white;
                            break;

                        case CompleteConditionType.Combo:
                            if (selectedLevel.level.levelProgression.maxCombo < selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].color = Color.yellow;
                            else
                                levelRecapValues.stars[i + 1].color = Color.white;
                            break;

                        case CompleteConditionType.Timing:
                            if (selectedLevel.level.levelProgression.minTiming < selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].color = Color.yellow;
                            else
                                levelRecapValues.stars[i + 1].color = Color.white;
                            break;
                    }
                }
                else
                {
                    switch (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt < selectedLevel.level.levelProgression.maxScore)
                                levelRecapValues.stars[i + 1].color = Color.yellow;
                            else
                                levelRecapValues.stars[i + 1].color = Color.white;
                            break;

                        case CompleteConditionType.Combo:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt < selectedLevel.level.levelProgression.maxCombo)
                                levelRecapValues.stars[i + 1].color = Color.yellow;
                            else
                                levelRecapValues.stars[i + 1].color = Color.white;
                            break;

                        case CompleteConditionType.Timing:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt < selectedLevel.level.levelProgression.minTiming)
                                levelRecapValues.stars[i + 1].color = Color.yellow;
                            else
                                levelRecapValues.stars[i + 1].color = Color.white;
                            break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < levelRecapValues.stars.Length; i++)
            {
                levelRecapValues.stars[i].color = Color.white;
            }
        }
    }
}
