using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManage : MonoBehaviour
{
    public GameObject FadeIn;
    bool gameStartEvent = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStartEvent && !FadeIn.activeSelf)
        {
            PlayerScript player = FindObjectOfType<PlayerScript>();
            player.AutoMoveTo(new Vector2(31.0f, -5.5f),1f);
            gameStartEvent = true;
        }
    }
}
