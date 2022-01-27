using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wargon.ezs.Unity
{
    public static class ComponentInspector
    {
        private static bool remove;
        private static int Count;
        private static readonly Dictionary<Type, ITypeInspector> inspectors;
        
        static ComponentInspector()
        {
            inspectors = new Dictionary<Type, ITypeInspector>();
            CrateInspectors();
        }

        private static void CrateInspectors()
        {
            var assembly = Assembly.GetAssembly(typeof(ITypeInspector));
            foreach (var type in assembly.GetTypes())
                if (typeof(ITypeInspector).IsAssignableFrom(type) && type != typeof(TypeInspector<>) &&
                    !type.IsInterface)
                {
                    var inspector = Activator.CreateInstance(type);
                    var genericType = inspector.GetType().GetField("FieldType").GetValue(inspector) as Type;
                    inspectors.Add(genericType!, inspector as ITypeInspector);
                }
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) return true;
                toCheck = toCheck.BaseType;
            }

            return false;
        }

        private static ITypeInspector NewInspector(Type type)
        {
            var inspectorType = typeof(TypeInspector<>);
            return Activator.CreateInstance(inspectorType.MakeGenericType(type)) as ITypeInspector;
        }

        private static ITypeInspector GetInspector(Type type)
        {
            return inspectors[type];
        }

        public static void DrawComponentBox(MonoEntity entity, int index, MonoEntity[] manyEntities, int count)
        {
            Count = count;
            if (entity.ComponentsCount < index) return;
            object component;

            Type type;
            if (entity.runTime)
            {
                var componentTypeID = entity.Entity.GetEntityData().componentTypes.ElementAt(index);
                var pool = entity.Entity.world.ComponentPools[componentTypeID];
                component = pool.Get(entity.Entity.id);
                type = pool.ItemType;
                EntityGUI.Vertical(EntityGUI.GetColorStyle(type), () => { DrawRunTimeMode(entity, component, pool); });
            }
            else
            {
                component = entity.Components[index];
                if (component == null) return;
                type = component.GetType();
                EntityGUI.Vertical(EntityGUI.GetColorStyle(type), () => { DrawEditorMode(entity, index); });
            }

            if (remove)
                Remove(entity, index, manyEntities);
        }

        private static void RemoveBtn()
        {
            if (GUILayout.Button(new GUIContent("✘", "Remove"), EditorStyles.miniButton, GUILayout.Width(21),
                GUILayout.Height(14)))
                remove = true;
        }

        private static void DrawTypeField(object component, FieldInfo field)
        {
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;

            if (fieldType == typeof(Object) || fieldType.IsSubclassOf(typeof(Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    fieldValue =
                        EditorGUILayout.ObjectField($"    {field.Name}", fieldValue as Object, fieldType, true);
                    component.GetType().GetField(field.Name).SetValue(component, fieldValue);
                });
                return;
            }

            EntityGUI.Horizontal(() => SetFieldValue(fieldValue, field.Name, component));
        }

        private static void DrawTypeFieldRunTime(object component, FieldInfo field)
        {
            if (component == null) return;
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;

            if (fieldType == typeof(Object) || fieldType.IsSubclassOf(typeof(Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    fieldValue =
                        EditorGUILayout.ObjectField($"    {field.Name}", fieldValue as Object, fieldType, true);
                    component.GetType().GetField(field.Name).SetValue(component, fieldValue);
                });
                return;
            }

            EntityGUI.Horizontal(() => SetFieldValue(fieldValue, field.Name, component));
        }

        private static void SetFieldValue(object fieldValue, string fieldName, object component)
        {
            if (fieldValue == null) return;
            var fieldType = fieldValue.GetType();
            if (inspectors.ContainsKey(fieldType))
            {
                if (typeof(IEnumerable).IsAssignableFrom(fieldType))
                    fieldValue = GetInspector(typeof(IEnumerable)).DrawIn(fieldName, fieldValue);
                else
                    fieldValue = GetInspector(fieldValue.GetType()).DrawIn(fieldName, fieldValue);
            }

            component.GetType().GetField(fieldName).SetValue(component, fieldValue);
        }

        public static void SetCollectionElement(object element, string fieldName, Type elementType)
        {
            if (elementType == typeof(Object) || elementType.IsSubclassOf(typeof(Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    element = EditorGUILayout.ObjectField($"    {fieldName}", element as Object, elementType, true);
                });
                return;
            }

            if (element == null) return;
            var fieldType = element.GetType();
            if (inspectors.ContainsKey(fieldType))
            {
                if (typeof(IEnumerable).IsAssignableFrom(fieldType))
                    element = GetInspector(typeof(IEnumerable)).DrawIn(fieldName, element);
                else
                    element = GetInspector(element.GetType()).DrawIn(fieldName, element);
            }
        }

        private static void Remove(MonoEntity entity, int index, MonoEntity[] manyEntities)
        {
            var type = entity.Components[index].GetType();
            if (Count > 1)
            {
                for (var i = 0; i < Count; i++)
                {
                    var e = manyEntities[i];
                    if (entity.runTime)
                    {
                        e.Entity.RemoveByTypeID(e.Entity.GetEntityData().componentTypes.ElementAt(index));
                        remove = false;
                    }
                    else
                    {
                        if (e.Components.HasType(type))
                            e.Components.RemoveAt(index);
                        remove = false;
                    }
                }
            }
            else
            {
                if (entity.runTime)
                {
                    entity.Entity.RemoveByTypeID(entity.Entity.GetEntityData().componentTypes.ElementAt(index));
                    remove = false;
                }
                else
                {
                    entity.Components.RemoveAt(index);
                    remove = false;
                }
            }
        }

        private static void Remove(Entity entity, int index)
        {
            entity.RemoveByTypeID(entity.GetEntityData().componentTypes.ElementAt(index));
            remove = false;
        }

        private static void DrawEditorMode(MonoEntity entity, int index)
        {
            var component = entity.Components[index];
            if (component == null)
            {
                entity.Components = entity.Components.Where(x => x != null).ToList();
                return;
            }

            var type = component.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            EntityGUI.Horizontal(() =>
            {
                EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);
                RemoveBtn();
            });

            foreach (var field in fields)
                DrawTypeField(component, field);
        }

        private static void DrawRunTimeMode(MonoEntity entity, object component, IPool pool)
        {
            if (remove) return;

            var type = pool.ItemType;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);
            RemoveBtn();
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < fields.Length; i++)
                DrawTypeFieldRunTime(component, fields[i]);

            pool.Set(component, entity.Entity.id);
        }


        public static void DrawComponentBox(Entity entity, int index)
        {
            if (entity.GetEntityData().componentTypes.Count <= index) return;
            var componentTypeID = entity.GetEntityData().componentTypes.ElementAt(index);
            var pool = entity.world.ComponentPools[componentTypeID];
            var component = pool.Get(entity.id);
            var type = component.GetType();
            EntityGUI.Vertical(EntityGUI.GetColorStyle(type),
                () => { DrawRunTimeMode2(entity, component, type, pool); });

            if (remove)
                Remove(entity, index);
        }

        private static void DrawRunTimeMode2(Entity entity, object component, Type type, IPool pool)
        {
            if (remove) return;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(component == null ? $"ERROR! {type.Name} IS NULL" : $"{type.Name}",
                EditorStyles.boldLabel);
            RemoveBtn();
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < fields.Length; i++)
                DrawTypeFieldRunTime(component, fields[i]);

            pool.Set(component, entity.id);
        }
    }
}