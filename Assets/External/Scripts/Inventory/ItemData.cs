using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite icon;
}