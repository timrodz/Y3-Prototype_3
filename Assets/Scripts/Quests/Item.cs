/// <summary>
/// Items must be defined by name and then assigning their values via bitwise operation
// / This allows multiple ItemHasg selection (e.g. Honey | Berry)

/// </summary>

[System.Flags]
public enum ItemType
{
    None = (1 << 0),
    Honey = (1 << 1),
    Berry = (1 << 2),
    Wood = (1 << 3),
    Carrot = (1 << 4)
}