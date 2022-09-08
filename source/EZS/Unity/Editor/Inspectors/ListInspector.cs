using System;
using System.Collections;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace Wargon.ezs.Unity
{
    public class ListInspector : TypeInspector<IList>
    {
        private ReorderableList _list;
        private string _nameOfField;
        private IList _target;
        private object _component;
        private bool _locked;
        public void Init(IList targetList, Type elementType)
        {
            if(_list != null) return;
            _list = new ReorderableList(targetList, elementType);
            // _list.onAddCallback = list =>
            // {
            //     
            // };
            _list.drawElementCallback = DrawListItems;
            _list.drawHeaderCallback = DrawHeader;
            _list.onReorderCallback = reorderableList =>
            {
                _locked = !reorderableList.draggable;
            };
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            if(_locked) return;
            var element = _list.list[index];
            ComponentInspector.SetCollectionElement(rect, element, index,$"{_nameOfField} [{index}]", element.GetType(), _target);
        }
        
        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _nameOfField);
        }
        
        public override object DrawCollectionElement(Rect rect, object element)
        {
            var o = (IList) element;
            element = Draw(_nameOfField, ref o);
            return element;
        }

        protected override object Draw(string fieldName, ref IList field)
        {
            _target = field;
            _list.list = _target;
            _nameOfField = fieldName;
            _list.DoLayoutList();
            return _target;
        }
    }
}