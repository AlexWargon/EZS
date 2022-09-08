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
        private static Dictionary<Type, ITypeInspector> inspectors;
        private static Dictionary<Type, ListInspector> listInspectors;
        private static Dictionary<Type, TextAsset> scriptsAssets;
        private static Dictionary<Type, bool> csFileExist;
        private static bool inited;
        public static void Init()
        {
            if(inited) return;
            inspectors = new Dictionary<Type, ITypeInspector>();
            scriptsAssets = new Dictionary<Type, TextAsset>();
            csFileExist = new Dictionary<Type, bool>();
            listInspectors = new Dictionary<Type, ListInspector>();
            CrateInspectors();
            FillScriptAssets();
            inited = true;
        }
        private static void FillScriptAssets()
        {
            var types = ComponentTypesList.GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                AddScirptAsset(types[i]);
            }
        }
        private static void CrateInspectors()
        {
            var assembly = Assembly.GetAssembly(typeof(ITypeInspector));
            //var newList = new List<ITypeInspector>();
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(ITypeInspector).IsAssignableFrom(type) && type != typeof(TypeInspector<>) && !type.IsInterface)
                {
                    var inspector = Activator.CreateInstance(type)  as ITypeInspector;
                    //Debug.Log($"type {inspector.GetType()} --- generic type {inspector.GetGenericType()}");
                    inspectors.Add(inspector.GetGenericType(), inspector);
                }
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

        public static ITypeInspector GetInspector(Type type)
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
                EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type), () => { DrawRunTimeMode(entity, component, pool); });
            }
            else
            {
                component = entity.Components[index];
                if (component == null) return;
                type = component.GetType();
                EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type), () => { DrawEditorMode(entity, index); });
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
            fieldType = typeof(IList).IsAssignableFrom(fieldType) ? typeof(IList) : fieldType;
            if (fieldType == typeof(Object) || fieldType.IsSubclassOf(typeof(Object)))
            {
                fieldValue = GetInspector(typeof(Object)).DrawIn(fieldName, fieldValue);
            }
            else
            if (inspectors.ContainsKey(fieldType))
            {
                if (typeof(IList).IsAssignableFrom(fieldType))
                {
                    if (GetInspector(typeof(IList)) is ListInspector inspecotr)
                    {
                        inspecotr.Init(fieldValue as IList, fieldValue.GetType().GetElementType());
                        fieldValue = inspecotr.DrawIn(fieldName, fieldValue);
                    }
                }
                
                else
                {
                    fieldValue = GetInspector(fieldValue.GetType()).DrawIn(fieldName, fieldValue);
                    //Debug.Log(fieldValue.GetType());
                }
            }

            component.GetType().GetField(fieldName).SetValue(component, fieldValue);
        }

        public static void SetCollectionElement(Rect rect, object elementValue , int index, string fieldName, Type elementType, object collection)
        {
            if (elementType == typeof(Object) || elementType.IsSubclassOf(typeof(Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    elementValue = EditorGUI.ObjectField(new Rect(rect.x + 10, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight),$"    {fieldName}", elementValue as Object, elementType, true);
                });
                return;
            }

            if (elementValue == null) return;

            elementType = typeof(IList).IsAssignableFrom(elementType) ? typeof(IList) : elementType;
            if (elementType == typeof(Object) || elementType.IsSubclassOf(typeof(Object)))
            {
                elementValue = GetInspector(typeof(Object)).DrawCollectionElement(new Rect(rect.x + 10, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight), elementValue);
            }
            else
            if (inspectors.ContainsKey(elementType))
            {
                if (typeof(IList).IsAssignableFrom(elementType))
                {
                    var inspecotr = (ListInspector) GetInspector(typeof(IList));
                    inspecotr.Init(elementValue as IList, elementValue.GetType().GetElementType());
                    elementValue = inspecotr.DrawCollectionElement(new Rect(rect.x + 10, rect.y, 250, EditorGUIUtility.singleLineHeight), elementValue);
                }
                else
                    elementValue = GetInspector(elementValue.GetType()).DrawCollectionElement(new Rect(rect.x + 10, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight), elementValue);
            }
            collection.GetType().GetProperty("Item").SetValue(collection, elementValue, new object[] { index });
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
            DrawScirptField(type);

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
            DrawScirptField(type);
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
            EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type),
                () => { DrawRunTimeMode2(entity, component, type, pool); });

            if (remove)
                Remove(entity, index);
        }

        private static void DrawRunTimeMode2(Entity entity, object component, Type type, IPool pool)
        {
            if (remove) return;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(component == null ? $"ERROR! {type.Name} IS NULL" : $"{type.Name}", EditorStyles.boldLabel);

            RemoveBtn();
            EditorGUILayout.EndHorizontal();

            DrawScirptField(type);

            for (var i = 0; i < fields.Length; i++)
                DrawTypeFieldRunTime(component, fields[i]);

            pool.Set(component, entity.id);
        }


        private static void DrawScirptField(Type type)
        {
            if (csFileExist.ContainsKey(type))
            {
                var textAsset = scriptsAssets[type];
                GUI.enabled = false;
                EditorGUILayout.ObjectField("source", textAsset, typeof(TextAsset), false);
                GUI.enabled = true;
            }
        }
        private static void AddScirptAsset(Type type)
        {
            TextAsset textAsset = null;
            var guids = AssetDatabase.FindAssets(type.Name);
            List<TextAsset> textAssetsList = new List<TextAsset>();
            for (var i = 0; i < guids.Length; i++)
            {
                textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guids[i]));
                if (textAsset != null)
                    textAssetsList.Add(textAsset);
            }

            var temp = textAssetsList.FirstOrDefault(x => x.name == type.Name);
            if (temp != null)
            {
                csFileExist.Add(type, true);
                scriptsAssets.Add(type, temp);
            }
        }
    }
}