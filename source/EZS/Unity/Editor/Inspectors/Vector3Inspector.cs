using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Vector3Inspector : TypeInspector<Vector3>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            return EditorGUI.Vector3Field(rect, $"element [1]", (Vector3)element);
        }

        protected override object Draw(string fieldName, ref Vector3 field)
        {
            return EditorGUILayout.Vector3Field($"    {fieldName}", field);
        }
    }
}