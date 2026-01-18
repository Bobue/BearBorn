using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string Description;

    public TMP_Text quantityText;
    public Image itemImage;
    

    public GameObject selectedShader;
    public bool thisItemSelected;
    InventoryManager inventoryManager;

    [HideInInspector]
    public int itemIndexInInventory = -1;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        ClearSlot();//슬롯초기화
    }

    public void ClearSlot()
    {
        itemName = "";
        quantity = 0;
        Description = "";
        isFull = false;

        itemIndexInInventory = -1;

        if (quantityText != null) quantityText.text = "";
        if (itemImage != null) itemImage.enabled = false;

        Deselect();
    }
    public void SetItem(string name, int qty, Sprite sprite, string description, int index)
    {
        this.itemName = name;
        this.quantity = qty;
        this.itemSprite = sprite;
        this.Description = description;
        this.isFull = true;

        this.itemIndexInInventory = index;

        if(quantityText != null)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
        }
        if(itemImage != null)
        {
            itemImage.sprite = itemSprite;
            itemImage.enabled = true;
        }
    }
    public void Deselect()
    {
        if(selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        thisItemSelected = false;
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if(isFull&&itemIndexInInventory != -1)
        {
            inventoryManager.HandleSlotClick(itemIndexInInventory);
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            inventoryManager.EnterScreen();
        }
    }
    public void OnRightClick()
    {
        //사용함수 구현해야할수도?버튼식으로도 할거같긴한데 일단 기획이 나와봐야 할거같음
    }
}
