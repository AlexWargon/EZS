using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;
using Wargon.ezs.Unity;

namespace Wargon.ezs
{
    [CustomEditor(typeof(WorldDebug))]
    public class WorldDebugInspector : Editor
    {
        private List<EntityDrawer> EntityDrawers = new List<EntityDrawer>();
        private SearchField filterComponentsField;
        string filterComponentString;
        private bool showEntities;
        private void OnEnable()
        {
            filterComponentsField = new SearchField();
            filterComponentsField.SetFocus();
            filterComponentString = string.Empty;
            ComponentTypesList.Init();
            EntityGUI.Init();
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            var debug = target as WorldDebug;
            var world = debug.world;

            var floatMemory = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
            var mb = floatMemory.ToStringNonAlloc("0.00");
            
            EditorGUILayout.LabelField($"RAM ALLOCATED : {mb} MB");
            EditorGUILayout.LabelField($"Entity Count : {world.GetEntitiesCount().ToString()}");
            EditorGUILayout.LabelField($"Free Entity Count : {world.GetFreeEntitiesCount().ToString()}");
            EditorGUILayout.LabelField($"Systems Count : {world.GetAllSystems()[0].updateSystemsList.Count.ToString()}");
            EditorGUILayout.Space();
            DrawPools(world);
            EditorGUILayout.Space();

            showEntities = EditorGUILayout.Foldout(showEntities, "Entities");
            if (showEntities)
            {
                filterComponentString = filterComponentsField.OnGUI(EditorGUILayout.GetControlRect(), filterComponentString);

                var entities = world.entities;

                if (EntityDrawers.Count < world.entitiesCount)
                {
                    while (EntityDrawers.Count < world.entitiesCount)
                        EntityDrawers.Add(new EntityDrawer(world));
                }
                for (var i = 0; i < world.entitiesCount; i++)
                {
                    EntityDrawers[i].Draw(entities[i],filterComponentString);
                }
            }
            // for (var i = 0; i < GUI.skin.customStyles.Length; i++)
            // {
            //     if (GUILayout.Toggle(sss,$"{i}", GUI.skin.customStyles[i]))
            //     {}
            // }
        }

        private void DrawPools(World world)
        {
            var pools = world.ComponentPools;
            for (var i = 0; i < pools.Length; i++)
            {
                if(pools[i]!=null)
                    EditorGUILayout.LabelField($" Pool<{pools[i].ItemType}> Size : {pools[i].GetSize()}");
            }
        }
    }

    public class EntityDrawer
    {
        private bool toggled;
        private readonly World world;
        private readonly GUIStyle buttonStyle;

        private string name;
        public EntityDrawer(World world)
        {
            this.world = world;
            //buttonStyle = GUI.skin.customStyles[223];
            buttonStyle = GUI.skin.customStyles[528];
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 12;
            buttonStyle.fixedHeight = 20f;

            name = string.Empty;
        }
        private void EntityToString(Entity entity)
        {
            name = $" ID:{entity.id}";
            ref var data = ref entity.GetEntityData();
            foreach (var dataComponentType in data.componentTypes)
            {
                var pool = world.GetPoolByID(dataComponentType);
                var component = pool.Get(entity.id);
                if(component!= null)
                    name += $"; {component.GetType()}";
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Has(string filter)
        {
            if (filter == null) return true;
            return filter == string.Empty || name.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void Draw(Entity entity, string filter)
        {
            if(!Has(filter)) return;
            if (name == String.Empty)
                EntityToString(entity);
            if (GUILayout.Button(name, buttonStyle))
            {
                toggled = !toggled;
            }
            if(toggled)
                WorldEntityDrawer.Draw(entity);
        }
    }
    
    public static class WorldEntityDrawer
    {
        public static void Draw(Entity entity)
        {
            ref var data = ref entity.GetEntityData();
            var componentsCount = data.componentsCount;
            GUILayout.BeginVertical(GUI.skin.box);
            
            //EditorGUILayout.LabelField($"Entity ID : {entity.id.ToString()}");
            EditorGUILayout.LabelField($"ECS Components : [{componentsCount}]", EditorStyles.boldLabel);
            for(var index = 0; index < componentsCount; index++)
                ComponentInspector.DrawComponentBox(entity, index);
            GUILayout.EndVertical();
        }
    }

    public static class FloatToString
    {

        private const   string      floatFormat         = "0.0";
        private static  float       decimalMultiplier   = 1f;
        private static  string[]    negativeBuffer      = new string[0];
        private static  string[]    positiveBuffer      = new string[0];
        public static bool Inited
        {
            get
            {
                return negativeBuffer.Length > 0 || positiveBuffer.Length > 0;
            }
        }

        public static float MinValue
        {
            get
            {
                return -(negativeBuffer.Length - 1).FromIndex();
            }
        }

        public static float MaxValue
        {
            get
            {
                return (positiveBuffer.Length - 1).FromIndex();
            }
        }

        public static void Init(float minNegativeValue, float maxPositiveValue, int decimals = 1)
        {
            decimalMultiplier = Pow(10, Mathf.Clamp(decimals, 1, 5));

            int negativeLength = minNegativeValue.ToIndex();
            int positiveLength = maxPositiveValue.ToIndex();

            if (negativeLength >= 0)
            {
                negativeBuffer = new string[negativeLength];
                for (int i = 0; i < negativeLength; i++)
                {
                    negativeBuffer[i] = (-i).FromIndex().ToString(floatFormat);
                }
            }

            if (positiveLength >= 0)
            {
                positiveBuffer = new string[positiveLength];
                for (int i = 0; i < positiveLength; i++)
                {
                    positiveBuffer[i] = i.FromIndex().ToString(floatFormat);
                }
            }
        }

        public static string ToStringNonAlloc(this float value)
        {
            int valIndex = value.ToIndex();

            if (value < 0 && valIndex < negativeBuffer.Length)
            {
                return negativeBuffer[valIndex];
            }

            if (value >= 0 && valIndex < positiveBuffer.Length)
            {
                return positiveBuffer[valIndex];
            }

            return value.ToString();
        }

        public static string ToStringNonAlloc(this float value, string format)
        {
            int valIndex = value.ToIndex();

            if (value < 0 && valIndex < negativeBuffer.Length)
            {
                return negativeBuffer[valIndex];
            }

            if (value >= 0 && valIndex < positiveBuffer.Length)
            {
                return positiveBuffer[valIndex];
            }

            return value.ToString(format);
        }

        public static int ToInt(this float f)
        {
            return (int)f;
        }

        public static float ToFloat(this int i)
        {
            return (float)i;
        }
        
        private static int Pow(int f, int p)
        {
            for (int i = 1; i < p; i++)
            {
                f *= f;
            }
            return f;
        }

        private static int ToIndex(this float f)
        {
            return Mathf.Abs((f * decimalMultiplier).ToInt());
        }

        private static float FromIndex(this int i)
        {
            return (i.ToFloat() / decimalMultiplier);
        }
    }
}
