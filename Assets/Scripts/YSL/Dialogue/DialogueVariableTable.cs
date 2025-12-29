using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVariableTable
{
    private Dictionary<string, string> vars = new();

    public void Set(string key, string value)
    {
        vars[key] = value;
    }

    public string Get(string key)
    {
        return vars.TryGetValue(key, out var v) ? v : "";
    }
}
