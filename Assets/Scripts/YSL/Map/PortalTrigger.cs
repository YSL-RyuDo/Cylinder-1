using UnityEngine;
using System.Collections;

public class PortalTrigger : MonoBehaviour
{
    private PortalManager manager;
    private bool isOnCooldown = false;

    private DialogueLoader dialogueLoader;

    private void Start()
    {
        manager = FindObjectOfType<PortalManager>();
        if (manager == null)
            Debug.LogError("PortalManager를 찾을 수 없습니다.");

        dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader == null)
            Debug.LogError("DialogueLoader를 찾을 수 없습니다.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isOnCooldown) return;

        bool isOpen = manager.IsPortalOpen(gameObject);
        if (!isOpen)
        {
            ShowLockedDialogue(other.gameObject);
            return;
        }

        Transform dest = manager.GetDestination(gameObject);
        if (dest != null)
        {
            StartCoroutine(TeleportWithFade(other.gameObject, dest));
        }
    }

    private void ShowLockedDialogue(GameObject player)
    {
        int personalityId = 0;

        PlayerPersonality personality = player.GetComponent<PlayerPersonality>();
        if (personality != null)
            personalityId = personality.GetPersonalityIndex();

        if (dialogueLoader == null)
        {
            Debug.LogWarning("DialogueLoader가 준비되지 않았습니다.");
            return;
        }

        string[] lines = DialogueLoader.Instance.GetRandomPortalDialogueLines(personalityId);
        FindObjectOfType<DialogueManager>().StartDialogue(lines, personalityId);


    }


    private IEnumerator TeleportWithFade(GameObject player, Transform dest)
    {
        isOnCooldown = true;
        PortalTrigger destTrigger = dest.GetComponent<PortalTrigger>();
        if (destTrigger != null)
        {
            destTrigger.isOnCooldown = true;
        }

        yield return FadeManager.Instance.FadeOut();

        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            MapBoundary parentMapBoundary = dest.GetComponentInParent<MapBoundary>();
            if (parentMapBoundary != null)
                playerMove.SetCurrentMapBoundary(parentMapBoundary);

            playerMove.Warp(dest.position.x, dest.position.y);
        }

        yield return FadeManager.Instance.FadeIn();

        yield return new WaitForSeconds(0.2f);
        isOnCooldown = false;
        if (destTrigger != null)
            destTrigger.isOnCooldown = false;
    }
}
