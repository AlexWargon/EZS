using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public static class EntityGUI
    {
        private static Dictionary<Type, GUIStyle> colorBoxesByType;
        private static bool colored = true;
        private static readonly Color Default = new Color(0.6f, 0.6f, 0.6f, 0.18f);
        private static readonly GUIStyle DEFAULT_STYLE = GUI.skin.box;
        [MenuItem("EZS/Colored On|Off")]
        private static void SetColored()
        {
            colored = !colored;
        }

        static EntityGUI() {
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            Init();
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj) {
            inited = false;
        }

        private static bool inited;

        public static void Init()
        {
            if(inited) return;
            
            colorBoxesByType = new Dictionary<Type, GUIStyle>();
            ComponentTypesList.Init();
            InitColorStyles();
            //Debug.Log("ENTITY GUI RELOAD");
            inited = true;
        }
        private static void InitColorStyles()
        {
            var types = ComponentTypesList.GetTypes();
            for (int i = 0, iMax = ComponentTypesList.Count; i < iMax; i++)
            {
                var newType = types[i];
                var componentColor = ComponentTypesList.GetColorStyle(newType);
                var style = new GUIStyle(GUI.skin.box) {normal = {background = NewTexture(2, 2, componentColor)}};
                colorBoxesByType.Add(types[i], style);
            }
        }

        public static GUIStyle GetColorStyleByType(Type type) {
            if (!colorBoxesByType.ContainsKey(type))
                return DEFAULT_STYLE;
            return colorBoxesByType[type];
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