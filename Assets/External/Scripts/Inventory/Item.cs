using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int quantity;
    public Sprite sprite;
    InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }
    private void Update()
    {
        plusing();
    }

    void plusing()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //inventoryManager.AddItem(itemName, quantity, sprite);
        }
    }
}
