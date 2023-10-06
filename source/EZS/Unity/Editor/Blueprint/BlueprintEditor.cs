using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity {
    [CustomEditor(typeof(EntityBlueprint))]
    public class BlueprintEditor : Editor
    {
        private GUIContent addComponentText;
        private GUIStyle addComponentButtonStyle;
        private Rect addButtonRect;
        private int entitiesCount;
        private bool flowed = true;
        public override void OnInspectorGUI() {
            EntityGUI.Init();

            var blueprint = target as EntityBlueprint;
            
            
            EditorGUI.BeginChangeCheck();
            addComponentText = new GUIContent("Add Component");
            addComponentButtonStyle = GUI.skin.button;

            // EntityGUI.Vertical(GUI.skin.box, () =>
            // {
            //     EntityGUI.Horizontal(() =>
            //     {
            //         flowed = EditorGUILayout.Foldout(flowed, $"ECS Components [{monoEntity.ComponentsCount.ToString()}]");
            //         
            //         if (GUILayout.Button(new GUIContent("Clear", "Remove All Components")))
            //             RemoveAll();
            //     });
            //     
            //     if (!flowed) return;
            //     addButtonRect = GUILayoutUtility.GetRect(addComponentText, addComponentButtonStyle);
            //     if (GUI.Button(addButtonRect, addComponentText, addComponentButtonStyle))
            //     {
            //         addButtonRect.y -= 20f;
            //         PopupWindow.Show(addButtonRect, new ComponentSearchPopup(AddComponent, manyEntities, entitiesCount));
            //     }
            //     DrawComponents();
            // });
            
            
        }
    }
}

