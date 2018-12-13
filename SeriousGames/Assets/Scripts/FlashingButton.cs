using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingButton : MonoBehaviour {

    private MainHandler mainHandler;
    public GameObject mainCamera;

    public float interval;
    bool active = false;
    public GameObject flashingObject;

    
	void Start () 
    {
        mainHandler = mainCamera.GetComponent<MainHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainHandler.currentActionsLeft <= 0 && active == false) //As long as there are no actions left, the controller will activate
        {
            FlashController(true);
            active = true;
        }
        else if (mainHandler.currentActionsLeft > 0)
        {
            FlashController(false);
        }

	}

    void FlashController (bool check) //When active, invoke the method that makes the object flash
    {
        if (check == true)
        {
            InvokeRepeating("FlashingObject", 0, interval);
        }   
        else
        {
            CancelInvoke("FlashingObject");
            flashingObject.SetActive(false);
            active = false;
        }
           
    }

    void FlashingObject()
    {
        if (flashingObject.activeSelf)
            flashingObject.SetActive(false);
        else
            flashingObject.SetActive(true);
    }
}
