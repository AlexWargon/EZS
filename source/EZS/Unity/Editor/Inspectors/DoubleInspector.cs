using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class DoubleInspector : TypeInspector<double>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref double field)
        {
            return EditorGUILayout.DoubleField($"    {fieldName}", field);
        }

        protected override double DrawGenericInternal(string fieldName, ref double field) {
            return EditorGUILayout.DoubleField($"    {fieldName}", field);
        }
    }
}