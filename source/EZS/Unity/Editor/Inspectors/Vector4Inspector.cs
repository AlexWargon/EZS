using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Vector4Inspector : TypeInspector<Vector4>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Vector4 field)
        {
            return EditorGUILayout.Vector4Field($"    {fieldName}", field);
        }

        protected override Vector4 DrawGenericInternal(string fieldName, ref Vector4 field) {
            return EditorGUILayout.Vector4Field($"    {fieldName}", field);
        }
    }
}