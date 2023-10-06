using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class EntityInspector : TypeInspector<Entity>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Entity field)
        {
            EditorGUILayout.BeginVertical();

            if (field.IsNULL() || field.World == null)
            {
                EditorGUILayout.LabelField($"    {fieldName} : NULL ENTITY");
                EditorGUILayout.EndVertical();
                return field;
            }
            if (field.Has<View>())
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("    View", field.Get<View>().Value, typeof(MonoEntity), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField($"    {fieldName}");
                EditorGUILayout.IntField("    Entity ID", field.id);
                EditorGUILayout.IntField("    Entity GEN", field.InternalGetGeneration());
            }

            EditorGUILayout.EndVertical();
            return field;
        }

        protected override Entity DrawGenericInternal(string fieldName, ref Entity field) {
            EditorGUILayout.BeginVertical();

            if (field.IsNULL() || field.World == null)
            {
                EditorGUILayout.LabelField($"    {fieldName} : NULL ENTITY");
                EditorGUILayout.EndVertical();
                return field;
            }
            if (field.Has<View>())
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("    View", field.Get<View>().Value, typeof(MonoEntity), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField($"    {fieldName}");
                EditorGUILayout.IntField("    Entity ID", field.id);
                EditorGUILayout.IntField("    Entity GEN", field.InternalGetGeneration());
            }

            EditorGUILayout.EndVertical();
            return field;
        }
    }
}