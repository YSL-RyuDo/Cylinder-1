using System.Collections.Generic;

[System.Serializable]
public class ItemDatas
{
    public int id;
    public string itemName;
    public string description;
    public int uses;
    public string iconName; // ex: "magnifying_glass"
}


[System.Serializable]
public class ItemDataList
{
    public List<ItemDatas> items;
}
