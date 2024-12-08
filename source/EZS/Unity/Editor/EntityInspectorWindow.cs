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
            EntityGUI.Init();
            DrawInspector();
        }
        private void Update()
        {
            Repaint();
        }

        private static void DrawInspector()
        {
            if(!Application.isPlaying) return;
            //if(Entity==default) return;
            if (Entity.IsNULL())
            {
                GUILayout.Label("ENTITY DEAD ");
                return;
            }
            ref var data = ref Entity.GetEntityData();

            var componentsCount = data.ComponentsCount;
            GUILayout.BeginVertical(GUI.skin.box);

            //EditorGUILayout.LabelField($"Entity ID : {entity.id.ToString()}");
            EditorGUILayout.LabelField($"Components : [{componentsCount}]", EditorStyles.boldLabel);
            unsafe {
                var index = 0;
                foreach (var componentTypeID in data.archetype.Mask) {
                    ComponentInspectorInternal.DrawComponentBox(Entity, index, componentTypeID);
                    index++;
                }
            }
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
            buttonStyle = GUI.skin.button;
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
            unsafe {
                foreach (var dataComponentType in data.archetype.Mask)
                {
                    var pool = world.GetPoolByID(dataComponentType);
                    var component = pool.GetBoxed(entity.id);
                    if (component != null)
                        name += $"; {component.GetType()}";
                }
            }

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
            if(entity.IsNULL()) return;
            if (name == string.Empty)
                EntityToString(entity);
            if (GUILayout.Button(name, buttonStyle)) OnFocusEntity?.Invoke(entity);
        }
    }
}