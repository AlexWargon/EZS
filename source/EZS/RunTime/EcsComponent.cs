using System;
[Serializable]
[AttributeUsage(AttributeTargets.Struct)]
public sealed class EcsComponentAttribute : Attribute
{
    public EcsComponentAttribute()
    {
    }
}
