using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterLayerTarget : MonoBehaviour
{
    [Tooltip("레이어를 바꿔야 하는 오브젝트(콜라이더가 있는 쪽). 비우면 자기 자신.")]
    [SerializeField] private GameObject target;

    public GameObject Target => target ? target : gameObject;

    private void Reset()
    {
        target = gameObject;
    }
}
