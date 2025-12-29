//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class InteractScript : MonoBehaviour
//{
//    public int id;
//    //상호작용이 npc인지아닌지 구분하는 용도
//    public bool isNpc;
//    public ItemData itemData;
    
//    //아이템 상호작용후 소멸
//    public void interact()
//    {
//        PlayerScript player = FindObjectOfType<PlayerScript>();
//        if (!isNpc && player != null && itemData != null)
//        {
//            player.AddItem(itemData);
//            Destroy(gameObject);
//        }
//    }
//}
