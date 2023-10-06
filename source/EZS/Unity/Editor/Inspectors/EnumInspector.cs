using System;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class EnumInspector : TypeInspector<Enum>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Enum field)
        {
            return EditorGUILayout.EnumPopup($"    {fieldName}", field);
        }

        protected override Enum DrawGenericInternal(string fieldName, ref Enum field) {
            return EditorGUILayout.EnumPopup($"    {fieldName}", field);
        }
    }
    
    public class EnumFlagInspector : TypeInspector<Enum>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Enum field)
        {
            return EditorGUILayout.EnumFlagsField($"    {fieldName}", field);
        }

        protected override Enum DrawGenericInternal(string fieldName, ref Enum field) {
            return EditorGUILayout.EnumFlagsField($"    {fieldName}", field);
        }
    }
}