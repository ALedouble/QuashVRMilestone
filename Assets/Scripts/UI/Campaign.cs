using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using System.Linq;

public class Campaign : MonoBehaviour
{
    [Header("Levels")]
    public List<LevelsScriptable> levelsToCheck;

    [Header("Panel")]
    public RectTransform CampaignPanel;
    public List<LevelsScriptable> levelsImplemented;
    public LevelRecap levelRecapValues;
    public GameObject upButton;
    public GameObject downButton;

    private int numberOfPanelPositions = 31;
    private float panelSize = 1.6f;
    [SerializeField] private float[] panelPositions = new float[0];
    private int panelIndex = -1;
    private float nextPanelPosition = 0;
    private float panelTop = 0f;
    private float panelBottom = 100f;
    private float panelLeft = -0.3f;
    private float panelRight = 3.3f;
    private int lastIndex = 0;
    private bool isMoving;
    [Range(0.01f, 3f)] public float scrollingSpeed;

    private LevelsScriptable levelToPlay = null;
    private LevelButton buttonSelected = null;

    [Header("Stars")]
    public Sprite lockedStarSprite;
    public Sprite unlockedStarSprite;
    [HideInInspector] public int PlayerStars;
    public TextMeshProUGUI starCounter;

    [Header("Conditions Specifics")]
    public string timingConditionEntry;
    public string scoreConditionEntry;
    public string comboConditionEntry;

    [Header("Side Panel")]
    public GameObject sidePanel;

    bool isLevelLaunch;
    private Viveport.StatusCallback ViveportCallback;


    public static Campaign instance;




    private void Awake()
    {
        ViveportCallback += (viveportInt => { });

        
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }

