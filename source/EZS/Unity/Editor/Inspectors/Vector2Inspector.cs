using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class Vector2Inspector : TypeInspector<Vector2>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object Draw(string fieldName, ref Vector2 field)
        {
            return EditorGUILayout.Vector2Field($"    {fieldName}", field);
        }
    }
}