using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public static class EntityGUI
    {
        private static Dictionary<int, GUIStyle[]> colorBoxes;
        private static Dictionary<Type, GUIStyle> newColorBoxes;
        private static bool colored = true;
        private static readonly Color Default = new Color(0.6f, 0.6f, 0.6f, 0.18f);

        [MenuItem("Entity/Colored On|Off")]
        private static void SetColored()
        {
            colored = !colored;
        }

        public static void Init()
        {
            colorBoxes = new Dictionary<int, GUIStyle[]>();
            newColorBoxes = new Dictionary<Type, GUIStyle>();
        }

        public static GUIStyle GetColorStyle(int componentsCount, int index)
        {
            if (colorBoxes == null) colorBoxes = new Dictionary<int, GUIStyle[]>();
            GUIStyle[] styles;
            if (colorBoxes.TryGetValue(componentsCount, out styles)) return styles[index];

            styles = new GUIStyle[componentsCount];

            SetComponentColor(componentsCount, styles);

            colorBoxes.Add(componentsCount, styles);
            return styles[index];
        }

        public static GUIStyle GetColorStyle(Type type)
        {
            if (newColorBoxes == null) newColorBoxes = new Dictionary<Type, GUIStyle>();

            if (ComponentTypesList.Count != newColorBoxes.Count || !newColorBoxes.ContainsKey(type))
            {
                var types = ComponentTypesList.GetTypeValues();
                newColorBoxes.Clear();
                for (int i = 0, iMax = ComponentTypesList.Count; i < iMax; i++)
                {
                    var newType = types[i];
                    var componentColor = ComponentTypesList.GetColorStyle(newType);
                    componentColor.a = 0.15f;
                    var style = new GUIStyle(GUI.skin.box);
                    style.normal.background = NewTexture(2, 2, componentColor);
                    newColorBoxes.Add(types[i], style);
                }
            }
            return newColorBoxes[type];
        }
        private static void SetComponentColor(int componentsCount, GUIStyle[] styles)
        {
            if (colored)
            {
                for (int i = 0, iMax = styles.Length; i < iMax; i++)
                {
                    var h = (float) i / componentsCount;
                    var componentColor = Color.HSVToRGB(h, 0.7f, 1f);
                    componentColor.a = 0.15f;
                    var style = new GUIStyle(GUI.skin.box);
                    style.normal.background = NewTexture(2, 2, componentColor);
                    styles[i] = style;
                }
            }
            else
            {
                for (int i = 0, iMax = styles.Length; i < iMax; i++)
                {
                    var style = new GUIStyle(GUI.skin.box);
                    style.normal.background = NewTexture(2, 2, Default);
                    styles[i] = style;
                }
            }
        }

        public static Texture2D NewTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (var i = 0; i < pixels.Length; ++i) pixels[i] = color;
            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        public static void Horizontal(GUIStyle style, Action body = null, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);
            body?.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void Horizontal(Action body = null, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            body?.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void Vertical(GUIStyle style, Action body = null, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(style, options);
            body?.Invoke();
            GUILayout.EndVertical();
        }

        public static void Vertical(Action body = null, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            body?.Invoke();
            GUILayout.EndVertical();
        }

        private static Styles styles;

        public static Styles Styles
        {
            get
            {
                if (styles == null)
                    styles = new Styles();
                return styles;
            }
        }
        
    }
    public class Styles
    {
        private GUIStyle font1;
        public GUIStyle Impact
        {
            get
            {
                if (font1 == null)
                {
                    font1 = new GUIStyle();
                    font1.font = Font.CreateDynamicFontFromOSFont("Impact",14);
                }

                return font1;
            }
        }
    }
}