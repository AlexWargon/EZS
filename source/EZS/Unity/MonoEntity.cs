using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Wargon.ezs.Unity {
    [DisallowMultipleComponent]
    public class MonoEntity : MonoBehaviour {
        public TextAsset json;
        public Entity Entity;
        [SerializeReference] public List<object> Components = new List<object>();
        public bool runTime;
        public bool destroyObject;
        public bool destroyComponent;
        public int id;
        private bool converted;
        private World world;
        public int ComponentsCount => runTime ? Entity.GetEntityData().componentTypes.Count : Components.Count;
        private void Start() {
            ConvertToEntity();
        }
#if UNITY_EDITOR
        private void OnEnable() {
            Enable();
        }
        private void OnDisable() {
            Disable();
        }
#endif

        private void OnDestroy() {
            if (!destroyObject)
                if (world != null)
                    if (world.Alive)
                        if (!Entity.IsNULL())
                            Entity.Destroy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ConvertToEntity() {
            if (converted) return;
            Entity = MonoConverter.GetWorld().CreateEntity();
            world = Entity.World;
#if UNITY_EDITOR
            gameObject.name = $"{gameObject.name} ID:{Entity.id.ToString()}";
#endif
            id = Entity.id;
            MonoConverter.Execute(Entity, Components);
            converted = true;
            if (destroyComponent) Destroy(this);
            if (destroyObject) Destroy(gameObject);
            runTime = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : new(){
            return ref Entity.Get<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>() where T : new() {
            Entity.Remove<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActive(bool state) {
            if (state)
                Enable();
            else
                Disable();
            gameObject.SetActive(state);
        }
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Disable() {
            if (!converted) return;
            if (!world.Alive) return;
            if (Entity.IsNULL()) return;
            Entity.Set<Inactive>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Enable() {
            if (!converted) return;
            if (Entity.Has<Inactive>())
                Entity.Remove<Inactive>();
        }

        public void DestroyWithoutEntity() {
            destroyObject = true;
            Destroy(gameObject);
        }
    }

    [EcsComponent]
    public class View {
        public MonoEntity Value;
    }

    public static class MonoEntityExtension {
        public static string ToJson(this MonoEntity monoEntity) {
            var components = monoEntity.Components;
            var monoEntityJson = $"{monoEntity.name} :";
            monoEntityJson += System.Environment.NewLine;
            for (var index = 0; index < components.Count; index++) {
                var component = components[index];
                var type = component.GetType();
                var componentJson = string.Empty;
                string classOrStruct = type.IsValueType ? "struct" : "class";
                componentJson += $"ComponentIndex:{index} [type:{type.Name},{classOrStruct}:";
                componentJson += System.Environment.NewLine;
                componentJson += "      Fields:";
                for (var i = 0; i < type.GetFields().Length; i++) {
                    var fieldInfo = type.GetFields()[i];
                    componentJson += System.Environment.NewLine;
                    componentJson += "      ";
                    var name = fieldInfo.Name;
                    var fieldType = fieldInfo.FieldType;
                    var fieldValue = fieldInfo.GetValue(component);
                    componentJson += $"fieldIndex:{i} ;";
                    componentJson += $"Name:{name} ;";
                    componentJson += $"Type:{fieldType} ;";
                    componentJson += $"Value:";
                    if (fieldValue != null)
                        componentJson += fieldValue.ToString();
                    else
                        componentJson += "NULL";
                    componentJson += ";";
                }

                componentJson += "];";
                monoEntityJson += componentJson;
                monoEntityJson += System.Environment.NewLine;
            }

            System.IO.File.WriteAllText(Application.dataPath + $"/{monoEntity.name}.json", monoEntityJson);
            return monoEntityJson;
        }

        public static void FromJson(this MonoEntity monoEntity, TextAsset file) {
            var json = file.text;
            string[] fLines = System.Text.RegularExpressions.Regex.Split ( json, "\n|\r|\r\n" );
            for (var i = 0; i < fLines.Length; i++) {
                Debug.Log(i);
                var line = fLines[i];
                if (line.Contains($"ComponentIndex:{i}")) {
                    int pFrom = line.IndexOf("type:") + "type:".Length;
                    int pTo = line.LastIndexOf(",");

                    var result = line.Substring(pFrom, pTo - pFrom);
                    Debug.Log(result);
                }
            }
        }

    }
}