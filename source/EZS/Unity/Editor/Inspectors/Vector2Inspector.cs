using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Vector2Inspector : TypeInspector<Vector2>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Vector2 field)
        {
            return EditorGUILayout.Vector2Field($"    {fieldName}", field);
        }

        protected override Vector2 DrawGenericInternal(string fieldName, ref Vector2 field) {
            return EditorGUILayout.Vector2Field($"    {fieldName}", field);
        }
    }
    public class Float2Inspector : TypeInspector<float2>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref float2 field)
        {
            var vector = EditorGUILayout.Vector2Field($"    {fieldName}", new Vector2(field.x, field.y));
            return new float2(vector.x, vector.y);
        }

        protected override float2 DrawGenericInternal(string fieldName, ref float2 field) {
            var vector = EditorGUILayout.Vector2Field($"    {fieldName}", new Vector2(field.x, field.y));
            return new float2(vector.x, vector.y);
        }
    }
    public class Float3Inspector : TypeInspector<float3>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref float3 field)
        {
            var vector = EditorGUILayout.Vector3Field($"    {fieldName}", new Vector3(field.x, field.y, field.z));
            return new float3(vector.x, vector.y, field.z);
        }

        protected override float3 DrawGenericInternal(string fieldName, ref float3 field) {
            var vector = EditorGUILayout.Vector3Field($"    {fieldName}", new Vector3(field.x, field.y, field.z));
            return new float3(vector.x, vector.y, field.z);
        }
    }
}