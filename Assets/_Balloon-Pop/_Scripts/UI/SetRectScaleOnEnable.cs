using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRectScaleOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
    }
}
