using UnityEditor;

namespace Wargon.ezs.Unity
{
    public class EntityInspector : TypeInspector<Entity>
    {
        protected override object Draw(string fieldName, ref Entity field)
        {
            EditorGUILayout.BeginVertical();
            if (field.IsDead())
            {
                EditorGUILayout.LabelField($"    {fieldName} : NULL");
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
                EditorGUILayout.IntField("    Entity GEN", field.generation);
            }

            EditorGUILayout.EndVertical();
            return field;
        }
    }
}