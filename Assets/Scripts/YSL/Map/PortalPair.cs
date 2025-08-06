using UnityEngine;

[System.Serializable]
public class PortalPair
{
    public GameObject portalA;
    public GameObject portalB;

    public bool AtoB_Active = true;
    public bool BtoA_Active = true;

    public GameObject GetOtherPortal(GameObject from)
    {
        if (from == portalA && AtoB_Active) return portalB;
        if (from == portalB && BtoA_Active) return portalA;
        return null;
    }
}
