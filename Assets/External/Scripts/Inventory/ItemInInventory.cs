using System;

[Serializable]
public struct ItemInInventory
{
    public int itemID;
    public int quantity;

    public ItemInInventory(int id, int qty)
    {
        itemID = id;
        quantity = qty;
    }
}
