using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class GradientInspector : TypeInspector<Gradient>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            return null;
        }

        protected override object DrawInternal(string fieldName, ref Gradient field)
        {
            return EditorGUILayout.GradientField($"    {fieldName}", field);
        }

        protected override Gradient DrawGenericInternal(string fieldName, ref Gradient field) {
            return EditorGUILayout.GradientField($"    {fieldName}", field);
        }
    }
}