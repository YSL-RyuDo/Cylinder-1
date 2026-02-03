using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private PlayerMove playerMove;
    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;
    private bool externalControl = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        if (target != null)
            playerMove = target.GetComponent<PlayerMove>();

        AdjustCameraSizeToMap();
    }

    void LateUpdate()
    {
        if (externalControl)
            return;

        if (playerMove == null)
            return;

        var mapBoundary = playerMove.CurrentMapBoundary;
        var circularBoundary = playerMove.CurrentCircularBoundary;

        if (mapBoundary == null && circularBoundary == null)
            return;

        if (circularBoundary != null)
        {
            transform.position = new Vector3(150f, 230f, transform.position.z);
            return;
        }

        Vector3 targetPos = target.position;
        Vector3 newPos = targetPos;

        float camViewWidth = camHalfWidth * 2f;
        float camViewHeight = camHalfHeight * 2f;

        float mapWidth = mapBoundary.max.x - mapBoundary.min.x;
        float mapHeight = mapBoundary.max.y - mapBoundary.min.y;

        bool mapSmallerThanCamView = (mapWidth < camViewWidth) || (mapHeight < camViewHeight);

        if (!mapSmallerThanCamView)
        {
            float minX = mapBoundary.min.x + camHalfWidth;
            float maxX = mapBoundary.max.x - camHalfWidth;
            float minY = mapBoundary.min.y + camHalfHeight;
            float maxY = mapBoundary.max.y - camHalfHeight;

            newPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        newPos.z = transform.position.z;
        transform.position = newPos;
    }

    public void SnapToPositionImmediate(Vector3 pos)
    {
        if (target == null)
            return;

        PlayerMove pm = target.GetComponent<PlayerMove>();
        if (pm == null)
            return;

        MapBoundary mapBoundary = pm.CurrentMapBoundary;
        CircularMapBoundary circularBoundary = pm.CurrentCircularBoundary;

        Vector3 newPos = pos;

        float camRadiusX = camHalfWidth;
        float camRadiusY = camHalfHeight;

        if (circularBoundary != null)
        {
            newPos = new Vector3(150f, 230f, transform.position.z);
            transform.position = newPos;
            return;
        }
        else if (mapBoundary != null)
        {
            float mapWidth = mapBoundary.max.x - mapBoundary.min.x;
            float mapHeight = mapBoundary.max.y - mapBoundary.min.y;

            float clampedMinX = mapBoundary.min.x + Mathf.Min(camRadiusX, mapWidth / 2f);
            float clampedMaxX = mapBoundary.max.x - Mathf.Min(camRadiusX, mapWidth / 2f);
            float clampedMinY = mapBoundary.min.y + Mathf.Min(camRadiusY, mapHeight / 2f);
            float clampedMaxY = mapBoundary.max.y - Mathf.Min(camRadiusY, mapHeight / 2f);

            newPos.x = Mathf.Clamp(pos.x, clampedMinX, clampedMaxX);
            newPos.y = Mathf.Clamp(pos.y, clampedMinY, clampedMaxY);
        }

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
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

        if (mapHeight < defaultSize * 2f)
        {
            sizeByHeight = mapHeight / 2f;
        }

        float widthBasedSize = mapWidth / (2f * aspect);
        if (mapWidth < defaultSize * 2f * aspect)
        {
            sizeByWidth = widthBasedSize;
        }

        cam.orthographicSize = Mathf.Min(sizeByHeight, sizeByWidth);

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * aspect;

        SnapToPositionImmediate(target.position);
    }

    public void BeginExternalControl()
    {
        externalControl = true;
    }

    public void EndExternalControl()
    {
        externalControl = false;
    }

}
