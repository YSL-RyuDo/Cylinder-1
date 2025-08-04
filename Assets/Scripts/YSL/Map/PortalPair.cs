using UnityEngine;

[System.Serializable]
public class PortalPair
{
    public GameObject portalA;
    public GameObject portalB;

    public GameObject GetOtherPortal(GameObject from)
    {
        if (from == portalA) return portalB;
        if (from == portalB) return portalA;
        return null;
    }
}
