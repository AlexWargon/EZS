using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Wargon.ezs.Unity
{
    public static class ComponentInspectorInternal
    {
        private static bool remove;
        private static int Count;
        private static Dictionary<Type, ITypeInspector> inspectors;
        private static Dictionary<Type, ListInspector> listInspecors;
        private static Dictionary<Type, TextAsset> scriptsAssets;
        private static Dictionary<Type, IComponentInspector> componentInspectors;
        private static Dictionary<Type, bool> csFileExist;
        private static bool inited;
        public static void Init()
        {
            if(inited) return;
            inspectors = new Dictionary<Type, ITypeInspector>();
            listInspecors = new Dictionary<Type, ListInspector>();
            scriptsAssets = new Dictionary<Type, TextAsset>();
            componentInspectors = new Dictionary<Type, IComponentInspector>();
            csFileExist = new Dictionary<Type, bool>();
            CreateInspectors();
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
        private static void CreateInspectors()
        {
            var assembly = Assembly.GetAssembly(typeof(ITypeInspector));
            
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(ITypeInspector).IsAssignableFrom(type) && type != typeof(TypeInspector<>) && !type.IsInterface)
                {
                    var inspector = (ITypeInspector)Activator.CreateInstance(type);
                    var fieldType = inspector.GetTypeOfField();
                    if (fieldType == typeof(Enum) && type == typeof(EnumFlagInspector) ) {
                        inspectors.Add(typeof(FlagsAttribute), inspector);
                    }
                    else {
                        inspectors.Add(fieldType, inspector);
                    }
                    
                }
                else
                if (typeof(IComponentInspector).IsAssignableFrom(type) && type != typeof(ComponentInspector<>) && !type.IsInterface)
                {
                    var inspector = (IComponentInspector)Activator.CreateInstance(type);
                    Log.Show(Color.green, inspector.ComponentType.Name);
                    inspector.OnCreate();
                    componentInspectors.Add(inspector.ComponentType, inspector);
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
        
        private static ITypeInspector GetInspector(Type key)
        {
            if (!inspectors.ContainsKey(key))
            {
                if (key == typeof(UnityObject) || key.IsSubclassOf(typeof(UnityObject)))
                {
                    var inspector = UnityObjectInspector.New(key) as ITypeInspector;
                    if (inspector != null)
                    {
                        inspectors.Add(key, inspector);
                    }
                    return inspector;
                }
            }

            if (!inspectors.ContainsKey(key))
                return null;
            return inspectors[key];
        }

        private static ListInspector GetListInspector(Type key)
        {
            if (!listInspecors.ContainsKey(key))
            {
                var inspector = new ListInspector();
                inspector.Init(key);
                listInspecors.Add(key, inspector);
            }
            return listInspecors[key];
        }

        private static HashSet<int> maskCache;
        public static MonoEntity currentEntity;
        public static void DrawComponentRun(MonoEntity entity, UnityObject obj, object component) {
            currentEntity = entity;
            Type type = component.GetType();
            int componentTypeID = ComponentType.GetID(type);
            IPool pool = entity.Entity.World.GetPoolByID(componentTypeID);
            EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type), () => { DrawRunTimeMode(entity, component, pool); });
            if (remove) {
                entity.Entity.RemoveByTypeID(componentTypeID);
                remove = false;
            }
        }

        public static void DrawComponentEditor(MonoEntity entity, UnityObject obj, object component, int index) {
            currentEntity = entity;
            Type type = component.GetType();
            EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type), () => { DrawEditorMode(entity, index); });
            if (remove) {
                if (entity.Components.HasType(type))
                    entity.Components.RemoveAt(index);
                EditorUtility.SetDirty(obj);
                remove = false;
            }
        }
        public static void DrawComponentBox(MonoEntity entity, int index, MonoEntity[] manyEntities, int count, UnityObject target)
        {
            Count = count;
            if (entity.ComponentsCount < index) return;
            object component;
            int componentTypeID = 0;
            Type type;
            if (entity.runTime)
            {
                unsafe {
                    ref var data = ref entity.Entity.GetEntityData();
                    maskCache = data.archetype.Mask;
                    componentTypeID = maskCache.ElementAt(index);
                }
                var pool = entity.Entity.World.GetPoolByID(componentTypeID);
                component = pool.GetBoxed(entity.Entity.id);
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
                Remove(entity, index, manyEntities, target, componentTypeID);
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
            EntityGUI.Horizontal(() => DrawField(fieldValue, field.Name,  field.FieldType, component, field));
        }

        private static void DrawTypeFieldRunTime(object component, FieldInfo field)
        {
            if (component == null) return;
            var fieldValue = field.GetValue(component);
            EntityGUI.Horizontal(() => DrawField(fieldValue, field.Name, field.FieldType, component, field));
        }

        private static void DrawField(object fieldValue, string fieldName, Type fieldType, object component, FieldInfo fieldInfo)
        {
            var isList = typeof(IList).IsAssignableFrom(fieldType);
            var inspector = isList ? GetListInspector(fieldType.GetElementsType()) : 
                fieldType.IsEnum ? (fieldType.GetCustomAttribute<FlagsAttribute>()) != null ? GetInspector(typeof(FlagsAttribute)) : GetInspector(typeof(Enum)) : GetInspector(fieldType);
            if (inspector != null)
            {
                if (isList)
                {
                    if (inspector is ListInspector listInspector)
                    {
                        if(!inspectors.ContainsKey(listInspector.FieldType)) return;
                        listInspector.SetTypeOfField(fieldType);
                        listInspector.SetTarget((IList)fieldValue);
                        listInspector.Init(fieldType.GetElementsType());
                        fieldValue = listInspector.Draw(fieldName, fieldValue);
                    }
                }
                else {
                    fieldValue = inspector.Draw(fieldName, fieldValue);
                }
            }
            component.GetType().GetField(fieldName).SetValue(component, fieldValue);
        }

        public static void SetCollectionElement(Rect rect, object elementValue , int index, string fieldName, Type elementType, object collection)
        {
            var isList = typeof(IList).IsAssignableFrom(elementType);
            var inspector = isList ? GetListInspector(elementType.GetElementsType()) : GetInspector(elementType);
            if (inspector != null)
            {
                if (isList)
                {
                    if (inspector is ListInspector listInspector)
                    {
                        listInspector.SetTarget((IList)elementValue);
                        listInspector.Init(elementType.GetElementsType());
                        elementValue = listInspector.DrawCollectionElement(new Rect(rect.x + 10, rect.y, 250, EditorGUIUtility.singleLineHeight), elementValue, index);
                    }
                }
                else
                {
                    elementValue = inspector.DrawCollectionElement(new Rect(rect.x + 10, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight), elementValue, index);
                }
            }
            collection.GetType().GetProperty("Item")?.SetValue(collection, elementValue, new object[] { index });
        }

        private static void Remove(MonoEntity entity, int index, IReadOnlyList<MonoEntity> manyEntities, UnityObject target, int componentTypeID)
        {
            var type = entity.Components[index].GetType();
            if (Count > 1)
            {
                for (var i = 0; i < Count; i++)
                {
                    var e = manyEntities[i];
                    if (entity.runTime)
                    {
                        e.Entity.RemoveByTypeID(componentTypeID);
                        remove = false;
                    }
                    else
                    {
                        if (e.Components.HasType(type))
                            e.Components.RemoveAt(index);
                        EditorUtility.SetDirty(target);
                        remove = false;
                    }
                }
            }
            else
            {
                if (entity.runTime)
                {
                    entity.Entity.RemoveByTypeID(componentTypeID);
                    remove = false;
                }
                else
                {
                    entity.Components.RemoveAt(index);
                    remove = false;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        private static void Remove(Entity entity, int index, int componentTypeID)
        {
            entity.RemoveByTypeID(componentTypeID);
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
            if (componentInspectors.ContainsKey(type))
            {
                componentInspectors[type].Draw(component);
                return;
            }
            
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
            
            pool.SetBoxed(component, entity.Entity.id);
        }


        public static void DrawComponentBox(Entity entity, int index, int componentTypeID)
        {
            if (entity.GetEntityData().ComponentsCount<= index) return;
            var pool = entity.World.GetPoolByID(componentTypeID);
            var component = pool.GetBoxed(entity.id);
            var type = component.GetType();
            EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type),
                () => { DrawRunTimeMode2(entity, component, type, pool); });

            if (remove)
                Remove(entity, index, componentTypeID);
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

            pool.SetBoxed(component, entity.id);
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

    public static class TypeExtensions
    {
        public static Type GetElementsType(this Type target)
        {
            return !target.IsArray ? target.GetGenericArguments()[0] : target.GetElementType();
        }
    }
    public static class Log
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Show(string massage)
        {
            Debug.Log(massage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Show(Color color, string massage)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#{(byte) (color.r * 255f):X2}{(byte) (color.g * 255f):X2}{(byte) (color.b * 255f):X2}>{massage}</color>");
#endif
        }
    }
}