using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    [InitializeOnLoad]
    public class EcsDebugWindow : EditorWindow
    {
        private static EntityInspectorWindow inspectorWindow;
        private readonly List<EntityView> EntityDrawers = new List<EntityView>();
        private World _world;

        private int entityCountOld;
        private SearchField filterComponentsField;
        private string filterComponentString;
        private Entity FocusedEntity;
        private Vector2 scrollPos;

        private void OnEnable()
        {
            filterComponentsField = new SearchField();
            filterComponentsField.SetFocus();
            filterComponentString = string.Empty;
        }

        private void OnGUI()
        {
            var world = MonoConverter.GetWorld();
            if (world is not { Alive: true })
            {
                GUILayout.Label("NO WORLDS");

                return;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width),
                GUILayout.Height(position.height));
            DrawEntityList(world);
            EditorGUILayout.EndScrollView();
        }

        private void OnInspectorUpdate()
        {
            if (_world == null) return;
            if (entityCountOld != _world.totalEntitiesCount)
                Repaint();
        }

        [MenuItem("EZS/EcsDebugWindow")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EcsDebugWindow), false, "Entities", true);
            inspectorWindow = (EntityInspectorWindow) GetWindow(typeof(EntityInspectorWindow), false, "Entity Inspector", true);
        }

        private static void OnFocusEntity(Entity entity)
        {
            EntityInspectorWindow.Entity = entity;
        }

        private void DrawEntityList(World world)
        {
            filterComponentString =
                filterComponentsField.OnGUI(EditorGUILayout.GetControlRect(), filterComponentString);
            _world = world;
            var entities = _world.entities;
            entityCountOld = _world.totalEntitiesCount;
            if (EntityDrawers.Count < _world.totalEntitiesCount)
                while (EntityDrawers.Count < _world.totalEntitiesCount)
                    EntityDrawers.Add(new EntityView(_world, OnFocusEntity));
            for (var i = 0; i < _world.totalEntitiesCount; i++)
                EntityDrawers[i].Draw(entities[i], filterComponentString);
        }
    }
}