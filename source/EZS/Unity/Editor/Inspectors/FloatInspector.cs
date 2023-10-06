using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Wargon.ezs.Unity
{
    public class FloatInspector : TypeInspector<float>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref float field)
        {
            return EditorGUILayout.FloatField($"    {fieldName}", field);
        }

        protected override float DrawGenericInternal(string fieldName, ref float field) {
            return EditorGUILayout.FloatField($"    {fieldName}", field);
        }
    }
}