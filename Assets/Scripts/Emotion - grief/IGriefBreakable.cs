using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGriefBreakable
{
    void GriefBreak();
}

public interface IGriefPushable
{
    void GriefPush(Vector2 direction, float force);
}

public interface IGriefKillable
{
    void GriefKill();
}