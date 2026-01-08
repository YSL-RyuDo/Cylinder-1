using UnityEngine;

public class DialogueFlagTable : MonoBehaviour
{
    public static DialogueFlagTable Instance;

    public bool dl_done;
    public bool dl_done0;
    public bool dl_done1;
    public bool dl_done2;
    public bool dl_done3;
    public bool dl_done4;
    public bool dl_done5;

    public string ritual_offering;

    // 상태 변경 감지용
    public bool Dirty { get; private set; }

    private void Awake()
    {
        Instance = this;
        Dirty = false;
    }

    public void Set(string key, string value)
    {
        bool v = (value == "true");
        bool changed = false;

        if (key == "dl_done" && dl_done != v) { dl_done = v; changed = true; }
        else if (key == "dl_done0" && dl_done0 != v) { dl_done0 = v; changed = true; }
        else if (key == "dl_done1" && dl_done1 != v) { dl_done1 = v; changed = true; }
        else if (key == "dl_done2" && dl_done2 != v) { dl_done2 = v; changed = true; }
        else if (key == "dl_done3" && dl_done3 != v) { dl_done3 = v; changed = true; }
        else if (key == "dl_done4" && dl_done4 != v) { dl_done4 = v; changed = true; }
        else if (key == "dl_done5" && dl_done5 != v) { dl_done5 = v; changed = true; }
        else if (key == "ritual_offering") { ritual_offering = value; changed = true; }

        if (changed)
            Dirty = true;
    }

    public void ClearDirty()
    {
        Dirty = false;
    }

    public string Get(string key)
    {
        if (key == "dl_done") return dl_done ? "true" : "false";
        if (key == "dl_done0") return dl_done0 ? "true" : "false";
        if (key == "dl_done1") return dl_done1 ? "true" : "false";
        if (key == "dl_done2") return dl_done2 ? "true" : "false";
        if (key == "dl_done3") return dl_done3 ? "true" : "false";
        if (key == "dl_done4") return dl_done4 ? "true" : "false";
        if (key == "dl_done5") return dl_done5 ? "true" : "false";
        if (key == "ritual_offering") return ritual_offering;

        return null;
    }
}
