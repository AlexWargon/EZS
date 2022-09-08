using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class IntInspector : TypeInspector<int>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            return EditorGUI.IntField(rect, (int)element);
        }

        protected override object Draw(string fieldName, ref int field)
        {
            return EditorGUILayout.IntField($"    {fieldName}", field);
        }
    }
}