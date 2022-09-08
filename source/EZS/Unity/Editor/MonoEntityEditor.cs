using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using PopupWindow = UnityEditor.PopupWindow;

namespace Wargon.ezs.Unity {

    [CustomEditor(typeof(MonoEntity), true), CanEditMultipleObjects]
    public class MonoEntityEditor : Editor 
    {
        private MonoEntity monoEntity;
        private MonoEntity[] manyEntities = new MonoEntity[16];
        private bool flowed = true;
        private GUIContent addComponentText;
        private GUIStyle addComponentButtonStyle;
        private Rect addButtonRect;
        //private bool editMany;
        private int entitiesCount;
        private UnityEngine.Object currentDragObject = null;

        public override bool RequiresConstantRepaint() {
            if (monoEntity == null) return false;
            if (monoEntity.Entity == null) return false;
            return monoEntity.runTime || base.RequiresConstantRepaint();
        }

        public override void OnInspectorGUI()
        {
            EntityGUI.Init();
            //editMany = targets.Length > 1;
            entitiesCount = targets.Length;
            for (var i = 0; i < entitiesCount; i++)
            {
                manyEntities[i] = (MonoEntity) targets[i];
            }
            monoEntity = (MonoEntity) targets[0];
            
            
            EditorGUI.BeginChangeCheck();
            addComponentText = new GUIContent("Add Component");
            addComponentButtonStyle = GUI.skin.button;

            if (monoEntity.runTime)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(new GUIContent("Kill Entity"),GUILayout.Width(154), GUILayout.Height(24)))
                    monoEntity.Entity.Destroy();
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                monoEntity.destroyComponent = EditorGUILayout.Toggle("Destroy MonoBeh", monoEntity.destroyComponent);
                monoEntity.destroyObject = EditorGUILayout.Toggle("Destroy GO", monoEntity.destroyObject);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Run Time", monoEntity.runTime ? "✔" : "✘", EditorStyles.largeLabel);
            EditorGUILayout.LabelField(monoEntity.runTime ? $"ID:{monoEntity.Entity.id.ToString()}" : "ID:X");
            EditorGUILayout.EndHorizontal();
            
            
            EntityGUI.Vertical(GUI.skin.box, () =>
            {
                if (monoEntity.runTime)
                {
                    if (monoEntity.Entity.IsDead())
                    {
                        EditorGUILayout.LabelField("ENTITY DEAD", EditorStyles.whiteLargeLabel);
                        return;
                    }
                }
                
                EntityGUI.Horizontal(() =>
                {
                    flowed = EditorGUILayout.Foldout(flowed, $"ECS Components [{monoEntity.ComponentsCount.ToString()}]");
                    
                    if (GUILayout.Button(new GUIContent("Clear", "Remove All Components")))
                        RemoveAll();
                });
                
                if (!flowed) return;
                addButtonRect = GUILayoutUtility.GetRect(addComponentText, addComponentButtonStyle);
                if (GUI.Button(addButtonRect, addComponentText, addComponentButtonStyle))
                {
                    addButtonRect.y -= 20f;
                    PopupWindow.Show(addButtonRect, new ComponentSearchPopup(AddComponent, manyEntities, entitiesCount));
                }
                DrawComponents();
            });

            DragAndDropComponent();

            EditorGUILayout.LabelField("EZS", EditorStyles.whiteMiniLabel);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(target);
                
        }

        private void DragAndDropComponent()
        {
            var dragObj = DragAndDrop.objectReferences;
            if (dragObj.Length > 1)
            {
                foreach (var o in dragObj)
                {
                    currentDragObject = o;
                }
            }


            if (Event.current.rawType == EventType.DragExited)
            {
                dragObj = DragAndDrop.objectReferences;
                foreach (var o in dragObj)
                {
                    currentDragObject = o;
                }

                if (dragObj.Length < 1)
                    currentDragObject = null;

                DragAndDrop.AcceptDrag();
                if (currentDragObject != null)
                {
                    if (ComponentTypesList.NamesHash.Contains(currentDragObject.name))
                    {

                        AddComponent(currentDragObject.name, monoEntity);
                    }

                }
            }
        }


        private static object NewObject(Type type) {
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
            monoEntity.Components.Clear();
        }

        private void AddComponent(string componentName, MonoEntity entity) {
            if (entity.runTime)
                AddComponentRuntime(componentName,entity);
            else
                AddComponentEditor(componentName,entity);
            EditorUtility.SetDirty(entity);
        }
        
        private void AddComponentEditor(string componentName, MonoEntity entity)
        {
            var type = GetComponentType(componentName);
            if (entity.Components.HasType(type)) {
                //Debug.LogError($"ENTITY ALREADY HAS '{type}' COMPONENT");
                return;
            }

            var resolver = NewObject(type);
            //Debug.Log(resolver.GetType());
            entity.Components.Add(resolver);
        }
        
        private void AddComponentRuntime(string componentName, MonoEntity entity)
        {
            var type = GetComponentType(componentName);
            if (entity.Entity.GetEntityData().componentTypes.Contains(ComponentTypeMap.GetID(type))) {
                Debug.LogError($"ENTITY ALREADY HAS '{type}' COMPONENT");
                return;
            }
            
            var component= NewObject(type);
            entity.Entity.AddBoxed(component);
            entity.Components.Add(component);
            EditorUtility.SetDirty(entity);
        }

        private void DrawComponents() 
        {
            //Resolve(monoEntity);
            for (var index = 0; index < monoEntity.ComponentsCount; index++)
            {
                ComponentInspector.DrawComponentBox(monoEntity, index, manyEntities, entitiesCount);
            }
        }

        private void Resolve(MonoEntity monoEntity)
        {
            monoEntity.Components.RemoveAll(x => x == null);
        }
    }
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
    public static class ListExtension
    {
        public static bool HasType(this System.Collections.IList list, Type whatHas)
        {
            var i = 0;
            var count = list.Count;
            for (i = 0; i < count; i++)
            {
                if (list[i] == null) return false;
                if (list[i].GetType() == whatHas)
                    return true;
            }
            return false;
        }
    }
}

