using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {


    public GameObject player;
    public GameObject planet;
    public GameObject hud;
    public float currentAngle = 0f;
    public float goToAngle = 0f;
    int currentPosition;
    bool movingRight;
    public int rotationSpeed = 2;

    private void Start()
    {
   
    }

    public void Update()
    {
        if (currentAngle != goToAngle)
        {
            player.GetComponent<Animator>().SetBool("isMoving", true); //Activate running animation
            planetRotation();
        }
        else
            player.GetComponent<Animator>().SetBool("isMoving", false); //Deactivate running animation
    }

    public void charAnimation(int currentPosition, int slot)
    {

        //Check if moving to left or right and flip the animation
        if ((currentPosition == 1 && slot == 7) || (currentPosition == 0 && slot == 6) || (currentPosition == 0 && slot == 7))
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
            movingRight = true;
        }
        else if ((currentPosition == 7 && slot == 1) || (currentPosition == 7 && slot == 0) || (currentPosition == 6 && slot == 0))
        {
            player.transform.localScale = new Vector3(1, 1, 1);
            movingRight = false;
        }
        else if (currentPosition - slot > 0)
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
            movingRight = true;
        }      
        else
        {
            player.transform.localScale = new Vector3(1, 1, 1);
            movingRight = false;
        }

    }

    public void rotationActivation(int currentPosition) //Activates the planet rotation
    {
        goToAngle = 360 - currentPosition * 45;
        if (goToAngle == 360)
            goToAngle = 0;
    }

    public void planetRotation() //Smoothly rotates the planet to the new position
    {
        if (movingRight==true)
        {
            planet.transform.Rotate(new Vector3(0, 0, 1), 10.0f * rotationSpeed * Time.deltaTime);
            hud.transform.Rotate(new Vector3(0, 0, 1), 10.0f * rotationSpeed * Time.deltaTime);
        }
        else
        {
            planet.transform.Rotate(new Vector3(0, 0, -1), 10.0f * rotationSpeed * Time.deltaTime);
            hud.transform.Rotate(new Vector3(0, 0, -1), 10.0f * rotationSpeed * Time.deltaTime);
        }
        currentAngle = planet.transform.eulerAngles.z;
        if (currentAngle-goToAngle <= 0.3f && currentAngle-goToAngle>= -0.3f)
        {
            currentAngle = goToAngle;
        }
    }

    public void ResetRotation()
    {
        planet.transform.eulerAngles = new Vector3 (0,0,0);
        hud.transform.eulerAngles = new Vector3(0, 0, 0);
        currentAngle = 0;
        goToAngle = 0;
    }
}
