using System.Collections.Generic;

namespace Wargon.ezs.Unity {
    public static class Inspectors {
        public static class Field {
            public static AnimationCurveInspector AnimationCurve = new AnimationCurveInspector();
            public static BoolInspector Bool = new BoolInspector();
            public static Color32Inspector Color32 = new Color32Inspector();
            public static ColorInspector Color = new ColorInspector();
            public static DoubleInspector Double = new DoubleInspector();
            public static EntityInspector Entity = new EntityInspector();
            public static EnumInspector Enum = new EnumInspector();
            public static FloatInspector Float = new FloatInspector();
            public static GradientInspector Gradient = new GradientInspector();
            public static IntInspector Int = new IntInspector();
            public static LayerMaskInspector Layer = new LayerMaskInspector();
            public static ListInspector List = new ListInspector();
            public static QuaternionInspector Quaternion = new QuaternionInspector();
            public static StringInspector String = new StringInspector();
            public static Vector2Inspector Vector2 = new Vector2Inspector();
            public static Vector3Inspector Vector3 = new Vector3Inspector();
            public static Vector4Inspector Vector4 = new Vector4Inspector();
            public static Float2Inspector Float2 = new Float2Inspector();
            public static Float3Inspector Float3 = new Float3Inspector();
            private static Dictionary<string, UnityObjectInspector> UnityObjectInspectors =
                new Dictionary<string, UnityObjectInspector>();
            
            public static UnityObjectInspector UnityObject<T>() {
                if (UnityObjectInspectors.TryGetValue(nameof(T), out var inspector))
                    return inspector;
                UnityObjectInspectors.Add(nameof(T), UnityObjectInspector.New(typeof(T)));
                return UnityObjectInspectors[nameof(T)];
            }

            private static readonly Dictionary<string, ITypeInspector> _inspecors = new Dictionary<string, ITypeInspector>();
            public static TypeInspector<T> Get<T>() {
                
                return (TypeInspector<T>)_inspecors[nameof(T)];
            }

            public static void Draw<T>(string fieldName, ref T fieldValue) {
                var inspecor = Get<T>();
                inspecor.DrawGeneric(fieldName, ref fieldValue);
            }
        }
    }
}