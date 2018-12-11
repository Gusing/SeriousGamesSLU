using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {


    public GameObject player;



    public void charAnimation(int currentPosition, int slot)
    {
        player.GetComponent<Animator>().SetBool("isMoving", true);
        if ((currentPosition == 1 && slot == 7) || (currentPosition == 0 && slot == 6) || (currentPosition == 0 && slot == 7))
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if ((currentPosition == 7 && slot == 1) || (currentPosition == 7 && slot == 0) || (currentPosition == 6 && slot == 0))
        {
            player.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (currentPosition - slot > 0)
            player.transform.localScale = new Vector3(-1, 1, 1);
        else
            player.transform.localScale = new Vector3(1, 1, 1);

    }
}
