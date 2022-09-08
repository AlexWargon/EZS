using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class LayerMaskInspector : TypeInspector<LayerMask>
    {
        public override object DrawCollectionElement(Rect rect, object element)
        {
            throw new System.NotImplementedException();
        }

        protected override object Draw(string fieldName, ref LayerMask field)
        {
            LayerMask tempMask = EditorGUILayout.MaskField($"    {fieldName}",
                InternalEditorUtility.LayerMaskToConcatenatedLayersMask(field), InternalEditorUtility.layers);
            return InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
        }
    }
}