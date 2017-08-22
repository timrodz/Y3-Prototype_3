using UnityEngine;

[CreateAssetMenu(fileName = "ItemName", menuName = "Inventory/Item")]
public class ItemInfo : ScriptableObject {

    new public string name = "ItemName";
    public Sprite icon = null;
    public bool defaultItem = false;
    public ItemAction action;


}


public enum ItemAction
{
    ADD,
    REMOVE,
    THROW,
    DROP,
    GIVE
}