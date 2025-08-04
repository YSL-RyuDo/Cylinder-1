using UnityEngine;
using System.Collections;

public class PortalTrigger : MonoBehaviour
{
    private PortalManager manager;
    private bool isOnCooldown = false;

    private void Start()
    {
        manager = FindObjectOfType<PortalManager>();
        if (manager == null)
            Debug.LogError("PortalManager를 찾을 수 없습니다.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isOnCooldown) return;

        Transform dest = manager.GetDestination(gameObject);
        if (dest != null)
        {
            StartCoroutine(TeleportWithFade(other.gameObject, dest));
        }
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
