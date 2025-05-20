using UnityEngine;

[CreateAssetMenu(fileName = "NewItem" , menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public Sprite icon;
}
