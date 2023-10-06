using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class UnityObjectInspector : TypeInspector<Object>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            return EditorGUI.ObjectField(rect,$"element [{index}]", element as Object, FieldType, true);
        }
        
        protected override object DrawInternal(string fieldName, ref Object field)
        {
            return EditorGUILayout.ObjectField($"    {fieldName}", field, FieldType, true);
        }

        protected override Object DrawGenericInternal(string fieldName, ref Object field) {
            return EditorGUILayout.ObjectField($"    {fieldName}", field, FieldType, true);
        }

        public static UnityObjectInspector New(System.Type type)
        {
            var inspector = new UnityObjectInspector();
            inspector.SetTypeOfField(type);
            return inspector;
        }
    }
}