using UnityEditor;

namespace Wargon.ezs.Unity
{
    public class DoubleInspector : TypeInspector<double>
    {
        protected override object Draw(string fieldName, ref double field)
        {
            return EditorGUILayout.DoubleField($"    {fieldName}", field);
        }
    }
}