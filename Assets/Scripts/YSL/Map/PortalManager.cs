using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public List<PortalPair> portalPairs = new List<PortalPair>();

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
