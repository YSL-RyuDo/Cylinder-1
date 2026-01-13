using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldMovePointTable : MonoBehaviour
{
    public static WorldMovePointTable Instance;

    [Serializable]
    public class MovePoint
    {
        public string key;
        public Transform point;
    }

    public List<MovePoint> points;

    private Dictionary<string, Transform> table = new();

    private void Awake()
    {
        Instance = this;
        foreach (var p in points)
            table[p.key] = p.point;
    }

    public Transform Get(string key)
    {
        return table.TryGetValue(key, out var t) ? t : null;
    }
}
