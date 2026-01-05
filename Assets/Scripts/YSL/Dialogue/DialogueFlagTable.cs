using UnityEngine;
using System.Collections.Generic;

public class DialogueFlagTable : MonoBehaviour
{
    public static DialogueFlagTable Instance;

    public bool dl_done0;
    public bool dl_done1;
    public bool dl_done2;
    public bool dl_done3;
    public bool dl_done4;
    public string ritual_offering;

    private void Awake()
    {
        Instance = this;
    }

    public void Set(string key, string value)
    {
        bool v = (value == "true");

        if (key == "dl_done0") dl_done0 = v;
        else if (key == "dl_done1") dl_done1 = v;
        else if (key == "dl_done2") dl_done2 = v;
        else if (key == "dl_done3") dl_done3 = v;
        else if (key == "dl_done4") dl_done4 = v;
        else if (key == "ritual_offering") ritual_offering = value;
    }


    public string Get(string key)
    {
        if (key == "dl_done0") return dl_done0 ? "true" : "false";
        if (key == "dl_done1") return dl_done1 ? "true" : "false";
        if (key == "dl_done2") return dl_done2 ? "true" : "false";
        if (key == "dl_done3") return dl_done3 ? "true" : "false";
        if (key == "dl_done4") return dl_done4 ? "true" : "false";
        if (key == "ritual_offering") return ritual_offering;

        return null;
    }

}
