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
}
