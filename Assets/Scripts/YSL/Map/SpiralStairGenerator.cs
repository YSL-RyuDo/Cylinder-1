using UnityEngine;

public class RepeatingAnglesStairs : MonoBehaviour
{
    public GameObject stairPrefab;
    public Vector3 centerPosition = new Vector3(150f, 230f, 0f);
    public Vector3 stairScale = Vector3.one;
    public float zSpacing = 0.1f;

    public float[] baseAngles = new float[] { 0f, 90f, 180f, 270f };
    public int totalSegments = 12;

    void Start()
    {
        for (int i = 0; i < totalSegments; i++)
        {
            float angle = baseAngles[i % baseAngles.Length];
            Vector3 pos = new Vector3(centerPosition.x, centerPosition.y, centerPosition.z + i * zSpacing);

            GameObject stair = Instantiate(stairPrefab, pos, Quaternion.Euler(0f, 0f, angle), transform);
            stair.transform.localScale = stairScale;

            var sr = stair.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = i;
        }

        var boundary = GetComponent<CircularMapBoundary>();
        if (boundary != null)
        {
            boundary.center = new Vector2(centerPosition.x, centerPosition.y);
            boundary.CalculateBounds();
        }
    }
}
