using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignSetUp : MonoBehaviour
{
    public Transform CampaignTrans;
    public LevelsScriptable[] levelsToCheck;
    private LevelsScriptable[] levelsImplemented;

    public float ik;

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

                level.button.onClick.AddListener(() => CampaignLevel.Instance.SelectLevel(levelsToCheck[i]));

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
}
