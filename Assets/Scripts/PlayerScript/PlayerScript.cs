using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    //인벤토리 페이지와 페이지당 아이템 수
    private int currentPage = 0;
    private const int itemsPerPage = 5;
    //오브젝트 여러개 겹쳤을때 리스트에 일단 등록
    public List<GameObject> overlappingObjects = new List<GameObject>();
    public GameObject menu;
    public GameObject inventoryPanel;
    public GameObject itemTextPrefab;
    private PlayerControlls playerControlls;
    private Vector2 movement;
    private Rigidbody2D rb;

    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public string itemDescription;
        public Sprite itemIcon;
        public int itemId;

        public InventoryItem(string name, string description, Sprite icon, int id)
        {
            itemName = name;
            itemDescription = description;
            itemIcon = icon;
            itemId = id;
        }
    }

    public List<InventoryItem> inventory = new List<InventoryItem>();

    private void Awake()
    {
        playerControlls = new PlayerControlls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControlls.Enable();
    }

    private void Update()
    {
        //키 조작
        PlayerInput();
        //일시정지
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menu.activeSelf)
            {
                menu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                menu.SetActive(true);
                Time.timeScale = 0;
                UpdateInventoryUI();
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

    }
    
    private void FixedUpdate()
    {
        Move();
    }

    //입력을 받는 함수
    private void PlayerInput()
    {
        movement = playerControlls.Movement.Move.ReadValue<Vector2>();
    }

    //playerinput에서 받은 정보에 따라 속도를 곱해 이동시키는 함수
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    //문, 워프 같은것을 위해 만든 함수, x y값을 입력받아 해당 위치로 player의 위치를 즉시 이동시킴
    public void Warp(float x, float y)
    {
        rb.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //아이템과 NPC 겹쳐진것 전부 등록
        if (collision.CompareTag("Interact"))
        {
            overlappingObjects.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlappingObjects.Remove(collision.gameObject);
    }

    private GameObject FindNearestObject()
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        Vector2 playerPos = transform.position;

        foreach (GameObject obj in overlappingObjects) {
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

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 현재 페이지의 시작 인덱스 계산
        int startIndex = currentPage * itemsPerPage;

        // 시작 인덱스부터 최대 5개 아이템 표시
        for (int i = startIndex; i < startIndex + itemsPerPage && i < inventory.Count; i++)
        {
            InventoryItem item = inventory[i];
            GameObject newItemText = Instantiate(itemTextPrefab, inventoryPanel.transform);

            TextMeshProUGUI itemNameText = newItemText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            itemNameText.text = $"{item.itemName} - {item.itemDescription}";

            Image itemIconImage = newItemText.transform.GetChild(1).GetComponent<Image>();
            itemIconImage.sprite = item.itemIcon;
        }
    }

    public void AddItem(string name, string description, Sprite icon, int id)
    {
        InventoryItem newItem = new InventoryItem(name, description, icon, id);
        inventory.Add(newItem);
        Debug.Log("아이템 추가됨: " + name + ", 설명:" + description);
    }

    public void RemoveItem(int id)
    {
        InventoryItem itemToRemove = inventory.Find(item => item.itemId == id);
        if (itemToRemove != null)
        {
            inventory.Remove(itemToRemove);
            Debug.Log("아이템 제거됨: " + itemToRemove.itemName);
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning("제거할 아이템을 찾을 수 없음: ID " + id);
        }
    }

    public bool HasItem(int id)
    {
        return inventory.Exists(item => item.itemId == id);
    }

    public void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < inventory.Count)
        {
            currentPage++;
            UpdateInventoryUI();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateInventoryUI();
        }
    }
}