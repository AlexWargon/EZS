using UnityEngine;

[EcsComponent]
public struct TransformComponent
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public Vector3 right;
    public Vector3 forward;
}