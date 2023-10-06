using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class BoolInspector : TypeInspector<bool>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref bool item)
        {
            return EditorGUILayout.Toggle($"    {fieldName}", item);
        }

        protected override bool DrawGenericInternal(string fieldName, ref bool field) {
            return EditorGUILayout.Toggle($"    {fieldName}", field);
        }
    }
}