        SetUpCampaign();
        isLevelLaunch = false;
    }

    private void Update()
    {
        if (isMoving)
            MovingPanel();
    }


    float GetYScreenPositionPercent(float Ypos)
    {
        return (Ypos * 0.01f);
    }

    float GetXScreenPositionPercent(float Xpos)
    {
        return (Xpos / 6f);
    }

    Vector2 GetConvertedPositionFromPercents(Vector2 percents)
    {
        return new Vector2(((percents.x * (panelRight - panelLeft)) / 100f) + panelLeft, (percents.y * panelBottom) / 100f);
    }

    /// <summary>
    /// Set up Panel positions
    /// </summary>
    private void SetUpPanelPositions()
    {
        panelPositions = new float[numberOfPanelPositions + 1];
        //panelBottom = numberOfPanelPositions * panelSize;

        //Debug.Log("panelSize percent : " + ((panelSize * 100f) / panelBottom));

        for (int i = 0; i < numberOfPanelPositions + 1; i++)
        {
            panelPositions[i] = (panelBottom - (panelSize * 0.5f)) - (panelSize * i);
        }

        lastIndex = GetPanelIndex(GetHighestLevelInCampaign());
        //Debug.Log("lastIndex : " + lastIndex);
    }


    /// <summary>
    /// Check if the level is implemented in the campaign
    /// </summary>
    private void Check4ImplementedLevels()
    {
        levelsImplemented = new List<LevelsScriptable>();
        PlayerStars = 0;

        for (int i = 0; i < levelsToCheck.Count; i++)
        {
            if (levelsToCheck[i].level.levelProgression.isImplemented)
            {
                levelsImplemented.Add(levelsToCheck[i]);
                //levelsToCheck[i].level.levelProgression.LevelIndex = i;
            }
        }


        ReorderCampaign();

        JSON.instance.SetUpDATAs();
    }

    /// <summary>
    /// Count the number of stars
    /// </summary>
    /// <param name="level"></param>
    private void CountingStars(LevelsScriptable level)
    {
        if (level.level.levelProgression.isDone)
        {
            PlayerStars += 1;
            for (int i = 0; i < level.level.levelProgression.numberOfAdditionalConditions; i++)
            {
                if (level.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
                {
                    switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (level.level.levelProgression.maxScore > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                PlayerStars += 1;
                            break;

                        case CompleteConditionType.Combo:
                            if (level.level.levelProgression.maxCombo > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                PlayerStars += 1;
                            break;

                        case CompleteConditionType.Timing:
                            if (level.level.levelProgression.minTiming > level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                PlayerStars += 1;
                            break;
                    }
                }
                else
                {
                    switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (level.level.levelProgression.conditionsToComplete[i].conditionReachedAt > level.level.levelProgression.maxScore)
                                PlayerStars += 1;
                            break;

                        case CompleteConditionType.Combo:
                            if (level.level.levelProgression.conditionsToComplete[i].conditionReachedAt > level.level.levelProgression.maxCombo)
                                PlayerStars += 1;
                            break;

                        case CompleteConditionType.Timing:
                            if (level.level.levelProgression.conditionsToComplete[i].conditionReachedAt > level.level.levelProgression.minTiming)
                                PlayerStars += 1;
                            break;
                    }
                }
            }

        }

        starCounter.text = PlayerStars.ToString();
    }

    /// <summary>
    /// Tri la list
    /// </summary>
    private void ReorderCampaign()
    {
        levelsImplemented.Sort();
        for (int i = 0; i < levelsImplemented.Count; i++)
        {
            levelsImplemented[i].level.levelProgression.LevelIndex = i;
        }
    }

    /// <summary>
    /// Get panel index from level
    /// </summary>
    /// <param name="levelIndex">level reference</param>
    /// <returns></returns>
    public int GetPanelIndex(LevelsScriptable levelIndex)
    {
        Vector2 temp = GetConvertedPositionFromPercents(new Vector2(GetXScreenPositionPercent(levelIndex.level.levelProgression.levelPos.x), GetYScreenPositionPercent(levelIndex.level.levelProgression.levelPos.y)));
        float levelComparer = temp.y;

        //Debug.Log("level Ypos : " + temp.y);

        if (panelPositions.Length == 0)
            SetUpCampaign();

        for (int y = numberOfPanelPositions; y >= 0; y--)
        {
            float comparer = panelPositions[y] - (panelSize * 0.5f);
            //Debug.Log("At i = " + y + "compared : " + comparer);

            if (levelComparer <= comparer)
            {
                int finalIndex = y + 1;
                return finalIndex;
            }
        }

        return 0;
    }

    /// <summary>
    /// Set last panel index
    /// </summary>
    /// <param name="newIndex"></param>
    private void SetLastRecordedPanelIndex(int newIndex)
    {
        CampaignLevel.lastRecordedPanelIndex = newIndex;
    }

    /// <summary>
    /// Setup panel position to the first level unlocked from the top position
    /// </summary>
    private void SetUpPanelPositionAtStart()
    {
        SetPanelPosition(CampaignLevel.lastRecordedPanelIndex);

        //Debug.Log("lastRecordedPanelIndex" + CampaignLevel.lastRecordedPanelIndex);

        if(CampaignLevel.lastRecordedPanelIndex != 0)
            return;

        for (int i = levelsImplemented.Count - 1; i >= 0; i--)
        {
            if (levelsImplemented[i].level.levelProgression.isUnlocked)
            {
                Vector2 temp = GetConvertedPositionFromPercents(new Vector2(GetXScreenPositionPercent(levelsImplemented[i].level.levelProgression.levelPos.x), -GetYScreenPositionPercent(levelsImplemented[i].level.levelProgression.levelPos.y)));
                float levelComparer = temp.y + panelSize;

                MoveCampaignPanelTo(GetPanelIndex(levelsImplemented[i]));

                SetLastRecordedPanelIndex(GetPanelIndex(levelsImplemented[i]));

                #region OLD
                //for (int y = numberOfPanelPositions; y >= 0; y--)
                //{
                //    float comparer = (panelSize * y) * -1;

                //    if (levelComparer <= comparer)
                //    {

                //        int finalIndex = numberOfPanelPositions - (y + 1);

                //        MoveCampaignPanelTo(finalIndex);

                //        SetLastRecordedPanelIndex(finalIndex);
                //        return;
                //    }
                //}
                #endregion
            }
        }
    }

    /// <summary>
    /// Set up campaign tree
    /// </summary>
    private void SetUpCampaign()
    {
        //Set Up the number of stars
        Check4ImplementedLevels();


        //Set up notching 4 panel Move
        SetUpPanelPositions();

        for (int i = 0; i < levelsImplemented.Count; i++)
        {
            //Counting STARS
            CountingStars(levelsImplemented[i]);
        }


        //Set Up Graphic Elements
        for (int i = 0; i < levelsImplemented.Count; i++)
        {
            //Counting STARS
            //CountingStars(levelsImplemented[i]);

            //Spawn Level ICON 
            LevelButton level = PoolManager.instance.SpawnFromPool("LevelButton", Vector3.zero, Quaternion.identity).GetComponent<LevelButton>();
            level.transform.SetParent(CampaignPanel.transform);
            level.gameObject.name = levelsImplemented[i].level.levelSpec.levelName;

            //Transpose editor position into campaign position
            float xPos = GetXScreenPositionPercent(levelsImplemented[i].level.levelProgression.levelPos.x);
            float yPos = -GetYScreenPositionPercent(levelsImplemented[i].level.levelProgression.levelPos.y);

            Vector2 startPos = new Vector2(xPos, yPos);
            startPos = GetConvertedPositionFromPercents(startPos);

            //Debug.Log("startPos : " + startPos);

            //Set LEVEL icon position
            level.rectTransform.anchoredPosition3D = new Vector3(startPos.x, startPos.y, 0);

            //Set/Display Level Number or name 
            for (int u = 0; u < level.buttonTexts.Count; u++)
            {
                if (levelsImplemented[i].level.levelSpec.buttonName != null && levelsImplemented[i].level.levelSpec.buttonName != "" && levelsImplemented[i].level.levelSpec.buttonName != " ")
                {
                    level.buttonTexts[u].text = levelsImplemented[i].level.levelSpec.buttonName;
                }
                else
                {
                    if (levelsImplemented[i].level.levelProgression.levelNumber < 10)
                    {
                        level.buttonTexts[u].text = "0" + levelsImplemented[i].level.levelProgression.levelNumber.ToString();
                    }
                    else
                    {
                        level.buttonTexts[u].text = levelsImplemented[i].level.levelProgression.levelNumber.ToString();
                    }
                }
            }


            //Set onClick Event to reload values on the Level Panel
            LevelsScriptable lvl = levelsImplemented[i];
            level.button.onClick.AddListener(() => SetUpLevelRecapValues(lvl, level));

            //"RIGHT NOW" For the first Level ONLY (because no unlockCondition required)
            if (levelsImplemented[i].level.levelProgression.unlockConditions.Count == 0)
            {
                //For the first time because the level isn't unlocked ALREADY
                if (!levelsImplemented[i].level.levelProgression.isUnlocked)
                {
                    if (PlayerStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                    {
                        levelsImplemented[i].level.levelProgression.isUnlocked = true;
                        level.button.interactable = true;

                        if (!levelsImplemented[i].level.levelSpec.suddenDeath && !levelsImplemented[i].level.levelSpec.mandatoryBounce && !levelsImplemented[i].level.levelSpec.timeAttack)
                        {
                            level.unlockImages.SetActive(true);
                            Debug.Log("no unlock cond UNLOCK : " + levelsImplemented[i].level.levelSpec.levelName);
                        }
                        else
                        {
                            level.exoticUnlockImages.SetActive(true);
                            Debug.Log("no unlock cond EXO : " + levelsImplemented[i].level.levelSpec.levelName);
                        }
                    }
                    else
                    {
                        level.button.interactable = false;


                        level.lockImages.SetActive(true);
                    }
                }
                else
                {
                    level.button.interactable = true;


                    if (levelsImplemented[i].level.levelProgression.isDone)
                    {
                        if (levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions > 0
                            && !levelsImplemented[i].level.levelSpec.suddenDeath && !levelsImplemented[i].level.levelSpec.mandatoryBounce && !levelsImplemented[i].level.levelSpec.timeAttack)
                        {
                            int conditionCompleted = 0;

                            for (int o = 0; o < levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions; o++)
                            {
                                if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionComparator == 0)
                                {
                                    switch (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionType)
                                    {
                                        case CompleteConditionType.Score:
                                            if (levelsImplemented[i].level.levelProgression.maxScore > levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Combo:
                                            if (levelsImplemented[i].level.levelProgression.maxCombo > levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Timing:
                                            if (levelsImplemented[i].level.levelProgression.minTiming > levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt)
                                                conditionCompleted += 1;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionType)
                                    {
                                        case CompleteConditionType.Score:
                                            if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt > levelsImplemented[i].level.levelProgression.maxScore)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Combo:
                                            if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt > levelsImplemented[i].level.levelProgression.maxCombo)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Timing:
                                            if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt > levelsImplemented[i].level.levelProgression.minTiming)
                                                conditionCompleted += 1;
                                            break;
                                    }
                                }
                            }

                            if (conditionCompleted == levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions)
                            {

                                level.fullStarsImages.SetActive(true);
                            }
                            else
                            {
                                level.doneImages.SetActive(true);
                            }
                        }
                        else
                        {
                            if (!levelsImplemented[i].level.levelSpec.suddenDeath && !levelsImplemented[i].level.levelSpec.mandatoryBounce && !levelsImplemented[i].level.levelSpec.timeAttack)
                            {
                                level.fullStarsImages.SetActive(true);
                            }
                            else
                            {
                                level.exoticDoneImages.SetActive(true);
                            }

                        }

                    }
                    else
                    {
                        if (!levelsImplemented[i].level.levelSpec.suddenDeath && !levelsImplemented[i].level.levelSpec.mandatoryBounce && !levelsImplemented[i].level.levelSpec.timeAttack)
                        {
                            level.unlockImages.SetActive(true);
                        }
                        else
                        {
                            level.exoticUnlockImages.SetActive(true);
                        }

                    }
                }
            }
            else
            {
                for (int y = 0; y < levelsImplemented[i].level.levelProgression.unlockConditions.Count; y++)
                {
                    ////////////    DRAW Line renderer for CONNECTION    ////////////

                    //Spawn a LineRenderer from the Pool
                    UILineRenderer line = PoolManager.instance.SpawnFromPool("Connection", Vector3.zero, Quaternion.identity).GetComponent<UILineRenderer>();
                    RectTransform rect = line.gameObject.GetComponent<RectTransform>();
                    line.transform.SetParent(CampaignPanel.transform);


                    //Get concerned level pos
                    float xUPos = GetXScreenPositionPercent(levelsImplemented[i].level.levelProgression.unlockConditions[y].level.levelProgression.levelPos.x);
                    float yUPos = -GetYScreenPositionPercent(levelsImplemented[i].level.levelProgression.unlockConditions[y].level.levelProgression.levelPos.y);

                    Vector2 lastPos = new Vector2(xUPos, yUPos);
                    lastPos = GetConvertedPositionFromPercents(lastPos);

                    //Retargeting vectors on the edge of the level
                    Vector2 direction1 = lastPos - startPos;
                    direction1 = direction1.normalized;
                    Vector2 startDiffPos = startPos + direction1 * 0.31f;


                    Vector2 direction = startPos - lastPos;
                    Vector2 newLastPos = direction * -1;
                    Vector2 newDirection = direction.normalized;

                    Vector2 endDiffPos = newLastPos + newDirection * 0.62f;

                    line.gameObject.transform.localPosition = new Vector2(startDiffPos.x, startDiffPos.y);


                    //Set line positions
                    line.Points[1] = new Vector2(endDiffPos.x, endDiffPos.y);


                    //Set line connection color and button interactable
                    if (!levelsImplemented[i].level.levelProgression.isUnlocked)
                    {
                        if (levelsImplemented[i].level.levelProgression.unlockConditions[y].level.levelProgression.isDone && PlayerStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                        {
                            levelsImplemented[i].level.levelProgression.isUnlocked = true;
                            level.button.interactable = true;
                            line.color = new Color32((byte)255, (byte)255, (byte)255, (byte)255);
                        }
                        else
                        {
                            line.color = new Color32((byte)150, (byte)150, (byte)150, (byte)255);
                            level.button.interactable = false;


                            level.lockImages.SetActive(true);
                        }
                    }
                    else
                    {
                        line.color = new Color32((byte)255, (byte)255, (byte)255, (byte)255);
                    }
                }

                ////////////    CHECK UN/LOCK CONDITIONS    ////////////
                if (levelsImplemented[i].level.levelProgression.isUnlocked)
                {
                    if (levelsImplemented[i].level.levelProgression.isDone)
                    {
                        if (levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions > 0
                            && !levelsImplemented[i].level.levelSpec.suddenDeath && !levelsImplemented[i].level.levelSpec.mandatoryBounce && !levelsImplemented[i].level.levelSpec.timeAttack)
                        {
                            int conditionCompleted = 0;

                            //Check each condition for star descritpion
                            for (int o = 0; o < levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions; o++)
                            {
                                if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionComparator == 0)
                                {
                                    switch (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionType)
                                    {
                                        case CompleteConditionType.Score:
                                            if (levelsImplemented[i].level.levelProgression.maxScore > levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Combo:
                                            if (levelsImplemented[i].level.levelProgression.maxCombo > levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Timing:
                                            if (levelsImplemented[i].level.levelProgression.minTiming > levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt)
                                                conditionCompleted += 1;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionType)
                                    {
                                        case CompleteConditionType.Score:
                                            if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt > levelsImplemented[i].level.levelProgression.maxScore)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Combo:
                                            if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt > levelsImplemented[i].level.levelProgression.maxCombo)
                                                conditionCompleted += 1;
                                            break;

                                        case CompleteConditionType.Timing:
                                            if (levelsImplemented[i].level.levelProgression.conditionsToComplete[o].conditionReachedAt > levelsImplemented[i].level.levelProgression.minTiming)
                                                conditionCompleted += 1;
                                            break;
                                    }
                                }
                            }

                            if (conditionCompleted == levelsImplemented[i].level.levelProgression.numberOfAdditionalConditions)
                            {
                                level.fullStarsImages.SetActive(true);
                            }
                            else
                            {
                                level.doneImages.SetActive(true);
                            }
                        }
                        else
                        {
                            if (levelsImplemented[i].level.levelSpec.suddenDeath || levelsImplemented[i].level.levelSpec.mandatoryBounce || levelsImplemented[i].level.levelSpec.timeAttack)
                            {
                                level.exoticDoneImages.SetActive(true);
                            }
                            else
                            {
                                level.fullStarsImages.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if (levelsImplemented[i].level.levelSpec.suddenDeath || levelsImplemented[i].level.levelSpec.mandatoryBounce || levelsImplemented[i].level.levelSpec.timeAttack)
                        {
                            level.exoticUnlockImages.SetActive(true);
                        }
                        else
                        {
                            level.unlockImages.SetActive(true);
                        }

                    }
                }
            }


            //Stargate activation
            if (levelsImplemented[i].level.levelProgression.starsRequired > 0)
            {
                Vector2 levelPos = startPos;
                float levelBorderOffset = 0.4f;


                Vector2 leftLevelSide = levelPos + Vector2.left * levelBorderOffset;
                Vector2 leftBorder = new Vector2(leftLevelSide.x - panelLeft, 0);

                Vector2 rightLevelSide = levelPos + Vector2.right * levelBorderOffset;
                Vector2 rightBorder = new Vector2(panelRight - rightLevelSide.x, 0);

                StargateValues leftGate = new StargateValues();
                StargateValues rightGate = new StargateValues();


                if (leftBorder.x > levelBorderOffset)
                {
                    UILineRenderer leftLine = PoolManager.instance.SpawnFromPool("Stargate", Vector3.zero, Quaternion.identity).GetComponent<UILineRenderer>();
                    leftLine.gameObject.transform.SetParent(CampaignPanel.gameObject.transform);
                    //leftLine.gameObject.transform.parent = CampaignPanel.gameObject.transform;
                    leftLine.rectTransform.anchoredPosition3D = new Vector3(leftLevelSide.x, leftLevelSide.y, 0);
                    leftLine.Points[1] = new Vector2(-leftBorder.x, 0);
                    leftGate = leftLine.gameObject.GetComponent<StargateValues>();
                }

                if (rightBorder.x > levelBorderOffset)
                {
                    UILineRenderer rightLine = PoolManager.instance.SpawnFromPool("Stargate", Vector3.zero, Quaternion.identity).GetComponent<UILineRenderer>();
                    rightLine.gameObject.transform.SetParent(CampaignPanel.gameObject.transform);
                    //rightLine.gameObject.transform.parent = CampaignPanel.gameObject.transform;
                    rightLine.rectTransform.anchoredPosition3D = new Vector3(rightLevelSide.x, rightLevelSide.y, 0);
                    rightLine.Points[1] = rightBorder;
                    rightGate = rightLine.gameObject.GetComponent<StargateValues>();
                }

                //SetText
                if (leftBorder.x > rightBorder.x)
                {
                    leftGate.leftToRightGo[0].SetActive(true);

                    if (PlayerStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                        leftGate.leftToRightImage[0].sprite = unlockedStarSprite;
                    else
                        leftGate.leftToRightImage[0].sprite = lockedStarSprite;

                    leftGate.leftToRightText[0].text = levelsImplemented[i].level.levelProgression.starsRequired.ToString();
                }
                else
                {
                    rightGate.leftToRightGo[1].SetActive(true);

                    if (PlayerStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                        rightGate.leftToRightImage[1].sprite = unlockedStarSprite;
                    else
                        rightGate.leftToRightImage[1].sprite = lockedStarSprite;

                    rightGate.leftToRightText[1].text = levelsImplemented[i].level.levelProgression.starsRequired.ToString();
                }
            }
        }

        if (levelToPlay == null)
            SetUpPanelPositionAtStart();

        //Steam Achievements 
        if (BuildPlatformManager.Instance.targetBuildPlatform == TargetBuildPlatform.Steam)
        {
            if(SteamManager.Initialized)
                SteamAchievementsManager.instance.CheckAchievements();
        }
        else if(BuildPlatformManager.Instance.targetBuildPlatform == TargetBuildPlatform.Viveport)
        {
            if (Viveport.UserStats.IsReady(ViveportCallback) == 1)
                ViveportAchievementManager.instance.CheckAchievements();
        }
    }

    /// <summary>
    /// Set up campaign details in the side panel
    /// </summary>
    /// <param name="selectedLevel"></param>
    public void SetUpLevelRecapValues(LevelsScriptable selectedLevel, LevelButton button)
    {
        if (!sidePanel.activeSelf)
            sidePanel.SetActive(true);

        //Play "Disable" anim on the previous selected button (if there's one)
        DisableSelectedLevel();

        levelToPlay = selectedLevel;
        buttonSelected = button;

        if (selectedLevel.level.levelSpec.levelName != null)
            levelRecapValues.levelTitle.text = selectedLevel.level.levelSpec.levelName;
        else
            levelRecapValues.levelTitle.text = "NO NAME";

        if (selectedLevel.level.levelSpec.buttonName != null && selectedLevel.level.levelSpec.buttonName != "" && selectedLevel.level.levelSpec.buttonName != " ")
        {
            levelRecapValues.button.text = selectedLevel.level.levelSpec.buttonName;
        }
        else
        {
            if (selectedLevel.level.levelProgression.levelNumber < 10)
                levelRecapValues.button.text = "0" + selectedLevel.level.levelProgression.levelNumber.ToString();
            else
                levelRecapValues.button.text = selectedLevel.level.levelProgression.levelNumber.ToString();
        }


        //Deactivate Conditions
        for (int i = 0; i < levelRecapValues.conditionComparator.Length; i++)
        {
            //levelRecapValues.conditionComparator[i].transform.parent.gameObject.SetActive(false);

            levelRecapValues.stars[i + 1].gameObject.SetActive(false);
        }

        //Reactivate Conditions depending on the level
        if (!selectedLevel.level.levelSpec.suddenDeath && !selectedLevel.level.levelSpec.mandatoryBounce && !selectedLevel.level.levelSpec.timeAttack)
        {
            levelRecapValues.exoticCondition.text = "";
            levelRecapValues.exoticCondition.gameObject.SetActive(false);

            for (int i = 0; i < selectedLevel.level.levelProgression.numberOfAdditionalConditions; i++)
            {
                //levelRecapValues.conditionComparator[i].transform.parent.gameObject.SetActive(true);

                levelRecapValues.stars[i + 1].gameObject.SetActive(true);
            }
        }
        else
        {
            levelRecapValues.exoticCondition.gameObject.SetActive(true);

            if (selectedLevel.level.levelSpec.suddenDeath)
            {
                levelRecapValues.exoticCondition.text = levelRecapValues.suddenDeathTitle + "\n" + levelRecapValues.suddenDeathDescription;
            }
            else if (selectedLevel.level.levelSpec.mandatoryBounce)
            {
                levelRecapValues.exoticCondition.text = levelRecapValues.bounceModeTitle + "\n" + levelRecapValues.mandatoryBounceDescription;
            }
            else
            {
                levelRecapValues.exoticCondition.text = levelRecapValues.timeAttackTitle + "\n" + levelRecapValues.timeAttackDescriptionBeforeValue + " " + selectedLevel.level.levelSpec.timePerLayer + " " + levelRecapValues.timeAttackDescriptionAfterValue;
            }
        }


        //Set Up Condition if necessary
        if (selectedLevel.level.levelProgression.numberOfAdditionalConditions > 0 && !selectedLevel.level.levelSpec.suddenDeath && !selectedLevel.level.levelSpec.mandatoryBounce && !selectedLevel.level.levelSpec.timeAttack)
        {
            //if (selectedLevel.level.levelProgression.conditionsToComplete[0].conditionComparator == CompleteConditionComparator.Min)
            //    levelRecapValues.conditionComparator[0].text = ">";
            //else
            //    levelRecapValues.conditionComparator[0].text = "<";

            for (int h = selectedLevel.level.levelProgression.numberOfAdditionalConditions; h < levelRecapValues.conditionType.Length; h++)
            {
                levelRecapValues.conditionType[h].text = "";
                levelRecapValues.conditionReachedAt[h].text = "";
            }

            switch (selectedLevel.level.levelProgression.conditionsToComplete[0].conditionType)
            {
                case CompleteConditionType.Combo:
                    levelRecapValues.conditionType[0].text = comboConditionEntry;
                    break;

                case CompleteConditionType.Score:
                    levelRecapValues.conditionType[0].text = scoreConditionEntry;
                    break;

                case CompleteConditionType.Timing:
                    levelRecapValues.conditionType[0].text = timingConditionEntry;
                    break;
            }

            //levelRecapValues.conditionType[0].text = selectedLevel.level.levelProgression.conditionsToComplete[0].conditionType.ToString();

            if (selectedLevel.level.levelProgression.conditionsToComplete[0].conditionType == CompleteConditionType.Timing)
            {
                int conditionMinutes = (int)selectedLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt / 60;
                int conditionSeconds = (int)selectedLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt - (conditionMinutes * 60);

                if (conditionMinutes < 10)
                {
                    if (conditionSeconds < 10)
                    {
                        levelRecapValues.conditionReachedAt[0].text = ("0" + conditionMinutes + ":" + "0" + conditionSeconds);
                    }
                    else
                    {
                        levelRecapValues.conditionReachedAt[0].text = ("0" + conditionMinutes + ":" + conditionSeconds);
                    }
                }
                else
                {
                    if (conditionSeconds < 10)
                    {
                        levelRecapValues.conditionReachedAt[0].text = (conditionMinutes + ":" + "0" + conditionSeconds);
                    }
                    else
                    {
                        levelRecapValues.conditionReachedAt[0].text = (conditionMinutes + ":" + conditionSeconds);
                    }
                }


            }
            else
            {
                levelRecapValues.conditionReachedAt[0].text = selectedLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt.ToString();
            }
        }
        else
        {
            for (int h = 0; h < levelRecapValues.conditionType.Length; h++)
            {
                levelRecapValues.conditionType[h].text = "";
                levelRecapValues.conditionReachedAt[h].text = "";
            }
        }

        if (selectedLevel.level.levelProgression.numberOfAdditionalConditions > 1)
        {
            //if (selectedLevel.level.levelProgression.conditionsToComplete[1].conditionComparator == CompleteConditionComparator.Min)
            //    levelRecapValues.conditionComparator[1].text = ">";
            //else
            //    levelRecapValues.conditionComparator[1].text = "<";

            switch (selectedLevel.level.levelProgression.conditionsToComplete[1].conditionType)
            {
                case CompleteConditionType.Combo:
                    levelRecapValues.conditionType[1].text = comboConditionEntry;
                    break;

                case CompleteConditionType.Score:
                    levelRecapValues.conditionType[1].text = scoreConditionEntry;
                    break;

                case CompleteConditionType.Timing:
                    levelRecapValues.conditionType[1].text = timingConditionEntry;
                    break;
            }

            //levelRecapValues.conditionType[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionType.ToString();
            //levelRecapValues.conditionReachedAt[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt.ToString();

            if (selectedLevel.level.levelProgression.conditionsToComplete[1].conditionType == CompleteConditionType.Timing)
            {
                int conditionMinutes = (int)selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt / 60;
                int conditionSeconds = (int)selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt - (conditionMinutes * 60);

                if (conditionMinutes < 10)
                {
                    if (conditionSeconds < 10)
                    {
                        levelRecapValues.conditionReachedAt[1].text = ("0" + conditionMinutes + ":" + "0" + conditionSeconds);
                    }
                    else
                    {
                        levelRecapValues.conditionReachedAt[1].text = ("0" + conditionMinutes + ":" + conditionSeconds);
                    }
                }
                else
                {
                    if (conditionSeconds < 10)
                    {
                        levelRecapValues.conditionReachedAt[1].text = (conditionMinutes + ":" + "0" + conditionSeconds);
                    }
                    else
                    {
                        levelRecapValues.conditionReachedAt[1].text = (conditionMinutes + ":" + conditionSeconds);
                    }
                }


            }
            else
            {
                //levelRecapValues.conditionReachedAt[1].text = selectedLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt.ToString();
                levelRecapValues.conditionReachedAt[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt.ToString();
            }
        }


        //Best Time display
        int minutes = (int)selectedLevel.level.levelProgression.minTiming / 60;
        int seconds = (int)selectedLevel.level.levelProgression.minTiming - (minutes * 60);

        if (minutes < 10)
        {
            if (seconds < 10)
            {
                levelRecapValues.bestTime.text = ("0" + minutes + ":" + "0" + seconds);
            }
            else
            {
                levelRecapValues.bestTime.text = ("0" + minutes + ":" + seconds);
            }
        }
        else
        {
            if (seconds < 10)
            {
                levelRecapValues.bestTime.text = (minutes + ":" + "0" + seconds);
            }
            else
            {
                levelRecapValues.bestTime.text = (minutes + ":" + seconds);
            }
        }


        if (selectedLevel.level.levelProgression.maxScore == 0)
            levelRecapValues.highScore.text = "-";
        else
            levelRecapValues.highScore.text = selectedLevel.level.levelProgression.maxScore.ToString();

        if (selectedLevel.level.levelProgression.maxCombo == 0)
            levelRecapValues.bestCombo.text = "-";
        else
            levelRecapValues.bestCombo.text = selectedLevel.level.levelProgression.maxCombo.ToString();



        /////////////   STARS CONDITIONS CHECK  /////////////

        if (selectedLevel.level.levelProgression.isDone)
        {
            levelRecapValues.stars[0].sprite = unlockedStarSprite;

            for (int i = 0; i < selectedLevel.level.levelProgression.numberOfAdditionalConditions; i++)
            {
                if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
                {
                    switch (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (selectedLevel.level.levelProgression.maxScore > selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Combo:
                            if (selectedLevel.level.levelProgression.maxCombo > selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Timing:
                            if (selectedLevel.level.levelProgression.minTiming > selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
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
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt > selectedLevel.level.levelProgression.maxScore)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Combo:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt > selectedLevel.level.levelProgression.maxCombo)
                                levelRecapValues.stars[i + 1].sprite = unlockedStarSprite;
                            else
                                levelRecapValues.stars[i + 1].sprite = lockedStarSprite;
                            break;

                        case CompleteConditionType.Timing:
                            if (selectedLevel.level.levelProgression.conditionsToComplete[i].conditionReachedAt > selectedLevel.level.levelProgression.minTiming)
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
            int length = levelRecapValues.stars.Length - (2 - selectedLevel.level.levelProgression.numberOfAdditionalConditions);
            //Debug.Log("length : " + length);
            for (int i = 0; i < length; i++)
            {
                levelRecapValues.stars[i].sprite = lockedStarSprite;
            }

            levelRecapValues.bestCombo.text = "-";
            levelRecapValues.bestTime.text = "-";
            levelRecapValues.highScore.text = "-";
        }
    }

    /// <summary>
    /// Launch Level
    /// </summary>
    public void PlayLevel()
    {
        if (levelToPlay == null || isLevelLaunch)
        {
            Debug.LogError("NO LEVEL SELECTED");
            return;
        }

        isLevelLaunch = true;

        SetLastRecordedPanelIndex(GetPanelIndex(levelToPlay));
        CampaignLevel.instance.SelectLevel(levelToPlay);
    }

    /// <summary>
    /// Move Panel WITH ANIMATION
    /// </summary>
    private void MovingPanel()
    {
        if (CampaignPanel.anchoredPosition3D.y == nextPanelPosition)
        {
            isMoving = false;
            return;
        }

        //Moving panel
        CampaignPanel.anchoredPosition3D = Vector3.MoveTowards(CampaignPanel.anchoredPosition3D,
            new Vector3(CampaignPanel.anchoredPosition3D.x, nextPanelPosition, CampaignPanel.anchoredPosition3D.z), scrollingSpeed);
    }

    /// <summary>
    /// Check if there are any level left up or down
    /// </summary>
    private void CheckPanelIndex()
    {
        //Debug.Log("Last Index : " + lastIndex);
        if (panelIndex >= lastIndex)
        {
            panelIndex = lastIndex;
            upButton.SetActive(false);
        }

        if (panelIndex < lastIndex)
        {
            upButton.SetActive(true);
        }

        if (panelIndex == 0)
        {
            downButton.SetActive(false);
        }

        if (panelIndex > 0)
        {
            downButton.SetActive(true);
        }
    }

    /// <summary>
    /// Set panel position at panel index position
    /// </summary>
    /// <param name="panelPosIndex">index position</param>
    private void SetPanelPosition(int panelPosIndex)
    {
        panelIndex = panelPosIndex;
        //Debug.Log("Set at POS");

        CheckPanelIndex();

        //Debug.Log("panelIndex : " + panelIndex);
        nextPanelPosition = panelPositions[panelIndex];

        CampaignPanel.anchoredPosition3D = new Vector3(CampaignPanel.anchoredPosition3D.x, nextPanelPosition, CampaignPanel.anchoredPosition3D.z);
    }

    /// <summary>
    /// Move panel from current index to the next(1) or previous(-1) one 
    /// </summary>
    /// <param name="upOrDown"></param>
    public void MoveCampaignPanel(int upOrDown) //-1 or 1
    {
        int newIndex = panelIndex + upOrDown;

        if (newIndex >= 0 && newIndex <= lastIndex)
        {
            panelIndex = newIndex;
        }
        else
        {
            return;
        }

        CheckPanelIndex();

        nextPanelPosition = panelPositions[panelIndex];

        isMoving = true;
    }

    /// <summary>
    /// Move Campaign Panel to selected position index
    /// </summary>
    /// <param name="newPanelIndex"></param>
    public void MoveCampaignPanelTo(int newPanelIndex)
    {
        if (newPanelIndex != panelIndex && newPanelIndex <= lastIndex && newPanelIndex >= 0)
        {
            panelIndex = newPanelIndex;
            //Debug.Log("newPanelIndex : " + newPanelIndex);
        }
        else
        {
            return;
        }

        CheckPanelIndex();

        nextPanelPosition = panelPositions[panelIndex];

        isMoving = true;
    }

    /// <summary>
    /// Return the level with the highest position in the campaign
    /// </summary>
    /// <returns></returns>
    private LevelsScriptable GetHighestLevelInCampaign()
    {
        LevelsScriptable highest = levelsImplemented[0];

        for (int i = 1; i < levelsImplemented.Count; i++)
        {
            if (highest.level.levelProgression.levelPos.y > levelsImplemented[i].level.levelProgression.levelPos.y)
                highest = levelsImplemented[i];
        }

        //Debug.Log("highest level : " + highest);
        return highest;
    }

    public void DisableSelectedLevel()
    {
        if (buttonSelected == null)
            return;

        buttonSelected.gameObject.GetComponent<Animator>().Play("Disabled");
    }
}
