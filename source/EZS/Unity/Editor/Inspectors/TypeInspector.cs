using System;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public interface ITypeInspector
    {
        Type GetTypeOfField();
        void SetTypeOfField(Type value);
        object Draw(string fieldName, object field);
        object DrawCollectionElement(Rect rect, object element, int index);
    }

    public abstract class TypeInspector<T> : ITypeInspector
    {
        public Type FieldType;
        public Type GetTypeOfField()
        {
            return FieldType;
        }
        public TypeInspector()
        {
            FieldType = typeof(T);
        }

        public void SetTypeOfField(Type value)
        {
            FieldType = value;
        }
        
        public object Draw(string fieldName, object field)
        {
            var v = (T) field;
            
            return DrawInternal(fieldName, ref v);
        }

        public abstract object DrawCollectionElement(Rect rect, object element, int index);

        protected abstract object DrawInternal(string fieldName, ref T field);

        public T DrawGeneric(string fieldName, ref T field) {
            var v = field;
            return DrawGenericInternal(fieldName, ref field);
        }

        protected abstract T DrawGenericInternal(string fieldName, ref T field);
    }

}