using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Color32Inspector : TypeInspector<Color32>
    {
        protected override object Draw(string fieldName, ref Color32 field)
        {
            return EditorGUILayout.ColorField($"    {fieldName}", field);
        }
    }
}