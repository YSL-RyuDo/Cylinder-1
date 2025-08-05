using UnityEngine;

// 플레이어가 어느 층에 있는지 체크 및 저장
public class PlayerLayerController : MonoBehaviour
{
    public int currentLayerIndex = 0;

    public CircularMapBoundary circularBoundary;
    private PlayerMove playerMove;

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        if (playerMove != null && circularBoundary == null)
            circularBoundary = playerMove.CurrentCircularBoundary;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (circularBoundary == null)
            return;

        if (other.CompareTag("LayerTrigger"))
        {
            Vector2 playerPos2D = new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Distance(playerPos2D, circularBoundary.center) <= circularBoundary.radius)
            {
                LayerTrigger layerTrigger = other.GetComponent<LayerTrigger>();
                if (layerTrigger != null)
                {
                    currentLayerIndex = layerTrigger.layerIndex;
                    Vector3 pos = transform.position;
                    pos.z = 0;  // 플레이어 z는 0으로 고정
                    transform.position = pos;
                }
            }
        }
    }
}
