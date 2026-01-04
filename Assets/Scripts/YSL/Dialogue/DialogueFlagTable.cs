using UnityEngine;
using System.Collections.Generic;

public class DialogueFlagTable : MonoBehaviour
{
    public static DialogueFlagTable Instance;

    public bool dl_done0;
    public bool dl_done1;
    public string ritual_offering;

    private void Awake()
    {
        Instance = this;
    }

    public void Set(string key, string value)
    {
        if (key == "dl_done0")
            dl_done0 = (value == "true");

        if (key == "ritual_offering")
            ritual_offering = value;
    }

    public string Get(string key)
    {
        if (key == "dl_done0")
            return dl_done0 ? "true" : "false";

        if (key == "ritual_offering")
            return ritual_offering;

        return null;
    }
}
