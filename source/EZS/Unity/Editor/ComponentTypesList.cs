    using System.Collections.Generic;
using System;
    using System.Linq;
    using System.Reflection;
using UnityEditor;
    using UnityEngine;
    using Object = System.Object;

    namespace Wargon.ezs.Unity {
    public static class ComponentTypesList
    {
        private static Dictionary<Type, Color> Colors = new Dictionary<Type, Color>();
        private static List<string> Types = new List<string>();
        private static string[] TypesArray;
        private static Type[] typesValue;
        
        public static int Count => Types.Count;
        public static string Get(int index) => Types[index];
        public static List<string> GetAll() => Types;
        public static string[] GetAllInArray() => TypesArray;
        public static Color GetColorStyle(Type type) => Colors[type];

        public static Type[] GetTypes()
        {
            if(typesValue.Length < 1)
                Init();
            return typesValue;
        }
        public static bool IsNUll => !inited;
        private static bool inited;

        static ComponentTypesList()
        {
            Init();
        }
        public static void Init()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var listOfComponents = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var componentsFromAssebly = GetTypesWithAttribute(typeof(EcsComponentAttribute), assembly).ToArray();
                Array.Sort(componentsFromAssebly, (x,y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));
                foreach (var type in componentsFromAssebly)
                {
                    listOfComponents.Add(type);
                    Add($"{type}");
                }
            }

            typesValue = listOfComponents.ToArray();
            listOfComponents.Clear();
            Types.Sort();
            TypesArray = Types.ToArray();
            SetColorStyles(typesValue);
            inited = true;
        }

        private static void SetColorStyles(Type[] types)
        {
            if(Colors.Count > 1)
                Colors.Clear();
            for (int i = 0, iMax = types.Length; i < iMax; i++)
            {
                var h = (float) i / Count;
                var componentColor = Color.HSVToRGB(h, 0.7f, 0.8f);
                componentColor.a = 0.15f;
                Colors.Add(types[i], componentColor);
            }
        }

        private static void Add(string name) {
            if (!Types.Contains(name))
                Types.Add(name);
        }
        
        private static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                    yield return type;
            }
        }
    }

}
