using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public List<PortalPair> portalPairs = new List<PortalPair>();

    public bool IsPortalOpen(GameObject portal)
    {
        foreach (var pair in portalPairs)
        {
            if (portal == pair.portalA)
                return pair.AtoB_Active;
            else if (portal == pair.portalB)
                return pair.BtoA_Active;
        }
        return false;
    }

    public Transform GetDestination(GameObject portal)
    {
        foreach (var pair in portalPairs)
        {
            GameObject dest = pair.GetOtherPortal(portal);
            if (dest != null)
                return dest.transform;
        }
        return null;
    }

    public int GetPortalIndex(GameObject portal)
    {
        for (int i = 0; i < portalPairs.Count; i++)
        {
            if (portalPairs[i].portalA == portal || portalPairs[i].portalB == portal)
                return i;
        }
        return -1;
    }

    public void SetPortalActive(int portalIndex, bool active)
    {
        if (portalIndex < 0 || portalIndex >= portalPairs.Count)
            return;

        portalPairs[portalIndex].AtoB_Active = active;
        portalPairs[portalIndex].BtoA_Active = active;
    }

}
