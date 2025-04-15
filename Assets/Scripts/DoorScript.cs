using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private float warpPointX;
    [SerializeField] private float warpPointY;
    private PlayerScript playerScript;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScript player = collision.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.Warp(warpPointX, warpPointY);
            }
        }
    }
}
