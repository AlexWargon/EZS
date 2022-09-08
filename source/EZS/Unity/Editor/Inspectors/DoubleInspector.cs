using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class DoubleInspector : TypeInspector<double>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            throw new System.NotImplementedException();
        }

        protected override object Draw(string fieldName, ref double field)
        {
            return EditorGUILayout.DoubleField($"    {fieldName}", field);
        }
    }
}