//using System.Collections.Generic;
//using UnityEngine;

//public class PortalManager : MonoBehaviour
//{
//    public static PortalManager Instance;

//    [Header("씬에 존재하는 포탈 데이터들")]
//    public List<PortalData> portalDataList;

//    // portalId → PortalTrigger
//    private Dictionary<int, PortalTrigger> portalMap = new Dictionary<int, PortalTrigger>();

//    private void Awake()
//    {
//        Instance = this;

//        // 초기 잠김 상태 리셋
//        foreach (var p in portalDataList)
//            p.ResetRuntimeState();
//    }

//    // 포탈 등록
//    public void RegisterPortal(PortalTrigger trigger)
//    {
//        int id = trigger.data.portalId;

//        if (!portalMap.ContainsKey(id))
//            portalMap.Add(id, trigger);
//    }

//    // 연결된 포탈 Transform 가져오기
//    public Transform GetLinkedPortalTransform(int portalId)
//    {
//        if (!portalMap.ContainsKey(portalId))
//            return null;

//        PortalData data = portalMap[portalId].data;

//        if (!portalMap.ContainsKey(data.linkedPortalId))
//            return null;

//        return portalMap[data.linkedPortalId].transform;
//    }

//    // 연결된 포탈 Trigger 자체 가져오기
//    public PortalTrigger GetPortalTrigger(int portalId)
//    {
//        if (portalMap.ContainsKey(portalId))
//            return portalMap[portalId];

//        return null;
//    }
//}
