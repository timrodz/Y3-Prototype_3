/// <summary>
/// Items must be defined by name and then assigning their values via bitwise operation
/// This allows multiple Item selection (e.g. Honey | Berry)

/// </summary>

[System.Flags]
public enum Item
{
    None = (1 << 0),
    Honey = (1 << 1),
    Berry = (1 << 2),
    Wood = (1 << 3)
}