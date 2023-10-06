using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;
using Wargon.ezs;
using Wargon.ezs.Unity;
using Object = UnityEngine.Object;

public class ListDrawer
{
    private ReorderableList rList;
    private static Rect listRect = new Rect(Vector2.zero, Vector2.one * 500f);
    public ListDrawer(IList list, Type type)
    {
        rList = new ReorderableList(list, type);
        if (list.Count < 1)
            list.Add(null);
        // rList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, type.Name);
        // rList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        // {
        //     rect.y += 2f;
        //     rect.height = EditorGUIUtility.singleLineHeight;
        //     //EditorGUI.PropertyField(rect, rList.list., objectLabel);
        //
        //     EditorGUILayout.ObjectField($"    sss", rList.list[index] as Object, type, false);
        //};
        rList.drawHeaderCallback = DrawHeader;
        rList.drawElementCallback = DrawListItems;
        //rList.onAddCallback = reorderableList => reorderableList.list.Add(null);
    }

    public IList Draw(Vector2 pos)
    {
        listRect.position = pos;
        rList.DoList(listRect);
        return rList.list;
    }
    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {        
        var element = rList.list[index]; //The element in the list

        // Create a property field and label field for each property. 

        // The 'mobs' property. Since the enum is self-evident, I am not making a label field for it. 
        // The property field for mobs (width 100, height of a single line)
        //EditorGUILayout.ObjectField($"    sss", element as Object, element.GetType(), false);
        SetFieldValue(element, $"{element.GetType().Name} [{index}]");
    }
    void DrawHeader(Rect rect)
    {
        string name = "Wave";
        EditorGUI.LabelField(rect, name);
    }
    private static void SetFieldValue(object fieldValue, string fieldName)
    {
        switch (fieldValue)
        {
            case LayerMask field:
                LayerMask tempMask = EditorGUILayout.MaskField(fieldName,
                    InternalEditorUtility.LayerMaskToConcatenatedLayersMask(field),
                    InternalEditorUtility.layers);
                fieldValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
                break;
            case Enum field:
                fieldValue = EditorGUILayout.EnumFlagsField($"    {fieldName}", field);
                break;
            case int field:
                fieldValue = EditorGUILayout.IntField($"    {fieldName}", field);
                break;
            case float field:
                fieldValue = EditorGUILayout.FloatField($"    {fieldName}", field);
                break;
            case bool field:
                fieldValue = EditorGUILayout.Toggle($"    {fieldName}", field);
                break;
            case double field:
                fieldValue = EditorGUILayout.DoubleField($"    {fieldName}", field);
                break;
            case Vector2 field:
                fieldValue = EditorGUILayout.Vector2Field($"    {fieldName}", field);
                break;
            case Vector3 field:
                fieldValue = EditorGUILayout.Vector3Field($"    {fieldName}", field);
                break;
            case Vector4 field:
                fieldValue = EditorGUILayout.Vector4Field($"    {fieldName}", field);
                break;

            case AnimationCurve field:
                fieldValue = EditorGUILayout.CurveField($"    {fieldName}", field);
                break;

            case string field:
                fieldValue = EditorGUILayout.TextField($"    {fieldName}", field);
                break;
            case Entity field:
                EditorGUILayout.BeginVertical();
                if (field.IsNULL())
                {
                    EditorGUILayout.LabelField($"    {fieldName} : NULL");
                    EditorGUILayout.EndVertical();
                    break;
                }

                if (field.Has<View>())
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField($"    View", field.Get<View>().Value, typeof(MonoEntity), true);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUILayout.LabelField($"    {fieldName}");
                    EditorGUILayout.IntField("    Entity ID", field.id);
                    EditorGUILayout.IntField("    Entity GEN", field.InternalGetGeneration());
                }

                EditorGUILayout.EndVertical();
                break;

            case IList field:
                // EditorGUILayout.BeginVertical();
                // var elementType = field.GetType().GetElementType();
                //
                // int j = 0;
                // if (field.Count < 1)
                //     field.Add(null);
                //
                // foreach (var o in field)
                // {
                //     SetCollectionElement(o, $"{fieldName} [{j}]", elementType);
                //     j++;
                // }
                // EditorGUILayout.EndVertical();

                break;
        }

    }
}
