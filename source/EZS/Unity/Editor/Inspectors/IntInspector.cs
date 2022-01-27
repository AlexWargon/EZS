using UnityEditor;

namespace Wargon.ezs.Unity
{
    public class IntInspector : TypeInspector<int>
    {
        protected override object Draw(string fieldName, ref int field)
        {
            return EditorGUILayout.IntField($"    {fieldName}", field);
        }
    }
}