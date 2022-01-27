using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class GradientInspector : TypeInspector<Gradient>
    {
        protected override object Draw(string fieldName, ref Gradient field)
        {
            return EditorGUILayout.GradientField($"    {fieldName}", field);
        }
    }
}