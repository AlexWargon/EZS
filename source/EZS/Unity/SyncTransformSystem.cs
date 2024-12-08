using System;

[EcsComponent] public struct StaticTag{}
[EcsComponent] public struct TransformComponent : IEquatable<TransformComponent> {
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 scale;
    public UnityEngine.Quaternion rotation;

    public UnityEngine.Vector3 right => rotation * UnityEngine.Vector3.right;
    public UnityEngine.Vector3 down => rotation * UnityEngine.Vector3.right;

    public UnityEngine.Vector3 up {
        get => rotation * UnityEngine.Vector3.up;
        set => rotation = UnityEngine.Quaternion.FromToRotation(UnityEngine.Vector3.up, value);
    }
    public void Rorate(UnityEngine.Vector3 eulers) {
        UnityEngine.Quaternion quaternion = UnityEngine.Quaternion.Euler(eulers.x, eulers.y, eulers.z);
        rotation *= UnityEngine.Quaternion.Inverse(this.rotation) * quaternion * this.rotation;
    }

    public void RotateAround(UnityEngine.Vector3 pos, UnityEngine.Vector3 dir, float angle) {
        
    }

    public bool Equals(TransformComponent other) {
        return position == other.position && scale == other.scale && rotation == other.rotation;
    }
    public static UnityEngine.Vector3 Down(UnityEngine.Quaternion rotation) => rotation * UnityEngine.Vector3.right;
}
[EcsComponent] public struct TransformRef {
    public UnityEngine.Transform value;
}
[EcsComponent] public struct NotSync{}