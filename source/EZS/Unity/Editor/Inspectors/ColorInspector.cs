using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class ColorInspector : TypeInspector<Color>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            throw new System.NotImplementedException();
        }

        protected override object Draw(string fieldName, ref Color field)
        {
            return EditorGUILayout.ColorField($"    {fieldName}", field);
        }
    }
}