using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class QuaternionInspector : TypeInspector<Quaternion>
    {
        public override object DrawCollectionElement(Rect rect, object element, int index)
        {
            throw new System.NotImplementedException();
        }

        protected override object DrawInternal(string fieldName, ref Quaternion field)
        {
            var vec = QuaternionToVector4(field);
            var tempVec = EditorGUILayout.Vector4Field($"    {fieldName}", vec);
            return Vector4ToQuaternion(tempVec);
        }

        protected override Quaternion DrawGenericInternal(string fieldName, ref Quaternion field) {
            var vec = QuaternionToVector4(field);
            var tempVec = EditorGUILayout.Vector4Field($"    {fieldName}", vec);
            return Vector4ToQuaternion(tempVec);
        }

        private static Vector4 QuaternionToVector4(Quaternion rot)
        {
            return new Vector4(rot.x, rot.y, rot.z, rot.w);
        }

        private static Quaternion Vector4ToQuaternion(Vector4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
    }
}