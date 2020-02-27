using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;

public class Campaign : MonoBehaviour
{
    [Header("Levels")]
    public List<LevelsScriptable> levelsToCheck;

    [Header("Panel")]
    public RectTransform CampaignPanel;
    public List<LevelsScriptable> levelsImplemented;
    public LevelRecap levelRecapValues;

    private int numberOfPanelPositions = 34;
    private float positionQuotient = 0;
    [SerializeField] private float[] panelPositions = new float[0];
    private int panelIndex = -1;
    private float nextPanelPosition = 0;
    private float panelTop = 0.5f;
    private float panelBottom = 49.3f;
    private float panelLeft = -0.3f;
    private float panelRight = 3.3f;
    private bool isMoving;
    [Range(0.01f, 3f)]public float scrollingSpeed;

    private LevelsScriptable levelToPlay;

    [Header("Stars")]
    public Sprite lockedStarSprite;
    public Sprite unlockedStarSprite;
    [HideInInspector] public int totalOfStars;
    public TextMeshProUGUI starCounter;

    [Header("Side Panel")]
    public GameObject sidePanel;

    bool isLevelLaunch;


    public static Campaign instance;




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetUpCampaign();
        isLevelLaunch = false;
    }

    private void Update()
    {
        if (isMoving)
            MovingPanel();
    }


    /// <summary>
    /// Set up Panel positions
    /// </summary>
    private void SetUpPanelPositions()
    {
        panelPositions = new float[numberOfPanelPositions + 1];

        positionQuotient = (panelBottom - panelTop) / numberOfPanelPositions;


        for (int i = 0; i < numberOfPanelPositions + 1; i++)
        {
            panelPositions[i] = panelBottom - (positionQuotient * i);
        }
    }

    /// <summary>
    /// Check if the level is implemented in the campaign
    /// </summary>
    private void Check4ImplementedLevels()
    {
        levelsImplemented = new List<LevelsScriptable>();
        totalOfStars = 0;

        for (int i = 0; i < levelsToCheck.Count; i++)
        {
            if (levelsToCheck[i].level.levelProgression.isImplemented)
            {
                levelsImplemented.Add(levelsToCheck[i]);
                //levelsToCheck[i].level.levelProgression.LevelIndex = i;

                CountingStars(levelsToCheck[i]);
            }
        }

        starCounter.text = totalOfStars.ToString();

        ReorderCampaign();
    }

    /// <summary>
    /// Count the number of stars
    /// </summary>
    /// <param name="level"></param>
    private void CountingStars(LevelsScriptable level)
    {
        if (level.level.levelProgression.isDone)
        {
            totalOfStars += 1;

            for (int i = 0; i < level.level.levelProgression.numberOfAdditionalConditions; i++)
            {
                if (level.level.levelProgression.conditionsToComplete[i].conditionComparator == 0)
                {
                    switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (level.level.levelProgression.maxScore < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                totalOfStars += 1;
                            break;

                        case CompleteConditionType.Combo:
                            if (level.level.levelProgression.maxCombo < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                totalOfStars += 1;
                            break;

                        case CompleteConditionType.Timing:
                            if (level.level.levelProgression.minTiming < level.level.levelProgression.conditionsToComplete[i].conditionReachedAt)
                                totalOfStars += 1;
                            break;
                    }
                }
                else
                {
                    switch (level.level.levelProgression.conditionsToComplete[i].conditionType)
                    {
                        case CompleteConditionType.Score:
                            if (level.level.levelProgression.conditionsToComplete[i].conditionReachedAt < level.level.levelProgression.maxScore)
                                totalOfStars += 1;
                            break;

                        case CompleteConditionType.Combo:
                            if (level.level.levelProgression.conditionsToComplete[i].conditionReachedAt < level.level.levelProgression.maxCombo)
                                totalOfStars += 1;
                            break;

                        case CompleteConditionType.Timing:
                            if (level.level.levelProgression.conditionsToComplete[i].conditionReachedAt < level.level.levelProgression.minTiming)
                                totalOfStars += 1;
                            break;
                    }
                }
            }

        }
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
    /// Setup panel position to the first level unlocked from the top position
    /// </summary>
    private void SetUpPanelPositionAtStart()
    {
        MoveCampaignPanelTo(numberOfPanelPositions);

        for (int i = 0; i < levelsImplemented.Count; i++)
        {
            if (levelsImplemented[i].level.levelProgression.isUnlocked)
            {
                float levelComparer = (((levelsImplemented[i].level.levelProgression.levelPos.y * 0.5f)) * -0.01f) + positionQuotient;
                //Debug.Log("Level : " + levelsImplemented[i]);
                //Debug.Log("levelComparer : " + levelComparer);
                //Debug.Log("positionQuotient : " + positionQuotient);


                for (int y = numberOfPanelPositions; y > -1; y--)
                {
                    float comparer = (positionQuotient * y) * -1;
                    //Debug.Log("before y : " + y);

                    if (levelComparer <= comparer)
                    {
                        //Debug.Log("panel position : " + comparer);
                        //Debug.Log("level position : " + levelComparer);

                        int finalIndex = numberOfPanelPositions - (y + 1);
                        //Debug.Log("panel index : " + finalIndex);
                        //Debug.Log("y : " + y);

                        MoveCampaignPanelTo(finalIndex);
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set up campaign tree
    /// </summary>
    private void SetUpCampaign()
    {
        //Set up notching 4 panel Move
        SetUpPanelPositions();

        //Set Up the number of stars
        Check4ImplementedLevels();

        JSON.instance.SetUpDATAs();

        //Set Up Graphic Elements
        for (int i = 0; i < levelsImplemented.Count; i++)
        {
            //Spawn Level ICON 
            LevelButton level = PoolManager.instance.SpawnFromPool("LevelButton", Vector3.zero, Quaternion.identity).GetComponent<LevelButton>();
            level.transform.parent = CampaignPanel.transform;

            //Transpose editor position into campaign position
            float xPos = (levelsImplemented[i].level.levelProgression.levelPos.x * 0.5f) * 0.01f;
            float yPos = ((levelsImplemented[i].level.levelProgression.levelPos.y * 0.5f)) * -0.01f;

            Vector2 startPos = new Vector2(xPos, yPos);

            //Set LEVEL icon position
            level.rectTransform.anchoredPosition3D = new Vector3(xPos, yPos, 0);

            //Set Level Title
            level.text.text = levelsImplemented[i].level.levelProgression.buttonName;

            //Set onClick Event to reload values on the Level Panel
            LevelsScriptable lvl = levelsImplemented[i];
            level.button.onClick.AddListener(() => SetUpLevelRecapValues(lvl));


            for (int y = 0; y < levelsImplemented[i].level.levelProgression.unlockConditions.Count; y++)
            {
                ////////////    DRAW Line renderer for CONNECTION   ////////////

                //Spawn a LineRenderer from the Pool
                UILineRenderer line = PoolManager.instance.SpawnFromPool("Connection", Vector3.zero, Quaternion.identity).GetComponent<UILineRenderer>();
                RectTransform rect = line.gameObject.GetComponent<RectTransform>();

                line.transform.parent = CampaignPanel.transform;

                //rect.sizeDelta = new Vector2(0, 0);



                //Get concerned level pos
                float xUPos = (levelsImplemented[i].level.levelProgression.unlockConditions[y].level.levelProgression.levelPos.x * 0.5f) * 0.01f;
                float yUPos = ((levelsImplemented[i].level.levelProgression.unlockConditions[y].level.levelProgression.levelPos.y * 0.5f)) * -0.01f;

                Vector2 lastPos = new Vector2(xUPos, yUPos);

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


                ////////////    CHECK UN/LOCK CONDITIONS    ////////////

                if (!levelsImplemented[i].level.levelProgression.isUnlocked)
                {
                    Debug.Log("LOCKED level : " + levelsImplemented[i]);

                    line.color = new Color32((byte)150, (byte)150, (byte)150, (byte)255);
                    level.button.interactable = false;

                    for (int x = 0; x < level.lockImages.Count; x++)
                    {
                        level.lockImages[x].SetActive(true);
                        level.unlockImages[x].SetActive(false);
                    }

                    if (levelsImplemented[i].level.levelProgression.unlockConditions[y].level.levelProgression.isDone && totalOfStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                    {
                        Debug.Log("UNLEASH the level : " + levelsImplemented[i]);
                        levelsImplemented[i].level.levelProgression.isUnlocked = true;
                        level.button.interactable = true;
                        line.color = new Color32((byte)255, (byte)255, (byte)255, (byte)255);


                        for (int x = 0; x < level.lockImages.Count; x++)
                        {
                            level.lockImages[x].SetActive(false);
                            level.unlockImages[x].SetActive(true);
                        }
                    }
                }
                else
                {
                    Debug.Log("UNLOCKED level : " + levelsImplemented[i]);

                    line.color = new Color32((byte)255, (byte)255, (byte)255, (byte)255);
                    level.button.interactable = true;

                    for (int x = 0; x < level.lockImages.Count; x++)
                    {
                        level.lockImages[x].SetActive(false);
                        level.unlockImages[x].SetActive(true);
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
                    leftLine.gameObject.transform.parent = CampaignPanel.gameObject.transform;
                    leftLine.rectTransform.anchoredPosition3D = new Vector3(leftLevelSide.x, leftLevelSide.y, 0);
                    leftLine.Points[1] = new Vector2(-leftBorder.x, 0);
                    leftGate = leftLine.gameObject.GetComponent<StargateValues>();
                }

                if (rightBorder.x > levelBorderOffset)
                {
                    UILineRenderer rightLine = PoolManager.instance.SpawnFromPool("Stargate", Vector3.zero, Quaternion.identity).GetComponent<UILineRenderer>();
                    rightLine.gameObject.transform.parent = CampaignPanel.gameObject.transform;
                    rightLine.rectTransform.anchoredPosition3D = new Vector3(rightLevelSide.x, rightLevelSide.y, 0);
                    rightLine.Points[1] = rightBorder;
                    rightGate = rightLine.gameObject.GetComponent<StargateValues>();
                }

                //SetText
                if (leftBorder.x > rightBorder.x)
                {
                    leftGate.leftToRightGo[0].SetActive(true);

                    if (totalOfStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                        leftGate.leftToRightImage[0].sprite = unlockedStarSprite;
                    else
                        leftGate.leftToRightImage[0].sprite = lockedStarSprite;

                    leftGate.leftToRightText[0].text = levelsImplemented[i].level.levelProgression.starsRequired.ToString();
                }
                else
                {
                    rightGate.leftToRightGo[1].SetActive(true);

                    if (totalOfStars >= levelsImplemented[i].level.levelProgression.starsRequired)
                        rightGate.leftToRightImage[1].sprite = unlockedStarSprite;
                    else
                        rightGate.leftToRightImage[1].sprite = lockedStarSprite;

                    rightGate.leftToRightText[1].text = levelsImplemented[i].level.levelProgression.starsRequired.ToString();
                }
            }
        }

        SetUpPanelPositionAtStart();
    }

    /// <summary>
    /// Set up campaign details in the side panel
    /// </summary>
    /// <param name="selectedLevel"></param>
    public void SetUpLevelRecapValues(LevelsScriptable selectedLevel)
    {
        if (!sidePanel.activeSelf)
            sidePanel.SetActive(true);

        levelToPlay = selectedLevel;

        if (selectedLevel.level.levelSpec.levelName != null)
            levelRecapValues.levelTitle.text = selectedLevel.level.levelSpec.levelName;
        else
            levelRecapValues.levelTitle.text = selectedLevel.level.levelProgression.buttonName + "NO NAME";

        //Deactivate Conditions
        for (int i = 0; i < levelRecapValues.conditionComparator.Length; i++)
        {
            levelRecapValues.conditionComparator[i].transform.parent.gameObject.SetActive(false);

            levelRecapValues.stars[i + 1].gameObject.SetActive(false);
        }

        //Reactivate Conditions depending on the level
        for (int i = 0; i < selectedLevel.level.levelProgression.numberOfAdditionalConditions; i++)
        {
            levelRecapValues.conditionComparator[i].transform.parent.gameObject.SetActive(true);

            levelRecapValues.stars[i + 1].gameObject.SetActive(true);
        }

        //Set Up COndition if necessary
        if (selectedLevel.level.levelProgression.numberOfAdditionalConditions > 0)
        {
            levelRecapValues.conditionComparator[0].text = selectedLevel.level.levelProgression.conditionsToComplete[0].conditionComparator.ToString();
            levelRecapValues.conditionType[0].text = selectedLevel.level.levelProgression.conditionsToComplete[0].conditionType.ToString();
            levelRecapValues.conditionReachedAt[0].text = selectedLevel.level.levelProgression.conditionsToComplete[0].conditionReachedAt.ToString();
        }

        if (selectedLevel.level.levelProgression.numberOfAdditionalConditions > 1)
        {
            levelRecapValues.conditionComparator[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionComparator.ToString();
            levelRecapValues.conditionType[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionType.ToString();
            levelRecapValues.conditionReachedAt[1].text = selectedLevel.level.levelProgression.conditionsToComplete[1].conditionReachedAt.ToString();
        }


        if (selectedLevel.level.levelProgression.minTiming == 0)
            levelRecapValues.bestTime.text = "-";
        else
            levelRecapValues.bestTime.text = selectedLevel.level.levelProgression.minTiming.ToString();

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
            int length = levelRecapValues.stars.Length - (2 - selectedLevel.level.levelProgression.numberOfAdditionalConditions);
            Debug.Log("length : " + length);
            for (int i = 0; i < length; i++)
            {
                levelRecapValues.stars[i].sprite = lockedStarSprite;
            }

            levelRecapValues.bestCombo.text = "-";
            levelRecapValues.bestTime.text = "-";
            levelRecapValues.highScore.text = "-";
        }
    }

    public void PlayLevel()
    {
        if (levelToPlay == null || isLevelLaunch)
        {
            Debug.LogError("NO LEVEL SELECTED");
            return;
        }

        isLevelLaunch = true;

        int indexOfLevel = levelToPlay.level.levelProgression.LevelIndex;
        Debug.Log(indexOfLevel + " level name : " + levelToPlay);
        CampaignLevel.Instance.SelectLevel(levelToPlay);
    }

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
    /// Move panel from current index to the next(1) or previous(-1) one 
    /// </summary>
    /// <param name="upOrDown"></param>
    public void MoveCampaignPanel(int upOrDown) //-1 or 1
    {
        int newIndex = panelIndex + upOrDown;

        if (newIndex >= 0 && newIndex <= numberOfPanelPositions)
        {
            panelIndex = newIndex;
        }
        else
        {
            return;
        }

        nextPanelPosition = panelPositions[panelIndex];

        isMoving = true;
    }

    /// <summary>
    /// Move Campaign Panel to selected position index
    /// </summary>
    /// <param name="newPanelIndex"></param>
    public void MoveCampaignPanelTo(int newPanelIndex)
    {
        if (newPanelIndex != panelIndex && newPanelIndex <= numberOfPanelPositions && newPanelIndex >= 0)
        {
            panelIndex = newPanelIndex;
        }
        else
        {
            return;
        }

        nextPanelPosition = panelPositions[panelIndex];

        isMoving = true;
    }
}
