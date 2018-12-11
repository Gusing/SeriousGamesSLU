using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereControl : MonoBehaviour {

    public float atmSpeed;
    public float cloudSpeed;

    public Sprite[] atmBackSprites;
    public Sprite[] atmCloudsSprites;
    public Sprite[] atmTopSprites;
    public SpriteRenderer atmBack;
    public SpriteRenderer atmClouds;
    public SpriteRenderer atmTop;

    public GameObject backLayer;
    public GameObject cloudLayer;
    public GameObject topLayer;

    public GameObject mainCamera;

    private MainHandler mainHandler;

    public int totalCO2;


    void Awake()
    {
        mainHandler = mainCamera.GetComponent<MainHandler>();
        totalCO2 = mainHandler.totalCO2;
    }
    // Use this for initialization
    void Start ()
    {
      
	}
	
	// Update is called once per frame
	void Update ()
    {
        totalCO2 = mainHandler.totalCO2;
        if (totalCO2 >= 0 && totalCO2 < 20)
        {
            atmBack.sprite = atmBackSprites[0];
            atmClouds.sprite = atmCloudsSprites[0];
            topLayer.SetActive(false);
        }
        if (totalCO2 >= 20 && totalCO2 < 40)
        {
            atmBack.sprite = atmBackSprites[1];
            atmClouds.sprite = atmCloudsSprites[1];
            topLayer.SetActive(true);
            atmTop.sprite = atmTopSprites[0];
        }
        if (totalCO2 >= 40 && totalCO2 <60)
        {
            atmBack.sprite = atmBackSprites[2];
            atmClouds.sprite = atmCloudsSprites[1];
            atmTop.sprite = atmTopSprites[0];
        }
        if(totalCO2 >= 60)
        {
            atmBack.sprite = atmBackSprites[2];
            atmClouds.sprite = atmCloudsSprites[2];
            atmTop.sprite = atmTopSprites[1];
        }

        backLayer.transform.Rotate(Vector3.forward, 10f * atmSpeed * Time.deltaTime);
        topLayer.transform.Rotate(Vector3.forward, 10f * atmSpeed * Time.deltaTime);
        cloudLayer.transform.Rotate(Vector3.forward, 10f * cloudSpeed * Time.deltaTime);


    }
}
