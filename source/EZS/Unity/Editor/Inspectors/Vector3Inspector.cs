using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Vector3Inspector : TypeInspector<Vector3>
    {
        protected override object Draw(string fieldName, ref Vector3 field)
        {
            return EditorGUILayout.Vector3Field($"    {fieldName}", field);
        }
    }
}