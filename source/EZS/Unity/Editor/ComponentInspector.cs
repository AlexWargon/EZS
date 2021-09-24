using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEditorInternal;
using System.Linq;
using System;
using System.Collections;


namespace Wargon.ezs.Unity {
    public static class ComponentInspector {

        private static bool remove;

        public static void DrawComponentBox(MonoEntity entity, int index)
        {
            if(entity.ComponentsCount < index) return;
            object component;
            Type type;
            if (entity.runTime)
            {
                var componentTypeID = entity.Entity.GetEntityData().componentTypes.ElementAt(index);
                var pool = entity.Entity.world.ComponentPools[componentTypeID];
                component = pool.Get(entity.Entity.id);
                type = pool.ItemType;
                EntityGUI.Vertical(EntityGUI.GetColorStyle(type), () =>
                {
                    DrawRunTimeMode(entity, component, pool);
                });
            }
            else
            {
                component = entity.Components[index];
                type = component.GetType();
                EntityGUI.Vertical(EntityGUI.GetColorStyle(type), () =>
                {
                    DrawEditorMode(entity, index);
                });
            }
            
            if (remove)
                Remove(entity, index);
        }

        private static void RemoveBtn()
        {
            if (GUILayout.Button(new GUIContent("✘", "Remove"), EditorStyles.miniButton, GUILayout.Width(21), GUILayout.Height(14)))
                remove = true;
        }

        private static void DrawTypeField(object component, FieldInfo field)
        {
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;

            if (fieldType == typeof(UnityEngine.Object) || fieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    fieldValue = EditorGUILayout.ObjectField($"    {field.Name}", fieldValue as UnityEngine.Object, fieldType, true);
                    component.GetType().GetField(field.Name).SetValue(component, fieldValue);
                });
                return;
            }

