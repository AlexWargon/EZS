using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class AnimationCurveInspector : TypeInspector<AnimationCurve>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            return null;
        }

        protected override object Draw(string fieldName, ref AnimationCurve field)
        {
            return EditorGUILayout.CurveField($"    {fieldName}", field);
        }
    }
}