using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class UnityObjectInpector : TypeInspector<Object>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            return EditorGUI.ObjectField(rect, element as Object, element.GetType(), true);
        }

        protected override object Draw(string fieldName, ref Object field)
        {
            return EditorGUILayout.ObjectField($"    {fieldName}", field, field.GetType(), true);
        }
    }
}