using UnityEditor;

namespace Wargon.ezs.Unity
{
    public class StringInspector : TypeInspector<string>
    {
        protected override object Draw(string fieldName, ref string field)
        {
            return EditorGUILayout.TextField($"    {fieldName}", field);
        }
    }
}