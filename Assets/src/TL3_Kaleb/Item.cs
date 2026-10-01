using UnityEngine;

public class Item // YOUR superclass name​
{
    public virtual string GetItemId() // virtual = overridable
    {
        return "basic_item"; // RETURN a value
    }
}
