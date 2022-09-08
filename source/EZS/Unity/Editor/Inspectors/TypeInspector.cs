using System;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public interface ITypeInspector
    {
        Type GetGenericType();
        object DrawIn(string fieldName, object field);
        object DrawCollectionElement(Rect rect, object element);
    }

    public abstract class TypeInspector<T> : ITypeInspector
    {
        public Type FieldType;
        public Type GetGenericType()
        {
            return FieldType;
        }
        
        public TypeInspector()
        {
            FieldType = typeof(T);
        }

        public object DrawIn(string fieldName, object field)
        {
            var v = (T) field;
            
            return Draw(fieldName, ref v);
        }

        public abstract object DrawCollectionElement(Rect rect, object element);

        protected abstract object Draw(string fieldName, ref T field);
    }

}