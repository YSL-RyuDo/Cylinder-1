using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private PlayerMove playerMove;
    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        if (target != null)
            playerMove = target.GetComponent<PlayerMove>();
        AdjustCameraSizeToMap();
    }

    void FixedUpdate()
    {
        if (playerMove == null)
            return;

        var mapBoundary = playerMove.CurrentMapBoundary;
        if (mapBoundary == null)
            return;

        Vector3 newPos = target.position;

        float minX = mapBoundary.min.x + camHalfWidth;
        float maxX = mapBoundary.max.x - camHalfWidth;
        float minY = mapBoundary.min.y + camHalfHeight;
        float maxY = mapBoundary.max.y - camHalfHeight;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        newPos.z = transform.position.z;

        float smoothSpeed = 10f;
        transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.fixedDeltaTime);
    }


    public void SnapToPositionImmediate(Vector3 pos)
    {
        if (target == null)
            return;

        MapBoundary mapBoundary = target.GetComponent<PlayerMove>()?.CurrentMapBoundary;
        if (mapBoundary == null)
            return;

        float minX = mapBoundary.min.x + camHalfWidth;
        float maxX = mapBoundary.max.x - camHalfWidth;
        float minY = mapBoundary.min.y + camHalfHeight;
        float maxY = mapBoundary.max.y - camHalfHeight;

        float clampedX = Mathf.Clamp(pos.x, minX, maxX);
        float clampedY = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    public void AdjustCameraSizeToMap()
    {
        if (target == null)
            return;

        MapBoundary mapBoundary = target.GetComponent<PlayerMove>()?.CurrentMapBoundary;
        if (mapBoundary == null)
            return;

        float defaultSize = 5f;
        float mapHeight = mapBoundary.max.y - mapBoundary.min.y;
        float mapWidth = mapBoundary.max.x - mapBoundary.min.x;
        float aspect = cam.aspect;

        float sizeByHeight = defaultSize;
        float sizeByWidth = defaultSize;

        // Y 방향이 너무 작으면 줄이기
        if (mapHeight < defaultSize * 2f)
        {
            sizeByHeight = mapHeight / 2f;
        }

        // X 방향이 너무 작으면 줄이기 (가로 기준 세로 크기 계산)
        float widthBasedSize = mapWidth / (2f * aspect);
        if (mapWidth < defaultSize * 2f * aspect)
        {
            sizeByWidth = widthBasedSize;
        }

        // 둘 중 작은 쪽을 선택 (여백 안 보이게)
        cam.orthographicSize = Mathf.Min(sizeByHeight, sizeByWidth);

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * aspect;

        SnapToPositionImmediate(target.position);
    }

}