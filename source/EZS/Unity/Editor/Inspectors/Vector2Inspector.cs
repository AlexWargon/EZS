using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Vector2Inspector : TypeInspector<Vector2>
    {
        protected override object Draw(string fieldName, ref Vector2 field)
        {
            return EditorGUILayout.Vector2Field($"    {fieldName}", field);
        }
    }
}