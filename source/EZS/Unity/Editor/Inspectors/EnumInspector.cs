using System;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class EnumInspector : TypeInspector<Enum>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            throw new NotImplementedException();
        }

        protected override object Draw(string fieldName, ref Enum field)
        {
            return EditorGUILayout.EnumPopup($"    {fieldName}", field);
        }
    }
}