using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

namespace Wargon.ezs.Unity
{
    [CustomEditor(typeof(WorldDebug))]
    public class WorldDebugInspector : Editor
    {
        private List<EntityDrawer> EntityDrawers = new List<EntityDrawer>();
        private SearchField filterComponentsField;
        string filterComponentString;
        private bool showEntities;
        private bool showArchetypes;
        private bool showQueries;
        private Vector2 scrollPos;
        private void OnEnable()
        {
            filterComponentsField = new SearchField();
            filterComponentsField.SetFocus();
            filterComponentString = string.Empty;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            var debug = target as WorldDebug;
            var world = debug.world;


            //var sss = Profiler.GetMonoUsedSizeLong();
            var floatMemory = Profiler.GetMonoUsedSizeLong() / 1048576f;
            var mb = floatMemory.ToStringNonAlloc("0.00");
            
            EditorGUILayout.LabelField($"RAM ALLOCATED : {mb} MB");
            EditorGUILayout.LabelField($"Total Entity Count : {world.GetTotalEntitiesCount().ToString()}");
            EditorGUILayout.LabelField($"Alive Entity Count : {world.GetAliveEntntiesCount().ToString()}");
            EditorGUILayout.LabelField($"Free Entity Count : {world.GetFreeEntitiesCount().ToString()}");
            EditorGUILayout.LabelField($"Archetypes Count : {world.ArchetypesCount().ToString()}");
            EditorGUILayout.LabelField($"Systems Count : {world.GetAllSystems()[0].updateSystemsList.Count.ToString()}");
            EditorGUILayout.Space();
            //DrawPools(world);
            EditorGUILayout.Space();
            filterComponentString = filterComponentsField.OnGUI(EditorGUILayout.GetControlRect(), filterComponentString);
            showEntities = EditorGUILayout.Foldout(showEntities, "Entities");
            showArchetypes = EditorGUILayout.Foldout(showArchetypes, "Archetypes");
            showQueries = EditorGUILayout.Foldout(showQueries, "Queries");
            if (showEntities) {
                var entities = world.entities;
                EditorGUILayout.BeginVertical();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                var eCount = Screen.height / 20;
                if (EntityDrawers.Count < eCount)
                {
                    while (EntityDrawers.Count < world.totalEntitiesCount)
                        EntityDrawers.Add(new EntityDrawer(world));
                }
                for (var i = 1; i < eCount; i++)
                {
                    EntityDrawers[i].Draw(entities[i],filterComponentString);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            
            if (showArchetypes) {
                var archetypes = world.GetAllArchetypes();
                for (var i = 0; i < archetypes.Count; i++) {
                    ref var archetype = ref archetypes.Items[i];
                    EditorGUILayout.LabelField($"Archetype ID : {archetype.ID}");
                    // unsafe {
                    //     for (int j = 0; j < archetype.QueriesCount; j++) {
                    //         var q = archetype.Queries.ElementAt(i);
                    //         EditorGUILayout.LabelField(q->ToString());
                    //     }
                    // }
                    
                    // foreach (var i1 in archetype.Mask) {
                    //     var type = ComponentType.GetTypeValue(i1);
                    //     EntityGUI.Vertical(EntityGUI.GetColorStyleByType(type), () => {
                    //         EditorGUILayout.LabelField($"{type.Name}");
                    //     });
                    // }
                }
            }

            if (showQueries) {
                var qs = world.GetAllQueries();

                for (int i = 0; i < qs.Count; i++) {
                    EditorGUILayout.LabelField(qs.Items[i].ToString());
                    EditorGUILayout.LabelField($"Count:{qs.Items[i].Count}");
                }
            }

        }

        private void DrawPools(World world)
        {
            var pools = world.GetAllPoolsInternal();
            for (var i = 0; i < pools.Length; i++)
            {
                if (pools[i] != null)
                {
                    EditorGUILayout.LabelField("_________________________________________________________");
                    EditorGUILayout.LabelField($" Pool<{ComponentType.GetTypeValue(pools[i].TypeID).Name}>.Size  : {pools[i].GetSize()}");
                    EditorGUILayout.LabelField($" Pool<{ComponentType.GetTypeValue(pools[i].TypeID).Name}>.Count : {pools[i].Count}");
                }
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
            _stringBuilder = new StringBuilder();
        }

        private readonly StringBuilder _stringBuilder;
        private void EntityToString(Entity entity)
        {
            _stringBuilder.Append($" ID:{entity.id}");
            ref var data = ref entity.GetEntityData();
            foreach (var dataComponentType in data.archetype.Mask)
            {
                var pool = world.GetPoolByID(dataComponentType);
                var component = pool.GetBoxed(entity.id);
                if (component != null) {
                    name += $"; {component.GetType()}";
                    _stringBuilder.Append($"; {component.GetType()}");
                }
            }
            name = _stringBuilder.ToString();
            _stringBuilder.Clear();
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
            if(!Has(filter)) return;
            if (name == String.Empty)
                name = $"e:{entity.id}";
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
            var componentsCount = data.ComponentsCount;
            GUILayout.BeginVertical(GUI.skin.box);
            
            GUILayout.Label($"Entity ID :{entity.id}");
            //EditorGUILayout.LabelField($"Entity ID : {entity.id.ToString()}");
            EditorGUILayout.LabelField($"ECS Components : [{componentsCount}]", EditorStyles.boldLabel);
            var index = 0;
            foreach (var componentTypeID in data.archetype.Mask) {
                ComponentInspectorInternal.DrawComponentBox(entity, index, componentTypeID);
                index++;
            }
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
