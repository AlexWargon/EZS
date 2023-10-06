using UnityEngine;


[EcsComponent]
public struct StaticTag{}
[EcsComponent]
public struct TransformComponent {
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
}
[EcsComponent]
public struct TransformRef {
    public UnityEngine.Transform value;
}
