using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GriefKillUtil
{
    public static bool CanShowKillOption(GameObject target)
    {
        if (target == null) return false;

        bool isGrief = PlayerPersonality.Instance != null &&
                       PlayerPersonality.Instance.currentPersonality == PersonalityType.grief;

        if (!isGrief) return false;

        return target.GetComponentInParent<IGriefKillable>() != null;
    }

    public static void ExecuteKill(GameObject target)
    {
        if (target == null) return;

        var killable = target.GetComponentInParent<IGriefKillable>();
        if (killable != null)
            killable.GriefKill();
    }
}
