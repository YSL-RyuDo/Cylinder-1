using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Item nearbyItem = null;  // 현재 가까이 있는 아이템

    private void Update()
    {
        if (nearbyItem != null && Input.GetKeyDown(KeyCode.F))
        {
            nearbyItem.Pickup();
            nearbyItem = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            nearbyItem = item;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null && item == nearbyItem)
        {
            nearbyItem = null;
        }
    }
}
