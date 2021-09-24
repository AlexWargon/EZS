using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wargon.ezs.Unity {

    [CustomEditor(typeof(MonoEntity), true), CanEditMultipleObjects]
    public class MonoEntityEditor : Editor 
    {
        private MonoEntity entity;
        private bool flowed = true;
        private GUIContent addComponentText;
        private GUIStyle addComponentButtonStyle;
        private Rect addButtonRect;
        private void Awake() 
        {
            EntityGUI.Init();
            ComponentTypesList.Init();
        }
        
        public override void OnInspectorGUI() 
        {
            //DrawDefaultInspector();
            entity = (MonoEntity)target;
            addComponentText = new GUIContent("Add Component");
            addComponentButtonStyle = GUI.skin.button;
            EditorGUI.BeginChangeCheck();

            if (entity.runTime)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(new GUIContent("Kill Entity"),GUILayout.Width(154), GUILayout.Height(24)))
                    entity.Entity.Destroy();
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                entity.destroyComponent = EditorGUILayout.Toggle("Destroy MonoBeh", entity.destroyComponent);
                entity.destroyObject = EditorGUILayout.Toggle("Destroy GO", entity.destroyObject);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Run Time", entity.runTime ? "✔" : "✘", EditorStyles.largeLabel);
            EditorGUILayout.LabelField(entity.runTime ? $"ID:{entity.Entity.id.ToString()}" : "ID:X");
            EditorGUILayout.EndHorizontal();
            
            
            EntityGUI.Vertical(GUI.skin.box, () =>
            {
                if (entity.runTime)
                    if (entity.Entity.IsDead())
                    {
                        EditorGUILayout.LabelField("ENTITY DEAD", EditorStyles.whiteLargeLabel);
                        return;
                    }
                EntityGUI.Horizontal(() =>
                {
                    flowed = EditorGUILayout.Foldout(flowed, $"ECS Components [{entity.ComponentsCount.ToString()}]");
                    
                    if (GUILayout.Button(new GUIContent("Clear", "Remove All Components")))
                        RemoveAll();
                });

                if (flowed)
                {
                    addButtonRect = GUILayoutUtility.GetRect(addComponentText, addComponentButtonStyle);
                    if (GUI.Button(addButtonRect, addComponentText, addComponentButtonStyle))
                    {
                        addButtonRect.y -= 20f;
                        PopupWindow.Show(addButtonRect, new ComponentSearchPopup(AddComponent));
                    }
                    
                    DrawComponents();
                }


            });
            EditorGUILayout.LabelField($"EZS", EditorStyles.whiteMiniLabel);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(target);
        }

        private object NewObject(Type type) {
            return Activator.CreateInstance(type);
        }

        private Type GetComponentType(string typeName) {
            //return Type.GetType(type + ",Assembly-CSharp", true);
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => Assembly.Load(x.FullName))
                .Select(x => x.GetType(typeName))
                .FirstOrDefault(x => x != null);
            
        }

        private void RemoveAll() {
            entity.Components.Clear();
        }

        private void AddComponent(string componentName) {
            if (entity.runTime)
                AddComponentRuntime(componentName);
            else
                AddComponentEditor(componentName);
        }
        
        private void AddComponentEditor(string componentName)
        {
            var type = GetComponentType(componentName);
            if (entity.Components.HasType(type)) {
                return;
            }

            var resolver = NewObject(type);
            entity.Components.Add(resolver);
        }
        
        private void AddComponentRuntime(string componentName)
        {
            var type = GetComponentType(componentName);
            if (entity.Entity.GetEntityData().componentTypes.Contains(ComponentTypeMap.GetID(type))) {
                Debug.LogError($"ENTITY ALREADY HAS '{type}' COMPONENT");
                return;
            }
            
            var component= NewObject(type);
            entity.Entity.AddBoxed(component);
            entity.Components.Add(component);
        }

        private void DrawComponents() {
            for (var index = 0; index < entity.ComponentsCount; index++)
                ComponentInspector.DrawComponentBox(entity, index);
        }
    }
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}

