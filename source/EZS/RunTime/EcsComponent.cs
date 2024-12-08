using System;
using Wargon.ezs;

[Serializable]
[AttributeUsage(AttributeTargets.Struct)]
public sealed class EcsComponentAttribute : Attribute
{
    public EcsComponentAttribute()
    {
    }
}
[AttributeUsage(AttributeTargets.Field)]
public sealed class StringHashAttribute : Attribute {
    public StringHashAttribute() {
        
    }
}

public sealed class SwapComponentAttribute : Attribute {
    public Type swapTarget;
    public SwapComponentAttribute(Type swap) {
        swapTarget = swap;
    }
}

