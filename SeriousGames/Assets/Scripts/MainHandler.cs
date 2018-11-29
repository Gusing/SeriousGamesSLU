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

    public SpriteRenderer[] worldLandSlots;
    public Sprite[] spriteLandSlots;
    int[] landSlots;
    public Sprite[] spriteIces;
    public SpriteRenderer ice;
    public Sprite[] spriteGlobes;
    public SpriteRenderer globe;

    int stateCO2;
    int stateContamination;

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

    public GameObject btnUpgradeFood;
    public GameObject btnUpgradeHappy;
    public GameObject btnTrash;

    public int maxFood = 35;
    public int maxCO2 = 40;
    public int maxContamination = 24;
    public int maxDeaths = 15;

    int selectedBuilding;

    int currentFood;
    int requiredFood;
    int[] foodReqs;
    int currentFoodReqPosition;
    int extraFood;

    int currentCO2;
    int currentContamination;
    int currentDeaths;
    int currentWellfare;

    int totalCO2;
    int totalContamination;
    public int maxTotalCO2 = 75;
    public int maxTotalContamination = 50;

    int currentPosition;

    int currentActionsLeft;

    bool gameOver;
	bool showingIntro;
    public Text textNextYear;

    public Sprite spriteIntro;
    public Sprite spriteEndNormal;
    public Sprite spriteEndDeath;
    public Sprite spriteEndFlood;
    public Sprite spriteEndContaminated;
    public SpriteRenderer popup;

    int currentNumberOfTrees;

	public ParticleSystem psCO2Build;
	public ParticleSystem psContaminationBuild;
	public ParticleSystem psCO2Loop;
	public ParticleSystem psContaminationLoop;
	public ParticleSystem psCow;
	public ParticleSystem psChicken;
	public ParticleSystem psHeart;
	public ParticleSystem psSkull;
	public ParticleSystem psWellfareHappy;
	public ParticleSystem psWellfareSad;

    // audio events
    FMOD.Studio.EventInstance eventMenuCancel;
    FMOD.Studio.EventInstance eventMenuConfirm;
    FMOD.Studio.EventInstance eventMenuNavigate;
    FMOD.Studio.EventInstance eventAmbience;
    FMOD.Studio.EventInstance eventBuildingPlacement;
    FMOD.Studio.EventInstance eventMainTheme;

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
        landSlots = new int[8];

        barFoodPosition = barFoodFill.transform.position;
        barCO2Position = barCO2Fill.transform.position;
        barContaminationPosition = barContaminationFill.transform.position;
        barDeathPosition = barDeathFill.transform.position;
        barFoodMarkerPosition = barFoodMarker.transform.position;
        barWellfareMarkerPosition = barWellfareMarker.transform.position;

        foodReqs = new int[] { 1, 2, 4, 6, 8, 9, 11, 13, 14, 16, 17, 19, 22, 24, 27, 29 };
        
        eventMainTheme = FMODUnity.RuntimeManager.CreateInstance("event:/Music/MainTheme");
        eventMenuCancel = FMODUnity.RuntimeManager.CreateInstance("event:/Sounds/Interface/MenuCancel");
        eventMenuConfirm = FMODUnity.RuntimeManager.CreateInstance("event:/Sounds/Interface/MenuConfirm");
        eventMenuNavigate = FMODUnity.RuntimeManager.CreateInstance("event:/Sounds/Interface/MenuNavigate");
        eventAmbience = FMODUnity.RuntimeManager.CreateInstance("event:/Sounds/Zone/Ambience");
        eventBuildingPlacement = FMODUnity.RuntimeManager.CreateInstance("event:/Sounds/Effect/BuildingPlacement");

        InitGame();

		showingIntro = true;
        gameOver = true;

		popup.sprite = spriteIntro;
		popup.enabled = true;
		textNextYear.text = "Börja";
	}

    void InitGame()
    {
        stateCO2 = 0;
        stateContamination = 0;

        ice.sprite = spriteIces[0];
        globe.sprite = spriteGlobes[0];
        currentFood = 0;
        extraFood = 0;
        currentContamination = 0;
        currentCO2 = 0;
        currentDeaths = 0;
        totalCO2 = 0;
        totalContamination = 0;
        currentFoodReqPosition = -1;
        currentDay = 0;
        popup.enabled = false;

        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            worldSlotStatus[i] = -1;
        }

        // randomize land slots
        for (int i = 0; i < worldLandSlots.Length; i++)
        {
            int temp = Random.Range(0, 8);
            landSlots[i] = temp;
            worldLandSlots[i].sprite = spriteLandSlots[temp];
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
        textNextYear.text = "Nästa År";
        textGameOver.text = "";

        eventMainTheme.start();
        eventMainTheme.setParameterValue("Start", 1);
        eventAmbience.start();

        NewDay();
        CheckChanges();
    }
	
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

		textDay.text = "År " + (2018 + currentDay);
        textActionsLeft.text = "Handlingar kvar: " + currentActionsLeft;

        textTotalCO2.text = "Total CO2: " + totalCO2;
        textTotalContamination.text = "Total Contamination: " + totalContamination;

        eventMainTheme.setParameterValue("Contamination", totalContamination);
        eventMainTheme.setParameterValue("Co2", totalCO2);

        // update number of trees
        currentNumberOfTrees = 0;
        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            if (worldSlotStatus[i] == FOREST) currentNumberOfTrees += 1;
        }

        if ((worldSlotStatus[currentPosition] == COWFARM ||
            worldSlotStatus[currentPosition] == CHICKENFARM ||
            worldSlotStatus[currentPosition] == EGGFARM ||
            worldSlotStatus[currentPosition] == CABBAGEFARM ||
            worldSlotStatus[currentPosition] == WEEDFARM) &&
			currentActionsLeft > 0 && !gameOver && !showingIntro)
        {
            btnUpgradeFood.SetActive(true);
        }
        else
        {
            btnUpgradeFood.SetActive(false);
        }

        if ((worldSlotStatus[currentPosition] == COWFARM ||
            worldSlotStatus[currentPosition] == CHICKENFARM ||
            worldSlotStatus[currentPosition] == EGGFARM) &&
			currentActionsLeft > 0 && !gameOver && !showingIntro)
        {
            btnUpgradeHappy.SetActive(true);
        }
        else
        {
            btnUpgradeHappy.SetActive(false);
        }

        if (worldSlotStatus[currentPosition] != NOTHING &&
			currentActionsLeft > 0 && !gameOver && !showingIntro)
        {
            btnTrash.SetActive(true);
        }
        else btnTrash.SetActive(false);

        // update bars
        barFoodFill.transform.localScale = new Vector3(((float)currentFood / (float)maxFood), 1f);
        barFoodFill.transform.position = barFoodPosition + new Vector3(-(1 - ((float)currentFood / (float)maxFood)) * 1.52f, 0);
        
        float tCurrentCO2 = currentCO2 - currentNumberOfTrees;
        tCurrentCO2 = Mathf.Clamp(tCurrentCO2, 0, 100);
        barCO2Fill.transform.localScale = new Vector3((tCurrentCO2 / (float)maxCO2), 1f);
        barCO2Fill.transform.position = barCO2Position + new Vector3(-(1 - (tCurrentCO2 / (float)maxCO2)) * 1.52f, 0);

        barContaminationFill.transform.localScale = new Vector3(((float)currentContamination / (float)maxContamination), 1f);
        barContaminationFill.transform.position = barContaminationPosition + new Vector3(-(1 - ((float)currentContamination / (float)maxContamination)) * 1.52f, 0);

        barDeathFill.transform.localScale = new Vector3(((float)currentDeaths / (float)maxDeaths), 1f);
        barDeathFill.transform.position = barDeathPosition + new Vector3(-(1 - ((float)currentDeaths / (float)maxDeaths)) * 1.78f, 0);

        // update markers
        barFoodMarker.transform.position = barFoodMarkerPosition + new Vector3(((float)requiredFood / (float)maxFood) * 3.04f, 0);

        barWellfareMarker.transform.position = barWellfareMarkerPosition + new Vector3(((float)currentWellfare / (float)20) * 3.04f, 0);
    }

    void CheckChanges()
    {
        // + 2.61
        if (currentPosition == 0)
        {
            player.transform.position = new Vector3(-0.09f, 3.49f);
            player.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (currentPosition == 1)
        {
            player.transform.position = new Vector3(-3.37f, 2.47f);
            player.transform.eulerAngles = new Vector3(0, 0, 45);
        }
        if (currentPosition == 2)
        {
            player.transform.position = new Vector3(-4.39f, -0.2f);
            player.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        if (currentPosition == 3)
        {
            player.transform.position = new Vector3(-3.25f, -3.14f);
            player.transform.eulerAngles = new Vector3(0, 0, 135);
        }
        if (currentPosition == 4)
        {
            player.transform.position = new Vector3(-0.2f, -4.51f);
            player.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (currentPosition == 5)
        {
            player.transform.position = new Vector3(2.87f, -3.5f);
            player.transform.eulerAngles = new Vector3(0, 0, 225);
        }
        if (currentPosition == 6)
        {
            player.transform.position = new Vector3(4.52f, -0.51f);
            player.transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (currentPosition == 7)
        {
            player.transform.position = new Vector3(3.35f, 2.98f);
            player.transform.eulerAngles = new Vector3(0, 0, 315);
        }
        for (int i = 0; i < worldSlotStatus.Length; i++)
        {
            if (worldSlotStatus[i] == -1) worldSlots[i].enabled = false;
            if (worldSlotStatus[i] > -1 && worldSlotStatus[i] < 14)
            {
                worldSlots[i].sprite = spriteSlots[worldSlotStatus[i]];
                worldSlots[i].enabled = true;
            }
        }
        btnUpgradeFood.transform.position = player.transform.position + new Vector3(-1.1f, 1.2f);
        btnUpgradeHappy.transform.position = player.transform.position + new Vector3(1.1f, 1.2f);
        btnTrash.transform.position = player.transform.position + new Vector3(0f, -0.3f);
    }

    public void NewDay()
    {
        if (gameOver)
        {
            InitGame();
            return;
        }

		if (showingIntro)
		{
			showingIntro = false;
			InitGame();
			return;
		}
        
        eventMenuConfirm.start();
        
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
            if (requiredFood - currentFood == 1)
            {
                extraFood += 1;
                requiredFood = foodReqs[currentFoodReqPosition] + extraFood;
            }
        }
        else
        {
            currentFoodReqPosition += 1;
            requiredFood = foodReqs[currentFoodReqPosition] + extraFood;
        }
        selectedBuilding = -1;

        // check punishments
        if (totalCO2 >= 15 && stateCO2 == 0)
        {
            stateCO2 = 1;
            ice.sprite = spriteIces[1];
        }
        else if (totalCO2 >= 30 && stateCO2 == 1)
        {
            stateCO2 = 2;
            for (int i = 0; i < worldLandSlots.Length; i++)
            {
                worldLandSlots[i].sprite = spriteLandSlots[landSlots[i] + 8];
            }
            ice.sprite = spriteIces[2];
        }
        else if (totalCO2 >= 45 && stateCO2 == 2)
        {
            stateCO2 = 3;
            ice.sprite = spriteIces[3];
        }
        else if (totalCO2 >= 60 && stateCO2 == 3)
        {
            stateCO2 = 4;
            for (int i = 0; i < worldLandSlots.Length; i++)
            {
                worldLandSlots[i].sprite = spriteLandSlots[landSlots[i] + 16];
            }
            ice.sprite = spriteIces[4];
        }

        if (totalContamination >= 10 && stateContamination == 0)
        {
            stateContamination = 1;
            globe.sprite = spriteGlobes[1];
        }
        else if (totalContamination >= 20 && stateContamination == 1)
        {
            stateContamination = 2;
            globe.sprite = spriteGlobes[2];
        }
        else if (totalContamination >= 30 && stateContamination == 2)
        {
            stateContamination = 3;
            globe.sprite = spriteGlobes[3];
        }
        else if (totalContamination >= 40 && stateContamination == 3)
        {
            stateContamination = 4;
            globe.sprite = spriteGlobes[4];
        }


        //check game over
        if (currentDeaths >= maxDeaths)
        {
            popup.sprite = spriteEndDeath;
            popup.enabled = true;
            currentDeaths = maxDeaths;
            gameOver = true;
            textNextYear.text = "Börja om";
        }
        else if (totalCO2 >= maxTotalCO2)
        {
            popup.sprite = spriteEndFlood;
            popup.enabled = true;
            totalCO2 = maxTotalCO2;
            gameOver = true;
            textNextYear.text = "Börja om";
        }
        else if (totalContamination >= maxTotalContamination)
        {
            popup.sprite = spriteEndContaminated;
            popup.enabled = true;
            totalContamination = maxTotalContamination;
            gameOver = true;
			textNextYear.text = "Börja om";
        }
        else if (currentDay == 15)
        {
            popup.sprite = spriteEndNormal;
            popup.enabled = true;
            textGameOver.text = Mathf.RoundToInt(((maxTotalContamination - totalContamination) + (maxTotalCO2 - totalCO2) + (maxDeaths - currentDeaths)) * ((float)currentWellfare / 10f)).ToString();
            gameOver = true;
			textNextYear.text = "Börja om";
        }

        currentDay++;
        currentActionsLeft = 2;
    }

    public void SelectBuilding(int slot)
    {
        if (gameOver) return;

        if (worldSlotStatus[currentPosition] != -1 || currentActionsLeft <= 0)
        {
            eventMenuCancel.start();
            return;
        }

        eventBuildingPlacement.start();

        selectedBuilding = slotOptions[slot];

        worldSlotStatus[currentPosition] = selectedBuilding;

        // apply stats from buldings
        if (selectedBuilding == COWFARM)
        {
            currentFood += 4;
            currentCO2 += 3;
			Instantiate (psCO2Build, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
        }
        if (selectedBuilding == CHICKENFARM)
        {
            currentFood += 3;
            currentCO2 += 2;
			Instantiate (psCO2Build, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
        }
        if (selectedBuilding == EGGFARM)
        {
            currentFood += 2;
            currentCO2 += 1;
			Instantiate (psCO2Build, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
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

        if (currentActionsLeft <= 0)
        {
            eventMenuCancel.start();
            return;
        }

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
                eventMenuNavigate.start();
                CheckChanges();
            }
            else
            {
                eventMenuCancel.start();
            }
        }
    }

    public void Upgrade(bool happy)
    {
        if (gameOver) return;

        eventMenuConfirm.start();

        if (worldSlotStatus[currentPosition] == COWFARM)
        {
            if (happy)
            {
                currentWellfare += 2;
                currentFood -= 1;
                worldSlotStatus[currentPosition] = COWHAPPY;
                currentActionsLeft -= 1;
				Instantiate (psCow, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psHeart, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psWellfareHappy, barWellfareMarker.transform.position + new Vector3(0.2f, 0), new Quaternion(0, 0, 0, 0));
            }
            else
            {
                currentWellfare -= 1;
                currentFood += 1;
                currentCO2 += 1;
                worldSlotStatus[currentPosition] = COWFOOD;
                currentActionsLeft -= 1;
				Instantiate (psCO2Build, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psCow, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psSkull, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psWellfareSad, barWellfareMarker.transform.position + new Vector3(-0.2f, 0), new Quaternion(0, 0, 0, 0));
            }
        }
        if (worldSlotStatus[currentPosition] == CHICKENFARM)
        {
            if (happy)
            {
                currentWellfare += 1;
                worldSlotStatus[currentPosition] = CHICKENHAPPY;
                currentActionsLeft -= 1;
				Instantiate (psChicken, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psHeart, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psWellfareHappy, barWellfareMarker.transform.position + new Vector3(0.2f, 0), new Quaternion(0, 0, 0, 0));
            }
            else
            {
                currentWellfare -= 3;
                currentFood += 2;
                currentCO2 -= 1;
                worldSlotStatus[currentPosition] = CHICKENFOOD;
                currentActionsLeft -= 1;
				Instantiate (psChicken, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psSkull, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psWellfareSad, barWellfareMarker.transform.position + new Vector3(-0.2f, 0), new Quaternion(0, 0, 0, 0));
            }
        }
        if (worldSlotStatus[currentPosition] == EGGFARM)
        {
            if (happy)
            {
                currentWellfare += 2;
                worldSlotStatus[currentPosition] = EGGHAPPY;
                currentActionsLeft -= 1;
				Instantiate (psChicken, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psHeart, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psWellfareHappy, barWellfareMarker.transform.position + new Vector3(0.2f, 0), new Quaternion(0, 0, 0, 0));
            }
            else
            {
                currentWellfare -= 1;
                currentFood += 1;
                worldSlotStatus[currentPosition] = EGGFOOD;
                currentActionsLeft -= 1;
				Instantiate (psChicken, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psSkull, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
				Instantiate (psWellfareSad, barWellfareMarker.transform.position + new Vector3(-0.2f, 0), new Quaternion(0, 0, 0, 0));
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
				Instantiate (psContaminationBuild, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
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
				Instantiate (psContaminationBuild, worldLandSlots[currentPosition].transform.position, new Quaternion(0, 0, 0, 0));
            }
        }

		CheckChanges();
    }

    public void Trash()
    {
        if (gameOver) return;

        eventMenuConfirm.start();

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
