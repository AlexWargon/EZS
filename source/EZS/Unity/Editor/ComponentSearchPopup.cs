using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class ComponentSearchPopup : PopupWindowContent
    {
        private const string COMPONENT_ICON_PATH = "Assets/EZS/Unity/Images/EcsComponentIcon.png";
        private readonly SearchField addComponentsField;
        private readonly GUIStyle buttonStyle;
        private string[] componentList;
        private readonly int entitiesCount;
        private readonly Texture2D icon;
        private readonly GUIStyle labelStyle;
        private readonly MonoEntity[] manyEntities;
        private readonly Action<string, MonoEntity> OnAddComponent;
        private Vector2 scrollPos;
        private string searchComponent;

        public ComponentSearchPopup(Action<string, MonoEntity> action, MonoEntity[] entities, int count)
        {
            addComponentsField = new SearchField();
            addComponentsField.SetFocus();
            searchComponent = string.Empty;
            componentList = new string[] { };
            buttonStyle = GUI.skin.customStyles[455];
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.fontStyle = FontStyle.Normal;
            buttonStyle.onHover.background = EntityGUI.NewTexture(2, 2, new Color(0f, 1f, 0.03f));
            buttonStyle.onHover.textColor = Color.black;
            buttonStyle.fontSize = 13;
            buttonStyle.hover.background = EntityGUI.NewTexture(2, 2, new Color(0f, 1f, 0.03f));
            icon = EditorGUIUtility.Load(COMPONENT_ICON_PATH) as Texture2D;

            labelStyle = GUI.skin.customStyles[468];
            labelStyle.fontStyle = FontStyle.Bold;
            OnAddComponent = action;
            manyEntities = entities;
            entitiesCount = count;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 350);
        }

        public override void OnGUI(Rect rect)
        {
            searchComponent = addComponentsField.OnToolbarGUI(EditorGUILayout.GetControlRect(), searchComponent);
            componentList = ComponentTypesList.GetAllInArray()
                .Where(x => x.Contains(searchComponent, StringComparison.OrdinalIgnoreCase)).ToArray();

            EditorGUILayout.LabelField("Components", labelStyle);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(250), GUILayout.Height(306));

            for (var i = 0; i < componentList.Length; i++) DrawComponentBtn(componentList[i]);
            EditorGUILayout.EndScrollView();
        }

        private void DrawComponentBtn(string name)
        {
            var content = new GUIContent(name, icon);

            if (GUILayout.Button(content, buttonStyle))
            {
                for (var i = 0; i < entitiesCount; i++)
                {
                    var entity = manyEntities[i];
                    OnAddComponent?.Invoke(name, entity!);
                }

                editorWindow.Close();
            }
        }
    }
}