using System;
[Serializable]
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class EcsComponentAttribute : Attribute
{
    public EcsComponentAttribute()
    {
    }
}
