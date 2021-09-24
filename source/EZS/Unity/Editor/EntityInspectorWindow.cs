using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class EntityInspectorWindow : EditorWindow
    {
        public static Entity Entity;

        private void OnGUI()
        {
            DrawInspector();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private static void DrawInspector()
        {
            if (Entity == null) return;
            if (Entity.IsDead())
            {
                GUILayout.Label("ENTITY DEAD ");
                return;
            }
            ref var data = ref Entity.GetEntityData();

            var componentsCount = data.componentsCount;
            GUILayout.BeginVertical(GUI.skin.box);

            //EditorGUILayout.LabelField($"Entity ID : {entity.id.ToString()}");
            EditorGUILayout.LabelField($"Components : [{componentsCount}]", EditorStyles.boldLabel);
            for (var index = 0; index < componentsCount; index++)
                ComponentInspector.DrawComponentBox(Entity, index);
            GUILayout.EndVertical();
        }
    }
    public class EntityView
    {
        private readonly GUIStyle buttonStyle;
        private readonly World world;
        private EntityData data;
        private string name;
        private readonly Action<Entity> OnFocusEntity;

        public EntityView(World world, Action<Entity> onFocusEntity)
        {
            this.world = world;
            buttonStyle = GUI.skin.customStyles[312];
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 12;
            buttonStyle.fixedHeight = 20f;
            data = default;
            name = string.Empty;
            OnFocusEntity = onFocusEntity;
        }

        private void EntityToString(Entity entity)
        {
            name = $" ID:{entity.id}";
            data = entity.GetEntityData();
            foreach (var dataComponentType in data.componentTypes)
            {
                var pool = world.GetPoolByID(dataComponentType);
                var component = pool.Get(entity.id);
                if (component != null)
                    name += $"; {component.GetType()}";
            }

            //return $"Entity{entity.id.ToString()}:{entity.generation.ToString()}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Has(string filter)
        {
            if (filter == null) return true;
            return filter == string.Empty || name.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Entity entity, string filter)
        {
            if (!Has(filter)) return;
            if(entity.IsDead()) return;
            if (name == string.Empty)
                EntityToString(entity);
            if (GUILayout.Button(name, buttonStyle)) OnFocusEntity?.Invoke(entity);
        }
    }
}