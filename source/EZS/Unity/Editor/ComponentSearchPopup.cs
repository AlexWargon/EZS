using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class ComponentSearchPopup : PopupWindowContent
    {
        private SearchField addComponentsField;
        private string[] componentList;
        private string searchComponent;
        private Action<string> OnAddComponent;
        private Vector2 scrollPos;
        private  GUIStyle buttonStyle;
        private Texture2D icon;
        private GUIStyle labelStyle;
        public ComponentSearchPopup(Action<string> action)
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
            icon = EditorGUIUtility.Load("Assets/EZS/Unity/Images/EcsComponentIcon.png") as Texture2D;

            labelStyle = GUI.skin.customStyles[468];
            labelStyle.fontStyle = FontStyle.Bold;
            OnAddComponent = action;
        }
        
        public override Vector2 GetWindowSize()
        {
            return new Vector2(234,350);
        }

        public override void OnGUI(Rect rect)
        {
            if(ComponentTypesList.Count < 1)
                ComponentTypesList.Init();
            
            searchComponent = addComponentsField.OnToolbarGUI(EditorGUILayout.GetControlRect(), searchComponent);
            componentList = ComponentTypesList.GetAllInArray().Where(x => x.Contains(searchComponent, StringComparison.OrdinalIgnoreCase)).ToArray();
            
            EditorGUILayout.LabelField("Components", labelStyle);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(230), GUILayout.Height(306));
            
            for (var i = 0; i < componentList.Length; i++)
            {
                DrawComponentBtn(componentList[i]);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawComponentBtn(string name)
        {
            var content = new GUIContent(name, icon);

            if (GUILayout.Button(content,buttonStyle))
            {
                OnAddComponent?.Invoke(name);
                editorWindow.Close();
            }
        }
    }
}