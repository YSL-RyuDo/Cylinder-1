using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerScript;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    //오브젝트 여러개 겹쳤을때 리스트에 일단 등록
    public List<GameObject> overlappingObjects = new List<GameObject>();
    public GameObject menu;
    private PlayerControlls playerControlls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    //캐릭터 자동 이동 구현용
    private bool isAutoMoving = false;
    private Vector2 autoMoveTarget;
    private float autoMoveSpeed = 1f;
    public float autoMoveThreshold = 0.25f;
    //리코셰 퍼즐
    public bool isOnRicochetPlatform = false;
    public bool isSliding = false;
    public Vector2 slideDirection;
    [SerializeField] private identityType playeridentity = identityType.None;
    //가장 가까운 오브젝트 위에 상호작용 가능 이모지 띄우기
    public GameObject interactIconPrefab;
    private GameObject currentInteractIcon;
    private GameObject currentNearest;
    [SerializeField] private GameObject npcIconPrefab;
    [SerializeField] private GameObject itemIconPrefab;
    //대화중인지 감지하기 위함
    public enum identityType
    {
        None,
        Pure,
        Lack,
        Desire,
        Grief,
        Arrogance
    }

    [System.Serializable]
    public class InventoryItem
    {
        public ItemData itemData;

        public InventoryItem(ItemData data)
        {
            itemData = data;
        }
    }

    public List<InventoryItem> inventory = new List<InventoryItem>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerControlls = new PlayerControlls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControlls.Enable();
    }

    private void Update()
    {
        if (TalkManagerScript.instance != null && TalkManagerScript.instance.dialogueGroup.alpha > 0)
        {
            movement = Vector2.zero;
            animator.SetBool("IsWalk", false);
            return;
        }
        //키 조작
        if (!isAutoMoving && !isSliding && isOnRicochetPlatform)
        {
            if (Input.GetKeyDown(KeyCode.W)) StartRicochet(Vector2.up);
            if (Input.GetKeyDown(KeyCode.S)) StartRicochet(Vector2.down);
            if (Input.GetKeyDown(KeyCode.A)) StartRicochet(Vector2.left);
            if (Input.GetKeyDown(KeyCode.D)) StartRicochet(Vector2.right);
        }
        if (!isAutoMoving)
        {
            PlayerInput();
        }
        else
        {
            Vector2 direction = (autoMoveTarget - rb.position).normalized;
            movement = direction;

            if (Vector2.Distance(rb.position, autoMoveTarget) < autoMoveThreshold)
            {
                movement = Vector2.zero;
                isAutoMoving = false;
            }
        }
        //일시정지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf)
            {
                menu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                menu.SetActive(true);
                Time.timeScale = 0;
            }
        }
        //상호작용키
        if (Input.GetKeyDown(KeyCode.F) && overlappingObjects.Count > 0 && TalkManagerScript.instance.dialogueGroup.alpha == 0)
        {
            GameObject nearest = FindNearestObject();
            if (nearest != null)
            {
                nearest.SendMessage("interact", SendMessageOptions.DontRequireReceiver);
                overlappingObjects.Remove(nearest);
            }
        }
        UpdateNearestAndInteractionIcon();
    }

    private void FixedUpdate()
    {
        if (TalkManagerScript.instance != null && TalkManagerScript.instance.dialogueGroup.alpha > 0)
            return;
        Move();
        if (isSliding)
        {
            rb.MovePosition(rb.position + slideDirection * moveSpeed * Time.fixedDeltaTime);
            animator.SetBool("IsWalk", true);

            if (slideDirection.x > 0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (slideDirection.x < -0.01f)
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //입력을 받는 함수
    private void PlayerInput()
    {
        movement = playerControlls.Movement.Move.ReadValue<Vector2>();
    }

    //playerinput에서 받은 정보에 따라 속도를 곱해 이동시키는 함수
    private void Move()
    {
        float speed = isAutoMoving ? autoMoveSpeed : moveSpeed;
        rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
        if (movement.x > 0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement.x < -0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        bool isWalking = movement.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalk", isWalking);
        if (isWalking)
        {
            animator.SetFloat("WalkDirX", movement.x);
            animator.SetFloat("WalkDirY", movement.y);
        }
    }

    //문, 워프 같은것을 위해 만든 함수, x y값을 입력받아 해당 위치로 player의 위치를 즉시 이동시킴
    public void Warp(float x, float y)
    {
        if (TalkManagerScript.instance != null && TalkManagerScript.instance.dialogueGroup.alpha > 0)
        {
            return;
        }
        rb.position = new Vector2(x, y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("벽에 부딪힘, 슬라이딩 멈춤");
            isSliding = false;
            movement = Vector2.zero;
            animator.SetBool("IsWalk", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //아이템과 NPC 겹쳐진것 전부 등록
        if (collision.CompareTag("Interact"))
        { 
            overlappingObjects.Add(collision.gameObject);
        }
        if (collision.CompareTag("RicochetPlatform"))
        {
            isOnRicochetPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlappingObjects.Remove(collision.gameObject);
        if (collision.CompareTag("RicochetPlatform") && collision.isTrigger)
        {
            isOnRicochetPlatform = false;
        }
    }

    private GameObject FindNearestObject()
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        Vector2 playerPos = transform.position;

        foreach (GameObject obj in overlappingObjects)
        {
            if (obj == null) continue;
            float distance = Vector2.Distance(playerPos, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }
        return nearest;
    }

    public void AddItem(ItemData data)
    {
        inventory.Add(new InventoryItem(data));
        Debug.Log("아이템 추가됨: " + data.itemName);
    }

    public void RemoveItem(ItemData data)
    {
        InventoryItem itemToRemove = inventory.Find(item => item.itemData == data);
        if (itemToRemove != null)
        {
            inventory.Remove(itemToRemove);
            Debug.Log("아이템 제거됨: " + data.itemName);
        }
        else
        {
            Debug.LogWarning("제거할 아이템을 찾을 수 없음: ID " + data.itemName);
        }
    }

    public bool HasItem(ItemData data)
    {
        return inventory.Exists(item => item.itemData == data);
    }


    public void AutoMoveTo(Vector2 target)
    {
        AutoMoveTo(target, moveSpeed);
    }
    public void AutoMoveTo(Vector2 target, float speed)
    {
        autoMoveTarget = target;
        autoMoveSpeed = speed;
        isAutoMoving = true;
    }

    public identityType Playeridentity
    {
        get { return playeridentity; }
        set
        {
            playeridentity = value;
            Debug.Log("속성 변경됨: " + playeridentity);
        }
    }

    public void ChangeIdentity(identityType newIdentity)
    {
        playeridentity = newIdentity;
    }

    private void UpdateNearestAndInteractionIcon()
    {
        GameObject nearest = FindNearestObject();
        if (nearest != currentNearest)
        {
            currentNearest = nearest;

            if (currentInteractIcon != null)
            {
                Destroy(currentInteractIcon);
                currentInteractIcon = null;
            }

            if (currentNearest != null)
            {
                InteractScript interactScript = currentNearest.GetComponent<InteractScript>();
                bool isNpc = interactScript != null && interactScript.isNpc;

                GameObject prefabToUse = isNpc ? npcIconPrefab : itemIconPrefab;

                if (prefabToUse != null)
                {
                    currentInteractIcon = Instantiate(prefabToUse);
                    currentInteractIcon.transform.SetParent(transform, false);
                    currentInteractIcon.transform.localPosition = new Vector3(0, 2.1f, 0); // 오브젝트 위에 띄우기
                }
                else
                {
                    Debug.LogWarning("프리팹이 설정되지 않았습니다: " + (isNpc ? "npcIconPrefab" : "itemIconPrefab"));
                }
            }
        }
    }

    private void StartRicochet(Vector2 direction)
    {
        isSliding = true;
        slideDirection = direction.normalized;
    }
}