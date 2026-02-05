using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistKillable : MonoBehaviour, IGriefKillable
{
    public void GriefKill()
    {
        // 죽음 처리(애니/드랍/비활성화 등)
        gameObject.SetActive(false);
    }
}
