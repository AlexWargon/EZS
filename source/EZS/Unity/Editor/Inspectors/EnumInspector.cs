using System;
using UnityEditor;

namespace Wargon.ezs.Unity
{
    public class EnumInspector : TypeInspector<Enum>
    {
        protected override object Draw(string fieldName, ref Enum field)
        {
            return EditorGUILayout.EnumPopup($"    {fieldName}", field);
        }
    }
}