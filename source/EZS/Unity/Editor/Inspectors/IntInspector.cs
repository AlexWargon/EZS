using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class IntInspector : TypeInspector<int>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            return EditorGUI.IntField(rect, $"element [{index}]", (int)element);
        }

        protected override object DrawInternal(string fieldName, ref int field)
        {
            return EditorGUILayout.IntField($"    {fieldName}", field);
        }

        protected override int DrawGenericInternal(string fieldName, ref int field) {
            return EditorGUILayout.IntField($"    {fieldName}", field);
        }
    }
}