using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class AnimationCurveInspector : TypeInspector<AnimationCurve>
    {
        protected override object Draw(string fieldName, ref AnimationCurve field)
        {
            return EditorGUILayout.CurveField($"    {fieldName}", field);
        }
    }
}