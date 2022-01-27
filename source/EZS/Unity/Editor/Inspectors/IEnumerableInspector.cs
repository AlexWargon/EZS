using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class IEnumerableInspector : TypeInspector<IEnumerable>
    {
        protected override object Draw(string fieldName, ref IEnumerable field)
        {
            EditorGUILayout.BeginVertical();
            var j = 0;

            foreach (var element in field)
            {
                if (element == null) continue;
                ComponentInspector.SetCollectionElement(element, $"{fieldName} [{j}]", element.GetType());
                j++;
            }

            if (j < 1)
            {
                EditorGUILayout.LabelField("    Empty");
            }
            if (field is IList list)
            {
                if (GUILayout.Button(new GUIContent("+"), GUILayout.Width(24), GUILayout.Height(24)))
                {
                    var elementsType = list.GetType().GetGenericArguments().Single();
                    if (elementsType == typeof(string))
                        list.Add(string.Empty);
                    else
                        list.Add(Activator.CreateInstance(list.GetType().GetGenericArguments().Single()));
                }

                if (GUILayout.Button(new GUIContent("-"), GUILayout.Width(24), GUILayout.Height(24)))
                    list.RemoveAt(list.Count - 1);
            }

            EditorGUILayout.EndVertical();
            return field;
        }
    }
}