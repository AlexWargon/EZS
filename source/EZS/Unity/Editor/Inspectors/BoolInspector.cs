using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class BoolInspector : TypeInspector<bool>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            throw new System.NotImplementedException();
        }

        protected override object Draw(string fieldName, ref bool item)
        {
            return EditorGUILayout.Toggle($"    {fieldName}", item);
        }
    }
}