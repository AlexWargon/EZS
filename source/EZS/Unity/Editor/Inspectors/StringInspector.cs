using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class StringInspector : TypeInspector<string>
    {
        
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
           return EditorGUI.TextField(rect, $"element [{index}]", element as string);
        }

        protected override object DrawInternal(string fieldName, ref string field) {
            return EditorGUILayout.TextField (fieldName, field);
        }

        protected override string DrawGenericInternal(string fieldName, ref string field) {
            return EditorGUILayout.TextField (fieldName, field);
        }
    }
}