using UnityEngine;
using System.Collections;

public class PortalTrigger : MonoBehaviour
{
    public PortalData data;

    private bool playerInRange = false;
    private bool portalReady = true;   // 이 포탈이 현재 작동 가능한 상태
    private GameObject player;

    public AudioClip unlockSound;      // 문이 열릴 때 재생할 효과음
    private AudioSource audioSource;
    private PortalMonologue portalMono;
    private void Start()
    {
        PortalManager.Instance.RegisterPortal(this);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!playerInRange || !portalReady)
            return;

        // 1) 열려있으면 자동 이동
        if (!data.isLocked)
        {
            TryTeleport();
            return;
        }

        // 2) 잠겨 있으면 F키로 해제 시도
        if (Input.GetKeyDown(KeyCode.F))
            TryUnlock();
    }

    // 포탈 이동
    private void TryTeleport()
    {
        Transform target = PortalManager.Instance.GetLinkedPortalTransform(data.portalId);
        if (target == null) return;

        // 텔레포트 시작 시 플레이어 참조 백업
        GameObject p = player;

        StartCoroutine(TeleportWithFade(p, target.position));

        portalReady = false;

        PortalTrigger targetPortal = PortalManager.Instance.GetPortalTrigger(data.linkedPortalId);
        if (targetPortal != null)
            targetPortal.portalReady = false;
    }

    private IEnumerator TeleportWithFade(GameObject p, Vector3 targetPos)
    {
        yield return FadeManager.Instance.FadeOut();

        if (p != null)
            p.GetComponent<PlayerMove>().Warp(targetPos);

        yield return FadeManager.Instance.FadeIn();
    }

    // 잠금 해제
    private void TryUnlock()
    {
        int pid = PlayerPersonality.Instance.GetPersonalityIndex();
        PortalMonologue portalMono = DialogueLoader.Instance.PortalGet(pid);

        ItemData registered = InventoryManager.Instance.GetRegisteredItem();

        // 아이템 없음
        if (registered == null)
        {
            //DialogueManager.Instance.ShowPortalMonologue(portalMono.missingItem, portalMono.folder);
            return;
        }

        // 정답 아이템
        if (registered.id == data.requiredItemId)
        {
            InventoryManager.Instance.ClearRegisteredItem();
            data.isLocked = false;
            SoundManager.Instance.PlaySFX(0);
            return;
        }

        // 잘못된 아이템
        //DialogueManager.Instance.ShowPortalMonologue(portalMono.wrongItem, portalMono.folder);
    }


    // 콜라이더 체크
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        player = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        player = null;

        // 완전히 벗어났을 때 다시 작동 가능
        portalReady = true;
    }
}
