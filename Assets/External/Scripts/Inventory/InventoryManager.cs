using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    bool menuActivated = true;
    public ItemSlot[] itemslot;

    public List<ItemInInventory> inventoryData = new List<ItemInInventory>(); //플레이어 데이터 리스트

    public static Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>();
    public string spreadsheetURL;//구글시트주소

    public Image ScreenImage;
    public TextMeshProUGUI ScreenName;
    public TextMeshProUGUI ScreenDescription;

    private int selectedItemIndex = -1; //아이템 인덱스




    void Awake()
    {
        /*프로토타입때문에 잠시 주석처리
        if(itemDatabase != null)
        {
            itemDatabase.Clear();//데이터베이스 비우기
        }*/
        InventoryMenu.SetActive(false);
        //StartCoroutine(LoadItemDataFromWeb()); //구글시트 로드
    }
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Space) && menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;

            //테스트 코드
            RefreshInventoryUI();
        }*/
        //테스트 코드
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TestAddItem(1001, 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TestAddItem(1002, 2);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TestAddItem(1003, 3);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TestAddItem(1004, 4);
        }
    }

    //테스트 코드
    public void TestAddItem(int id, int amount)
    {
        // 1. 아이템 데이터베이스에서 ItemData 객체를 조회
        ItemData dataToAcquire = GetItemData(id);

        if (dataToAcquire != null)
        {
            // 2. InventoryManager의 AddItem 함수를 호출하여 인벤토리에 추가
            AddItem(dataToAcquire, amount);
        }
        else
        {
            Debug.LogError($"테스트 아이템 ID {id}의 데이터를 찾을 수 없습니다. Google Sheets 로드를 확인하세요.");
        }
    }
    IEnumerator LoadItemDataFromWeb()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(spreadsheetURL);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("파일 불러오기 오류" + webRequest.error);
        }
        else//csv파일로 변환
        {
            string csvText = webRequest.downloadHandler.text;
            ParseCSVData(csvText);
        }
    }
    
    void ParseCSVData(string csvText)//csv파일 데이터로 변환하는 함수
    {
        string[] lines = csvText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 헤더 행을 찾을 때까지 반복, 왼쪽끝으로 올리면 할 필요는 없음
        int headerIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(lines[i].TrimStart(',')))//공백이지않은 문자열 찾기
            {
                headerIndex = i;
                break;
            }
        }

        if (headerIndex == -1 || lines.Length <= headerIndex + 1)
        {
            Debug.LogError("CSV 파일에서 헤더 또는 데이터 행을 찾을 수 없습니다.");
            return;
        }

        for (int i = headerIndex + 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            if (values.Length < 4) continue; //한 아이템당 몇개의 파라미터를 가져올지 정하는 곳

            int dataStartIndex = 0;
            for (int j = 0; j < values.Length; j++)
            {
                if (!string.IsNullOrWhiteSpace(values[j]))
                {
                    dataStartIndex = j;
                
                    break;
                }
            }
        
            ItemData newItemData = ScriptableObject.CreateInstance<ItemData>();
            newItemData.itemID = int.Parse(values[dataStartIndex + 0]);
            newItemData.itemName = values[dataStartIndex + 1].Trim();
            newItemData.itemDescription = values[dataStartIndex + 2].Trim();
            newItemData.icon = Resources.Load<Sprite>($"InventoryIcon/{newItemData.itemName.Replace(" ", "_").ToLower()}_icon");

            itemDatabase.Add(newItemData.itemID, newItemData);
        }

        Debug.Log("웹에서 " + itemDatabase.Count + "개의 아이템 데이터를 성공적으로 로드했습니다.");
    }


    public static ItemData GetItemData(int itemID)//아이템이 시스템에 있는지 확인하는 함수
    {
        if(itemDatabase.TryGetValue(itemID, out ItemData data))
        {
            return data;
        }

        Debug.LogError($"Item ID {itemID}가 아이템 베이스에 없습니다!");
        return null;
    }

    public void AddItem(ItemData itemData, int quantity =1)
    {
        if (itemData == null) return;

        ItemInInventory newItem = new ItemInInventory(itemData.itemID, quantity);
        inventoryData.Add(newItem);

        //RefreshInventoryUI();

        Debug.Log($"[획득]{itemData.itemName}, {quantity}개");
    }
    public void RefreshInventoryUI()
    {
        for(int i =0; i< itemslot.Length; i++)//슬롯초기화
        {
            itemslot[i].ClearSlot();
        }
        for(int i =0;i<inventoryData.Count;i++)
        {
            if(i>=itemslot.Length)
            {
                Debug.LogWarning("인벤토리 슬롯부족");
                break;
            }
            ItemInInventory itemInInv = inventoryData[i];
            ItemData itemData = GetItemData(itemInInv.itemID);

            if(itemData != null)
            {
                itemslot[i].SetItem(
                    itemData.itemName,
                    itemInInv.quantity,
                    itemData.icon,
                    itemData.itemDescription,
                    i
                    );
            }
            if(i==selectedItemIndex)
            {
                itemslot[i].selectedShader.SetActive(true);
                itemslot[i].thisItemSelected = true;
                EnterScreen();
            }
        }

    }

  /*  public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        for(int i = 0; i < itemslot.Length; i++)
        {
            if(itemslot[i].isFull == false)
            {
                itemslot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }
    }*/

    public void DeselectAllSlots()
    {
        /*for (int i = 0; i < itemslot.Length; i++)
        {
            itemslot[i].selectedShader.SetActive(false);
            itemslot[i].thisItemSelected = false;
        }*/
        for(int i =0;i<itemslot.Length;i++)
        {
            itemslot[i].Deselect();
        }
        selectedItemIndex = -1;
    }

    public void EnterScreen()
    {
        if(selectedItemIndex != -1 && selectedItemIndex < inventoryData.Count)
        {
            ItemInInventory itemInInv = inventoryData[selectedItemIndex];
            ItemData itemData = GetItemData(itemInInv.itemID);

            if(itemData != null)
            {
                ScreenName.text = itemData.itemName;
                ScreenDescription.text = itemData.itemDescription;
                ScreenImage.sprite = itemData.icon;
                return;
            }
        }
        /*
        for (int i = 0;i < 8; i++)
        {
            if (itemslot[i].thisItemSelected == true)
            {
                ScreenName.text = itemslot[i].itemName;
                ScreenDescription.text = itemslot[i].Description;
                ScreenImage.sprite = itemslot[i].itemSprite;
                break;
            }
            
        }*/
        
    }

    //itemslot에서 넘어온 함수들, 정리해야함
    public void HandleSlotClick(int itemIndexInInventory)
    {
        if(itemIndexInInventory<0||itemIndexInInventory >= inventoryData.Count)
        {
            DeselectAllSlots();
            return;
        }
        DeselectAllSlots();

        selectedItemIndex = itemIndexInInventory;

        itemslot[itemIndexInInventory].selectedShader.SetActive(true);
        itemslot[itemIndexInInventory].thisItemSelected = true;

        EnterScreen();
    }

    //우클릭 사용시 아이템 사용 및 소비 및 장착 여부 확인

    public void InventoryBtn()
    {
        if(menuActivated)//인벤토리 열때
        {
            InventoryMenu.SetActive(true);
            menuActivated = false;
            //RefreshInventoryUI();
        }
        else//인벤토리 닫을때
        {
            InventoryMenu.SetActive(false);
            menuActivated = true;
        }
    }
}
