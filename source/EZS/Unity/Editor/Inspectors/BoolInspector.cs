using UnityEditor;

namespace Wargon.ezs.Unity
{
    public class BoolInspector : TypeInspector<bool>
    {
        protected override object Draw(string fieldName, ref bool item)
        {
            return EditorGUILayout.Toggle($"    {fieldName}", item);
        }
    }
}