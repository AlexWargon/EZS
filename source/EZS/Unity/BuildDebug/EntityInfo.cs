using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfo : MonoBehaviour {
    public Text id;
    public Text componentsCount;

}

public class FieldView {
    public Text label;
    public Text fieldValue;
    public void Show(object value) {
        label.text = value.GetType().Name;
        fieldValue.text = value.ToString();
    }
}