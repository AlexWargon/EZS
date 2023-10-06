using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Wargon.DI
{
    public class Context
    {
        private readonly MethodInfo constructor;
        private readonly Type contextType;
        private readonly Type[] contructorParametersTypes;
        private readonly DependencyContainer di;
        private readonly List<(string, Type, DiType)> fieldsToInject = new List<(string, Type, DiType)>();
        private readonly bool isMonoBehaviourWithConstructor;
        private bool binded;
        private BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public Context(Type type, DependencyContainer di) {

            contextType = type;
            this.di = di;
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                constructor = type.GetMethod("Construct");
                if (constructor != null)
                {
                    var parameters = constructor.GetParameters();
                    contructorParametersTypes = new Type[constructor.GetParameters().Length];
                    for (var i = 0; i < contructorParametersTypes.Length; i++)
                        contructorParametersTypes[i] = parameters[i].ParameterType;
                    isMonoBehaviourWithConstructor = true;
                }
            }

            var fields = type.GetFields(bindingFlags);
            foreach (var fieldInfo in fields)
            {
                if (FieldHasAttribute(fieldInfo))
                {
                    var diType = DiType.New;
                    if (di.HasGlobal(fieldInfo.FieldType))
                        diType = DiType.Global;
                    else if (di.HasSingle(fieldInfo.FieldType))
                        diType = DiType.Single;

                    fieldsToInject.Add((fieldInfo.Name, fieldInfo.FieldType, diType));
                    //Log.Show(new Color(0.98f, 0.42f, 1f), $"Field [{fieldInfo.FieldType}] of [{contextType.Name}] added like must be injected");
                }
            }
        }

        private static bool FieldHasAttribute(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0;
        }

        public void Inject<T>(T obj) where T : class
        {
            if(binded) return;
            for (var i = 0; i < fieldsToInject.Count; i++)
            {
                switch (fieldsToInject[i].Item3)
                {
                    case DiType.New:
                        if(di.HasSingle(fieldsToInject[i].Item2)) 
                            contextType.GetField(fieldsToInject[i].Item1,bindingFlags)?.SetValue(obj, di.GetContainer(fieldsToInject[i].Item2).Get());
                        else Debug.LogError($"{fieldsToInject[i].Item1} can't be inhected. There no isntance in DI");

                        break;
                    case DiType.Single:
                        if(di.HasSingle(fieldsToInject[i].Item2))
                            contextType.GetField(fieldsToInject[i].Item1,bindingFlags)?.SetValue(obj, di.GetContainer(fieldsToInject[i].Item2).Get());
                        else Debug.LogError($"{fieldsToInject[i].Item1} can't be inhected. There no isntance in DI");
                        break;
                    case DiType.Global:
                        if(di.HasGlobal((fieldsToInject[i].Item2)))
                            contextType.GetField(fieldsToInject[i].Item1,bindingFlags)?.SetValue(obj, DependencyContainer.Globals[fieldsToInject[i].Item2]);
                        else Debug.LogError($"{fieldsToInject[i].Item1} can't be inhected. There no isntance in DI");
                        break;
                }
                //Log.Show(new Color(0.49f, 0.62f, 1f), $"Field [{fieldsToInject[i].Item2}] Binded to [{contextType.Name}]");
            }
            if (isMonoBehaviourWithConstructor)
            {
                var newParams = new object[contructorParametersTypes.Length];
                for (var i = 0; i < newParams.Length; i++)
                    newParams[i] = di.GetContainer(fieldsToInject[i].Item2).Get();
                constructor.Invoke(obj, newParams);
            }
            //Log.Show(Color.yellow, $"[{contextType}] Binded");
            binded = true;
        }
    }
}