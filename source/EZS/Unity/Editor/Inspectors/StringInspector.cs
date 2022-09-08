using System;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class StringInspector : TypeInspector<string>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
           return EditorGUI.TextField(rect, $"element [1]", element as string);
        }

        protected override object Draw(string fieldName, ref string field) {

            var newValue = EditorGUILayout.TextField (fieldName, field);
            return newValue;
        }
    }
}