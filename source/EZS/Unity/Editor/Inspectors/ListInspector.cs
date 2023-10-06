using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wargon.ezs.Unity
{
    public class ListInspector : TypeInspector<IList>
    {
        private Type elementType;
        private ReorderableList list;
        private bool locked;
        private string nameOfField;
        private IList target;

        public void Init(Type elementType)
        {
            if (list != null) return;
            this.elementType = elementType;
            list = new ReorderableList(target, this.elementType, true, true, true, true);
            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;
            list.showDefaultBackground = false;
            list.onReorderCallback = reorderableList => { locked = !reorderableList.draggable; };
            if (this.elementType == typeof(Object) || this.elementType.IsSubclassOf(typeof(Object)))
                list.onAddCallback = list =>
                {
                    if (!list.list.IsFixedSize)
                    {
                        list.list.Add(null);
                    }
                    else
                    {
                        var array = list.list as Array;
                        Resize(ref array, array.Length + 1);
                        list.list = array;
                    }
                };
        }

        private static void Resize(ref Array array, int newSize)
        {
            var elementType = array.GetType().GetElementType();
            var newArray = Array.CreateInstance(elementType, newSize);
            Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
            array = newArray;
        }

        public void SetTarget(IList target)
        {
            this.target = target;
            list.list = target;
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (locked) return;
            var element = target[index];
            ComponentInspectorInternal.SetCollectionElement(rect, element, index, $"{nameOfField} [{index}]", elementType,
                target);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, nameOfField);
        }

        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            var o = (IList) element;
            element = DrawInternal(nameOfField, ref o);
            return element;
        }

        protected override object DrawInternal(string fieldName, ref IList field)
        {
            target = field;
            list.list = field;
            nameOfField = fieldName;
            list.DoLayoutList();
            return target;
        }

        protected override IList DrawGenericInternal(string fieldName, ref IList field) {
            return (IList)DrawInternal(fieldName, ref field);
        }
    }
}