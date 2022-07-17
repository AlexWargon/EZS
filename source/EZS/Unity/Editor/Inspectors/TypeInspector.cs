using System;
using UnityEngine.UIElements;

namespace Wargon.ezs.Unity
{
    public interface ITypeInspector
    {
        object DrawIn(string fieldName, object field);
    }

    public abstract class TypeInspector<T> : ITypeInspector
    {
        public Type FieldType;

        protected TypeInspector()
        {
            FieldType = typeof(T);
        }

        public object DrawIn(string fieldName, object field)
        {
            var v = (T) field;
            
            return Draw(fieldName, ref v);
        }

        protected abstract object Draw(string fieldName, ref T field);
    }
}