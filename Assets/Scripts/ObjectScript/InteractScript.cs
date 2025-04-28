using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour
{
    public int id;
    //상호작용이 npc인지아닌지 구분하는 용도
    public bool isNpc;
    //아이템 용, npc일경우 안써도 무관
    public string Name;
    public string Description;
    public Sprite Icon;
    
    //아이템 상호작용후 소멸
    public void interact()
    {
        PlayerScript player = FindObjectOfType<PlayerScript>();
        if (!isNpc && player != null)
        {
            player.AddItem(Name, Description, Icon, id);
        }
        if(!isNpc)
            Destroy(gameObject);
    }
}
