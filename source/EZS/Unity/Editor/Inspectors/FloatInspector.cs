using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Wargon.ezs.Unity
{
    public class FloatInspector : TypeInspector<float>
    {
        protected override object Draw(string fieldName, ref float field)
        {
            return EditorGUILayout.FloatField($"    {fieldName}", field);
        }
    }
}