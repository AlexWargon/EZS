using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Wargon.ezs.Unity {
    public static class ComponentTypesList {
        private static Dictionary<Type, Color> Colors;
        private static readonly List<string> Types = new();
        public static readonly HashSet<string> NamesHash = new();
        private static string[] TypesArray;
        private static Type[] typesValue;
        private static bool inited;

        public static int Count => Types.Count;
        public static bool IsNUll => !inited;

        public static string Get(int index) {
            return Types[index];
        }

        public static List<string> GetAll() {
            return Types;
        }

        public static string[] GetAllInArray() {
            return TypesArray;
        }

        public static Color GetColorStyle(Type type) {
            return Colors[type];
        }

        public static Type[] GetTypes() {
            return typesValue;
        }

        public static void Init() {
            if (inited) return;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var listOfComponents = new List<Type>();
            foreach (var assembly in assemblies) {
                var componentsFromAssebly = GetTypesWithAttribute(typeof(EcsComponentAttribute), assembly).ToArray();
                Array.Sort(componentsFromAssebly, (x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
                foreach (var type in componentsFromAssebly) {
                    listOfComponents.Add(type);
                    Add($"{type}");
                }
            }

            typesValue = listOfComponents.ToArray();
            listOfComponents.Clear();
            Types.Sort();
            TypesArray = Types.ToArray();
            for (var i = 0; i < TypesArray.Length; i++) NamesHash.Add(TypesArray[i]);
            SetColorStyles(typesValue);
            inited = true;
        }

        private static void SetColorStyles(Type[] types) {
            if (Colors == null) Colors = new Dictionary<Type, Color>();
            if (Colors.Count > 1)
                Colors.Clear();
            for (int i = 0, iMax = types.Length; i < iMax; i++) {
                var h = (float)i / Count;
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
            foreach (var type in assembly.GetTypes())
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                    yield return type;
        }
    }
}