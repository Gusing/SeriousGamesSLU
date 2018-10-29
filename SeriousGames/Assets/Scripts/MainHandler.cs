using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHandler : MonoBehaviour {

    int currentDay;

    public Sprite[] spriteSlots;

    public SpriteRenderer[] slots;

    public GameObject player;

    public Text textDay;
    public Text textActionsLeft;

    public Text textTotalCO2;
    public Text textTotalContamination;

    public Text textGameOver;

    int[] slotOptions;

    int[] worldSlotStatus;

    public SpriteRenderer[] worldSlots;

    public GameObject barFoodFill;
    public GameObject barCO2Fill;
    public GameObject barContaminationFill;
    public GameObject barDeathFill;
    public GameObject barFoodMarker;
    public GameObject barWellfareMarker;

    Vector3 barFoodPosition;
    Vector3 barCO2Position;
    Vector3 barContaminationPosition;
    Vector3 barDeathPosition;
    Vector3 barFoodMarkerPosition;
    Vector3 barWellfareMarkerPosition;

    public int maxFood = 35;
    public int maxCO2 = 40;
    public int maxContamination = 24;
    public int maxDeaths = 15;

    int selectedBuilding;

    int currentFood;
    int requiredFood;
    int[] foodReqs;
    int currentFoodReqPosition;

    int currentCO2;
    int currentContamination;
    int currentDeaths;
    int currentWellfare;

    int totalCO2;
    int totalContamination;
    int maxTotalCO2 = 75;
    int maxTotalContamination = 40;

    int currentPosition;

    int currentActionsLeft;

    bool gameOver;
    public Text textNextYear;

    int currentNumberOfTrees;

    readonly int NOTHING = -1, COWFARM = 0, CHICKENFARM = 1, EGGFARM = 2, WEEDFARM = 3, CABBAGEFARM = 4, FOREST = 5, 
        COWHAPPY = 6, COWFOOD = 7, CHICKENHAPPY = 8, CHICKENFOOD = 9, EGGHAPPY = 10, EGGFOOD = 11, WEEDFOOD = 12, CABBAGEFOOD = 13;

    /* STATS
        Food req per day: 1, 2, 4, 6, 8, 9, 11, 13, 14, 16, 17, 19, 22, 24, 27
        
        Limits:
        Co2: Warning 30, 60, death 75
        Contamination: Warning 20, death 40
        Death: Warning 10, death 15

        Chicken farm:
        3 food, 2 co2
        Food: +2 food, -3 AW, -1 co2
        Happy: AW +1

        Cow farm:
        4 food, 3 co2
        Food: +1 food, -1 AW, +1 co2
        Happy: -1 food, +2 AW

        Egg farm:
        2 food, 1 co2
        Food: +1 food, -1 AW
        Happy: +2 AW

        Weed farm:
        1 food
        Food: +2 food, +3 Con

        Cabbage farm:
        1 food
        Food: +1 food, +2 Con
    */


    // Use this for initialization
    void Start()
    {
        slotOptions = new int[3];
        worldSlotStatus = new int[8];

        barFoodPosition = barFoodFill.transform.position;
        barCO2Position = barCO2Fill.transform.position;
        barContaminationPosition = barContaminationFill.transform.position;
        barDeathPosition = barDeathFill.transform.position;
        barFoodMarkerPosition = barFoodMarker.transform.position;
        barWellfareMarkerPosition = barWellfareMarker.transform.position;

        foodReqs = new int[] { 1, 2, 4, 6, 8, 9, 11, 13, 14, 16, 17, 19, 22, 24, 27 };

        InitGame();
	}

    void InitGame()
    {
        currentFood = 0;
        currentContamination = 0;
        currentCO2 = 0;
        currentDeaths = 0;
        totalCO2 = 0;
        totalContamination = 0;
        currentFoodReqPosition = -1;

        currentDay = 0;
        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            worldSlotStatus[i] = -1;
        }
        selectedBuilding = -1;
        currentPosition = 0;
        currentActionsLeft = 2;
        currentWellfare = 10;
        requiredFood = 0;

        for (int i = 0; i < 3; i++)
        {
            worldSlotStatus[Random.Range(0, 8)] = FOREST;
        }

        gameOver = false;
        textNextYear.text = "Next Year";
        textGameOver.text = "";

        NewDay();
        CheckChanges();
    }
	
	void Update ()
    {
        textDay.text = "Year " + currentDay;
        textActionsLeft.text = "Actions Left: " + currentActionsLeft;

        textTotalCO2.text = "Total CO2: " + totalCO2;
        textTotalContamination.text = "Total Contamination: " + totalContamination;

        // update number of trees
        currentNumberOfTrees = 0;
        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            if (worldSlotStatus[i] == FOREST) currentNumberOfTrees += 1;
        }

        // update bars
        barFoodFill.transform.localScale = new Vector3(((float)currentFood / (float)maxFood), 1f);
        barFoodFill.transform.position = barFoodPosition + new Vector3(-(1 - ((float)currentFood / (float)maxFood)) * 2.08f, 0);
        
        float tCurrentCO2 = currentCO2 - currentNumberOfTrees;
        tCurrentCO2 = Mathf.Clamp(tCurrentCO2, 0, 100);
        barCO2Fill.transform.localScale = new Vector3((tCurrentCO2 / (float)maxCO2), 1f);
        barCO2Fill.transform.position = barCO2Position + new Vector3(-(1 - (tCurrentCO2 / (float)maxCO2)) * 1.88f, 0);

        barContaminationFill.transform.localScale = new Vector3(((float)currentContamination / (float)maxContamination), 1f);
        barContaminationFill.transform.position = barContaminationPosition + new Vector3(-(1 - ((float)currentContamination / (float)maxContamination)) * 1.74f, 0);

        barDeathFill.transform.localScale = new Vector3(((float)currentDeaths / (float)maxDeaths), 1f);
        barDeathFill.transform.position = barDeathPosition + new Vector3(-(1 - ((float)currentDeaths / (float)maxDeaths)) * 2.08f, 0);

        // update markers
        barFoodMarker.transform.position = barFoodMarkerPosition + new Vector3(((float)requiredFood / (float)maxFood) * 4, 0);

        barWellfareMarker.transform.position = barWellfareMarkerPosition + new Vector3(((float)currentWellfare / (float)20) * 7, 0);
    }

    void CheckChanges()
    {
        if (currentPosition == 0)
        {
            player.transform.position = new Vector3(-2.7f, 3.49f);
            player.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (currentPosition == 1)
        {
            player.transform.position = new Vector3(-5.98f, 2.47f);
            player.transform.eulerAngles = new Vector3(0, 0, 45);
        }
        if (currentPosition == 2)
        {
            player.transform.position = new Vector3(-7f, -0.2f);
            player.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (currentPosition == 3)
        {
            player.transform.position = new Vector3(-5.86f, -3.14f);
            player.transform.eulerAngles = new Vector3(0, 0, 135);
        }
        if (currentPosition == 4)
        {
            player.transform.position = new Vector3(-2.81f, -4.51f);
            player.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (currentPosition == 5)
        {
            player.transform.position = new Vector3(0.26f, -3.5f);
            player.transform.eulerAngles = new Vector3(0, 0, 225);
        }
        if (currentPosition == 6)
        {
            player.transform.position = new Vector3(1.91f, -0.51f);
            player.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (currentPosition == 7)
        {
            player.transform.position = new Vector3(0.74f, 2.98f);
            player.transform.eulerAngles = new Vector3(0, 0, 315);
        }
        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            if (worldSlotStatus[i] == -1) worldSlots[i].enabled = false;
            if (worldSlotStatus[i] > -1 && worldSlotStatus[i] < 6)
            {
                worldSlots[i].sprite = spriteSlots[worldSlotStatus[i]];
                worldSlots[i].enabled = true;
            }
        }
    }

    public void NewDay()
    {
        if (gameOver)
        {
            InitGame();
            return;
        }

        for (int i = 0; i < slotOptions.Length; i++)
        {
            slotOptions[i] = Random.Range(0, 5);

            slots[i].sprite = spriteSlots[slotOptions[i]];
        }

        //update totals
        totalCO2 += Mathf.Clamp(currentCO2 - currentNumberOfTrees, 0, 100);
        totalContamination += currentContamination;

        if (currentFood < requiredFood)
        {
            currentDeaths += requiredFood - currentFood;
        }
        else
        {
            currentFoodReqPosition += 1;
            requiredFood = foodReqs[currentFoodReqPosition];
        }
        selectedBuilding = -1;

        //check game over
        if (currentDeaths >= maxDeaths)
        {
            currentDeaths = maxDeaths;
            textGameOver.text = "ALL IS DEAD";
            gameOver = true;
            textNextYear.text = "Restart";
        }
        else if (totalCO2 >= maxTotalCO2)
        {
            totalCO2 = maxTotalCO2;
            textGameOver.text = "ALL IS MELTED";
            gameOver = true;
            textNextYear.text = "Restart";
        }
        else if (totalContamination >= maxTotalContamination)
        {
            totalContamination = maxTotalContamination;
            textGameOver.text = "ALL IS CONTAMINATED";
            gameOver = true;
            textNextYear.text = "Restart";
        }
        else if (currentDay == 15)
        {
            textGameOver.text = "Game Complete. Score: " + Mathf.RoundToInt(((maxTotalContamination - totalContamination) + (maxTotalCO2 - totalCO2) + (maxDeaths - currentDeaths)) * ((float)currentWellfare / 10f));
            gameOver = true;
            textNextYear.text = "Restart";
        }

        currentDay++;
        currentActionsLeft = 2;
    }

    public void SelectBuilding(int slot)
    {
        if (gameOver) return;

        if (worldSlotStatus[currentPosition] != -1 || currentActionsLeft <= 0) return;

        selectedBuilding = slotOptions[slot];

        worldSlotStatus[currentPosition] = selectedBuilding;

        // apply stats from buldings
        if (selectedBuilding == COWFARM)
        {
            currentFood += 4;
            currentCO2 += 3;
        }
        if (selectedBuilding == CHICKENFARM)
        {
            currentFood += 3;
            currentCO2 += 2;
        }
        if (selectedBuilding == EGGFARM)
        {
            currentFood += 2;
            currentCO2 += 1;
        }
        if (selectedBuilding == WEEDFARM)
        {
            currentFood += 1;
        }
        if (selectedBuilding == CABBAGEFARM)
        {
            currentFood += 1;
        }

        currentActionsLeft--;

        CheckChanges();
    }

    public void SelectWorld(int slot)
    {
        if (gameOver) return;

        if (currentActionsLeft <= 0) return;

        if (slot != currentPosition)
        {
            if ((currentPosition == 0 && (slot == 7 || slot == 6)) ||
                (currentPosition == 1 && slot == 7) ||
                (currentPosition == 7 && (slot == 0 || slot == 1)) ||
                (currentPosition == 6 && slot == 0) ||
                (currentPosition + 2 >= slot && currentPosition - 2 <= slot))
            {
                currentPosition = slot;
                currentActionsLeft--;
                CheckChanges();

            }
        }
    }

    public void Upgrade(bool happy)
    {
        if (gameOver) return;

        if (worldSlotStatus[currentPosition] == COWFARM)
        {
            if (happy)
            {
                currentWellfare += 2;
                currentFood -= 1;
                worldSlotStatus[currentPosition] = COWHAPPY;
                currentActionsLeft -= 1;
            }
            else
            {
                currentWellfare -= 1;
                currentFood += 1;
                currentCO2 += 1;
                worldSlotStatus[currentPosition] = COWFOOD;
                currentActionsLeft -= 1;
            }
        }
        if (worldSlotStatus[currentPosition] == CHICKENFARM)
        {
            if (happy)
            {
                currentWellfare += 1;
                worldSlotStatus[currentPosition] = CHICKENHAPPY;
                currentActionsLeft -= 1;
            }
            else
            {
                currentWellfare -= 3;
                currentFood += 2;
                currentCO2 -= 1;
                worldSlotStatus[currentPosition] = CHICKENFOOD;
                currentActionsLeft -= 1;
            }
        }
        if (worldSlotStatus[currentPosition] == EGGFARM)
        {
            if (happy)
            {
                currentWellfare += 2;
                worldSlotStatus[currentPosition] = EGGHAPPY;
                currentActionsLeft -= 1;
            }
            else
            {
                currentWellfare -= 1;
                currentFood += 1;
                worldSlotStatus[currentPosition] = EGGFOOD;
                currentActionsLeft -= 1;
            }
        }
        if (worldSlotStatus[currentPosition] == WEEDFARM)
        {
            if (!happy)
            {
                currentContamination += 3;
                currentFood += 2;
                worldSlotStatus[currentPosition] = WEEDFOOD;
                currentActionsLeft -= 1;
            }
        }
        if (worldSlotStatus[currentPosition] == CABBAGEFARM)
        {
            if (!happy)
            {
                currentContamination += 2;
                currentFood += 1;
                worldSlotStatus[currentPosition] = CABBAGEFOOD;
                currentActionsLeft -= 1;
            }
        }

    }

    public void Trash()
    {
        if (gameOver) return;

        if (!(worldSlotStatus[currentPosition] == NOTHING))
        {
            if (worldSlotStatus[currentPosition] == COWFARM || worldSlotStatus[currentPosition] == COWHAPPY || worldSlotStatus[currentPosition] == COWFOOD)
            {
                currentFood -= 4;
                currentCO2 -= 3;
            }
            if (worldSlotStatus[currentPosition] == CHICKENFARM || worldSlotStatus[currentPosition] == CHICKENFOOD || worldSlotStatus[currentPosition] == CHICKENHAPPY)
            {
                currentFood -= 3;
                currentCO2 -= 2;
            }
            if (worldSlotStatus[currentPosition] == EGGFARM || worldSlotStatus[currentPosition] == EGGFOOD || worldSlotStatus[currentPosition] == EGGHAPPY)
            {
                currentFood -= 2;
                currentCO2 -= 1;
            }
            if (worldSlotStatus[currentPosition] == WEEDFARM || worldSlotStatus[currentPosition] == WEEDFOOD)
            {
                currentFood -= 1;
            }
            if (worldSlotStatus[currentPosition] == CABBAGEFARM || worldSlotStatus[currentPosition] == CABBAGEFOOD)
            {
                currentFood -= 1;
            }
            if (worldSlotStatus[currentPosition] == COWHAPPY)
            {
                currentWellfare -= 2;
                currentFood += 1;
            }
            if (worldSlotStatus[currentPosition] == COWFOOD)
            {
                currentWellfare += 1;
                currentFood -= 1;
                currentCO2 -= 1;
            }
            if (worldSlotStatus[currentPosition] == CHICKENHAPPY)
            {
                currentWellfare -= 1;
            }
            if (worldSlotStatus[currentPosition] == CHICKENFOOD)
            {
                currentWellfare += 3;
                currentCO2 += 1;
                currentFood -= 2;
            }
            if (worldSlotStatus[currentPosition] == EGGHAPPY)
            {
                currentWellfare -= 2;
            }
            if (worldSlotStatus[currentPosition] == EGGFOOD)
            {
                currentWellfare += 1;
                currentFood -= 1;
            }
            if (worldSlotStatus[currentPosition] == WEEDFOOD)
            {
                currentContamination -= 3;
                currentFood -= 2;
            }
            if (worldSlotStatus[currentPosition] == CABBAGEFOOD)
            {
                currentContamination -= 2;
                currentFood -= 1;
            }

            worldSlotStatus[currentPosition] = NOTHING;
            currentActionsLeft -= 1;
            CheckChanges();
        }
    }
}
