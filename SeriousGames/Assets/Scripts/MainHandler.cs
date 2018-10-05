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

    int[] slotOptions;

    int[] worldSlotStatus;

    public SpriteRenderer[] worldSlots;

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
    
    // Use this for initialization
    void Start ()
    {
        slotOptions = new int[3];
        worldSlotStatus = new int[8];

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
        NewDay();
        CheckChanges();
    }
	
	// Update is called once per frame
	void Update ()
    {
        textDay.text = "Day " + currentDay;
        textActionsLeft.text = "Actions Left: " + currentActionsLeft;
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
            if (worldSlotStatus[i] > -1 && worldSlotStatus[i] < 5)
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

        currentDay++;
        selectedBuilding = -1;
        currentActionsLeft = 2;
    }

    public void SelectBuilding(int slot)
    {
        if (worldSlotStatus[currentPosition] != -1) return;

        selectedBuilding = slotOptions[slot];

        worldSlotStatus[currentPosition] = selectedBuilding;

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
