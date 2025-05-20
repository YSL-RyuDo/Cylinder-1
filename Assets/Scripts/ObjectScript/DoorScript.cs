using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool needKey;
    public bool whileOpen;
    public ItemData itemData;
    public GameObject opendoor;
    [SerializeField] private float warpPointX;
    [SerializeField] private float warpPointY;
    private PlayerScript playerScript;
    
    // Update is called once per frame
    void Update()
    {

    }

    public void interact()
    {
        if (whileOpen)
        {
            opendoor.SetActive(true);
            gameObject.SetActive(false);
        }
        if (!needKey)
        {
            PlayerScript player = FindObjectOfType<PlayerScript>();
            player.Warp(warpPointX, warpPointY);
        }
    }
}