            EntityGUI.Horizontal(() => SetFieldValue(fieldValue, field.Name, component));
        }
        private static void DrawTypeFieldRunTime(object component, FieldInfo field)
        {
            if(component == null) return;
            var fieldValue = field.GetValue(component);
            var fieldType = field.FieldType;

            if (fieldType == typeof(UnityEngine.Object) || fieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    fieldValue = EditorGUILayout.ObjectField($"    {field.Name}", fieldValue as UnityEngine.Object, fieldType, true);
                    component.GetType().GetField(field.Name).SetValue(component, fieldValue);
                });
                return;
            }

            EntityGUI.Horizontal(() => SetFieldValue(fieldValue,field.Name, component));
        }
        private static void SetFieldValue(object fieldValue, string fieldName, object component)
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
                    fieldValue = EditorGUILayout.EnumPopup($"    {fieldName}", field);
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
                case Quaternion field:
                    var vec = QuaternionToVector4(field);
                    var tempVec = EditorGUILayout.Vector4Field($"    {fieldName}", vec);
                    fieldValue = Vector4ToQuaternion(tempVec);
                    break;
                case string field:
                    fieldValue = EditorGUILayout.TextField($"    {fieldName}", field);
                    break;
                case Entity field:
                    EditorGUILayout.BeginVertical();
                    if (field.IsDead())
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
                        EditorGUILayout.IntField("    Entity GEN", field.generation);
                    }

                    EditorGUILayout.EndVertical();
                    break;

                case IEnumerable field:
                    EditorGUILayout.BeginVertical();
                    int j = 0;
                    
                    foreach (var o in field)
                    {
                        SetCollectionElement(o, $"{fieldName} [{j}]", o.GetType());
                        j++;
                    }
                    if(j < 1)
                        EditorGUILayout.LabelField($"    Empty");
                    EditorGUILayout.EndVertical();
                    break;
            }

            // var fieldType = fieldValue.GetType();
            // if (fieldType.GetInterfaces().Contains(typeof(IList)))
            // {
            //
            //     int itemsCount = 0;
            //     float minHeight = EditorGUIUtility.singleLineHeight;
            //
            //     foreach (var item in (IEnumerable)fieldValue) { itemsCount++; }
            //
            //     GUILayout.BeginScrollView(EditorGUILayout.GetControlRect().position, GUILayout.Height(itemsCount * minHeight + minHeight));
            //     GUILayout.BeginVertical();
            //     foreach (var item in (IEnumerable)fieldValue)
            //     {
            //         EditorGUILayout.SelectableLabel(item.ToString(), (GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)));
            //         SetCollectionElement(item, item.ToString(), item.GetType());
            //
            //     }
            //     GUILayout.EndVertical();
            //     GUILayout.EndScrollView();
            //
            // }

            component.GetType().GetField(fieldName).SetValue(component, fieldValue);
        }
        private static void SetCollectionElement(object element, string fieldName, Type elementType)
        {
            if (elementType == typeof(UnityEngine.Object) || elementType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                EntityGUI.Horizontal(() =>
                {
                    element = EditorGUILayout.ObjectField($"    {fieldName}", element as UnityEngine.Object, elementType, true);
                });
                return;
            }
            switch (element)
            {
                case LayerMask field:
                    LayerMask tempMask = EditorGUILayout.MaskField(fieldName,
                        InternalEditorUtility.LayerMaskToConcatenatedLayersMask(field),
                        InternalEditorUtility.layers);
                    element = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
                    break;
                case Enum field:
                    element = EditorGUILayout.EnumFlagsField($"    {fieldName}", field);
                    break;
                case int field:
                    element = EditorGUILayout.IntField($"    {fieldName}", field);
                    break;
                case float field:
                    element = EditorGUILayout.FloatField($"    {fieldName}", field);
                    break;
                case bool field:
                    element = EditorGUILayout.Toggle($"    {fieldName}", field);
                    break;
                case double field:
                    element = EditorGUILayout.DoubleField($"    {fieldName}", field);
                    break;
                case Vector2 field:
                    element = EditorGUILayout.Vector2Field($"    {fieldName}", field);
                    break;
                case Vector3 field:
                    element = EditorGUILayout.Vector3Field($"    {fieldName}", field);
                    break;
                case Vector4 field:
                    element = EditorGUILayout.Vector4Field($"    {fieldName}", field);
                    break;
                case AnimationCurve field:
                    element = EditorGUILayout.CurveField($"    {fieldName}", field);
                    break;
                case Quaternion field:
                    var vec = QuaternionToVector4(field);
                    var tempVec = EditorGUILayout.Vector4Field($"    {fieldName}", vec);
                    element = Vector4ToQuaternion(tempVec);
                    break;
                case string field:
                    element = EditorGUILayout.TextField($"    {fieldName}", field);
                    break;
                case Entity field:
                    EditorGUILayout.BeginVertical();
                    if (field.IsDead())
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
                        EditorGUILayout.IntField("    Entity GEN", field.generation);
                    }

                    EditorGUILayout.EndVertical();
                    break;
                case IEnumerable field:
                    EditorGUILayout.BeginVertical();

                    int j = 0;

                    foreach (var o in field)
                    {
                        SetCollectionElement(o, $"{fieldName} [{j}]", o.GetType());
                        j++;
                    }
                    if(j < 1)
                        EditorGUILayout.LabelField($"    Empty");
                    EditorGUILayout.EndVertical();
                    break;
                // case IList field:
                //     EditorGUILayout.BeginVertical();
                //     var nelementType = field.GetType().GetElementType();
                //     int j = 0;
                //     if (field.Count < 1)
                //         field.Add(null);
                //     
                //     foreach (var o in field)
                //     {
                //         SetCollectionElement(o, $"{fieldName} [{j}]", nelementType);
                //         j++;
                //     }
                //     EditorGUILayout.EndVertical();
                //     break;
            }
            
        }
        private static Vector4 QuaternionToVector4(Quaternion rot)
        {
            return new Vector4(rot.x, rot.y, rot.z, rot.w);
        }

        private static Quaternion Vector4ToQuaternion(Vector4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
        private static void Remove(MonoEntity entity, int index)
        {
            if (entity.runTime)
            {
                entity.Entity.RemoveByTypeID(entity.Entity.GetEntityData().componentTypes.ElementAt(index));
                remove = false;
            }
            else
            {
                entity.Components.RemoveAt(index);
                remove = false;
            }
        }
        private static void Remove(Entity entity, int index)
        {
            entity.RemoveByTypeID(entity.GetEntityData().componentTypes.ElementAt(index));
            remove = false;
        }
        private static void DrawEditorMode(MonoEntity entity, int index)
        {
            var component = entity.Components[index];
            if (component == null)
            {
                entity.Components = entity.Components.Where(x => x != null).ToList();
                return;
            }

            var type = component.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            EntityGUI.Horizontal(() =>
            {
                EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);
                RemoveBtn();
            });

            foreach (var field in fields)
                DrawTypeField(component, field);
        }

        private static void DrawRunTimeMode(MonoEntity entity, object component, IPool pool)
        {
            if(remove) return;

            var type = pool.ItemType;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{type.Name}", EditorStyles.boldLabel);
            RemoveBtn();
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < fields.Length; i++)
                DrawTypeFieldRunTime(component, fields[i]);

            pool.Set(component,entity.Entity.id);
        }
        
        
        public static void DrawComponentBox(Entity entity, int index)
        {
            if(entity.GetEntityData().componentTypes.Count <= index) return;
            var componentTypeID = entity.GetEntityData().componentTypes.ElementAt(index);
            var pool = entity.world.ComponentPools[componentTypeID];
            var component = pool.Get(entity.id);
            var type = component.GetType();
            EntityGUI.Vertical(EntityGUI.GetColorStyle(type), () =>
            {
                DrawRunTimeMode2(entity, component, type, pool);
            });
            
            if (remove)
                Remove(entity, index);
        }
        
        private static void DrawRunTimeMode2(Entity entity, object component, Type type, IPool pool)
        {
            if(remove) return;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(component == null ? $"ERROR! {type.Name} IS NULL" : $"{type.Name}", EditorStyles.boldLabel);
            RemoveBtn();
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < fields.Length; i++)
                DrawTypeFieldRunTime(component, fields[i]);

            pool.Set(component,entity.id);
            // if(index >= entity.Components.Count) return;
            // entity.Components[index] = component;
        }
    }

}
