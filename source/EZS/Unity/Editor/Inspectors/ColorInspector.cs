using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class ColorInspector : TypeInspector<Color>
    {
        protected override object Draw(string fieldName, ref Color field)
        {
            return EditorGUILayout.ColorField($"    {fieldName}", field);
        }
    }
}