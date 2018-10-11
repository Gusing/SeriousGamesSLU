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

    public int maxFood = 100;
    public int maxCO2 = 100;
    public int maxContamination = 100;
    public int maxDeaths = 100;

    int selectedBuilding;

    int currentFood;
    int requiredFood;
    int currentCO2;
    int currentContamination;
    int currentDeaths;
    int currentWellfare;

    int totalCO2;
    int totalContamination;

    int currentPosition;

    int currentActionsLeft;

    int COWFARM = 0, CHICKENFARM = 1, EGGFARM = 2, WEEDFARM = 3, CABBAGEFARM = 4, FOREST = 5;
    
    // Use this for initialization
    void Start ()
    {
        slotOptions = new int[3];
        worldSlotStatus = new int[8];

        barFoodPosition = barFoodFill.transform.position;
        barCO2Position = barCO2Fill.transform.position;
        barContaminationPosition = barContaminationFill.transform.position;
        barDeathPosition = barDeathFill.transform.position;
        barFoodMarkerPosition = barFoodMarker.transform.position;
        barWellfareMarkerPosition = barWellfareMarker.transform.position;

        InitGame();
	}

    void InitGame()
    {
        currentDay = 0;
        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            worldSlotStatus[i] = -1;
        }
        selectedBuilding = -1;
        currentPosition = 0;
        currentActionsLeft = 2;
        currentWellfare = 10;
        requiredFood = 1;

        for (int i = 0; i < 3; i++)
        {
            worldSlotStatus[Random.Range(0, 8)] = FOREST;
        }

        NewDay();
        CheckChanges();
    }
	
	// Update is called once per frame
	void Update ()
    {
        textDay.text = "Day " + currentDay;
        textActionsLeft.text = "Actions Left: " + currentActionsLeft;

        textTotalCO2.text = "Total CO2: " + totalCO2;
        textTotalContamination.text = "Total Contamination: " + totalContamination;

        // update bars
        barFoodFill.transform.localScale = new Vector3(((float)currentFood / (float)maxFood), 1f);
        barFoodFill.transform.position = barFoodPosition + new Vector3(-(1 - ((float)currentFood / (float)maxFood)) * 2.08f, 0);

        barCO2Fill.transform.localScale = new Vector3(((float)currentCO2 / (float)maxCO2), 1f);
        barCO2Fill.transform.position = barCO2Position + new Vector3(-(1 - ((float)currentCO2 / (float)maxCO2)) * 1.88f, 0);

        barContaminationFill.transform.localScale = new Vector3(((float)currentContamination / (float)maxContamination), 1f);
        barContaminationFill.transform.position = barContaminationPosition + new Vector3(-(1 - ((float)currentContamination / (float)maxContamination)) * 1.28f, 0);

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
            player.transform.position = new Vector3(-1.96f, 2.26f);
            player.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (currentPosition == 1)
        {
            player.transform.position = new Vector3(-4.74f, 1.46f);
            player.transform.eulerAngles = new Vector3(0, 0, 45);
        }
        if (currentPosition == 2)
        {
            player.transform.position = new Vector3(-5.99f, -0.94f);
            player.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (currentPosition == 3)
        {
            player.transform.position = new Vector3(-4.99f, -3.56f);
            player.transform.eulerAngles = new Vector3(0, 0, 135);
        }
        if (currentPosition == 4)
        {
            player.transform.position = new Vector3(-1.96f, -4.74f);
            player.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (currentPosition == 5)
        {
            player.transform.position = new Vector3(1.07f, -3.95f);
            player.transform.eulerAngles = new Vector3(0, 0, 225);
        }
        if (currentPosition == 6)
        {
            player.transform.position = new Vector3(2.15f, -1.1f);
            player.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (currentPosition == 7)
        {
            player.transform.position = new Vector3(1.07f, 1.87f);
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
        for (int i = 0; i < slotOptions.Length; i++)
        {
            slotOptions[i] = Random.Range(0, 5);

            slots[i].sprite = spriteSlots[slotOptions[i]];
        }

        //update totals
        totalCO2 += currentCO2;
        totalContamination += currentContamination;

        if (currentFood < requiredFood)
        {
            currentDeaths += requiredFood - currentFood;
        }

        requiredFood += 2;

        currentDay++;
        selectedBuilding = -1;
        currentActionsLeft = 2;
    }

    public void SelectBuilding(int slot)
    {
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
}
