using System;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public interface ITypeInspector
    {
        Type GetTypeOfField();
        void SetTypeOfField(Type value);
        object Render(string fieldName, object field);
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
        
        public object Render(string fieldName, object field)
        {
            var v = (T) field;
            
            return Draw(fieldName, ref v);
        }

        public abstract object DrawCollectionElement(Rect rect, object element, int index);

        protected abstract object Draw(string fieldName, ref T field);
    }

}