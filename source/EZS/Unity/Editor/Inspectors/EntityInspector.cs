using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class EntityInspector : TypeInspector<Entity>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            throw new System.NotImplementedException();
        }

        protected override object Draw(string fieldName, ref Entity field)
        {
            EditorGUILayout.BeginVertical();

            if (field.IsDead() || field.world == null)
            {
                EditorGUILayout.LabelField($"    {fieldName} : NULL ENTITY");
                EditorGUILayout.EndVertical();
                return field;
            }
            if (field.Has<TransformRef>())
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("    View", field.Get<TransformRef>().Value, typeof(Transform), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField($"    {fieldName}");
                EditorGUILayout.IntField("    Entity ID", field.id);
                EditorGUILayout.IntField("    Entity GEN", field.generation);
            }

            EditorGUILayout.EndVertical();
            return field;
        }
    }
}