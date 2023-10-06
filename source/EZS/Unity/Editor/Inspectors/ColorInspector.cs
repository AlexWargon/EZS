using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class ColorInspector : TypeInspector<Color>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Color field)
        {
            return EditorGUILayout.ColorField($"    {fieldName}", field);
        }

        protected override Color DrawGenericInternal(string fieldName, ref Color field) {
            return EditorGUILayout.ColorField($"    {fieldName}", field);
        }
    }
}