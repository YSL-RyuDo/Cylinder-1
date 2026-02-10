using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CorpseDrag : MonoBehaviour, IInteractable
{
    public GameObject residuePrefab;
    public float spawnDistance = 0.5f;
    public Transform residueSpawnPoint;

    public float ropeLength = 1.5f;
    public float dragSpeed = 10f;

    private Rigidbody2D rb;
    private Collider2D[] myColliders;
    private Collider2D[] playerColliders;

    private Transform playerTransform;
    private bool isDragging = false;
    private Vector3 lastResiduePos;
    private int originalLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myColliders = GetComponentsInChildren<Collider2D>();
        originalLayer = gameObject.layer;

        rb.bodyType = RigidbodyType2D.Kinematic;

        if (residueSpawnPoint == null) residueSpawnPoint = transform;
    }

    public void Interact(GameObject player)
    {
        if (isDragging)
        {
            Detach();
        }
        else
        {
            Attach(player);
        }
    }

    private void Attach(GameObject player)
    {
        isDragging = true;
        playerTransform = player.transform;
        playerColliders = player.GetComponentsInChildren<Collider2D>();

        if (playerColliders != null && myColliders != null)
        {
            for (int i = 0; i < playerColliders.Length; i++)
            {
                for (int j = 0; j < myColliders.Length; j++)
                {
                    Physics2D.IgnoreCollision(playerColliders[i], myColliders[j], true);
                }
            }
        }

        SetLayerRecursively(gameObject, 2);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 0.01f;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        lastResiduePos = residueSpawnPoint.position;
    }

    private void Detach()
    {
        if (playerColliders != null && myColliders != null)
        {
            for (int i = 0; i < playerColliders.Length; i++)
            {
                for (int j = 0; j < myColliders.Length; j++)
                {
                    if (playerColliders[i] != null && myColliders[j] != null)
                    {
                        Physics2D.IgnoreCollision(playerColliders[i], myColliders[j], false);
                    }
                }
            }
        }

        SetLayerRecursively(gameObject, originalLayer);

        isDragging = false;
        playerTransform = null;
        playerColliders = null;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        if (isDragging && playerTransform != null)
        {
            FollowPlayer();
            HandleResidue();
        }
    }

    private void FollowPlayer()
    {
        float currentDistance = Vector2.Distance(transform.position, playerTransform.position);

        if (currentDistance > ropeLength)
        {
            Vector2 direction = (transform.position - playerTransform.position).normalized;
            Vector2 targetPos = (Vector2)playerTransform.position + (direction * ropeLength);

            Vector2 newPos = Vector2.Lerp(transform.position, targetPos, Time.fixedDeltaTime * dragSpeed);
            rb.MovePosition(newPos);
        }
    }

    private void HandleResidue()
    {
        if (residuePrefab == null) return;

        float distMoved = Vector2.Distance(residueSpawnPoint.position, lastResiduePos);

        if (distMoved >= spawnDistance)
        {
            SpawnResidue();
        }
    }

    private void SpawnResidue()
    {
        Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        Instantiate(residuePrefab, residueSpawnPoint.position, randomRot);
        lastResiduePos = residueSpawnPoint.position;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void OnDisable()
    {
        if (isDragging) Detach();
    }
}