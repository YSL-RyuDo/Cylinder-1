using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool needKey;
    public int KeyValue;
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
        if (!needKey)
        {
            PlayerScript player = FindObjectOfType<PlayerScript>();
            player.Warp(warpPointX, warpPointY);
        }
        else
        {
            PlayerScript player = FindObjectOfType<PlayerScript>();
            if (player.HasItem(1))
            {
                player.RemoveItem(1);
                needKey = false;
                opendoor.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
