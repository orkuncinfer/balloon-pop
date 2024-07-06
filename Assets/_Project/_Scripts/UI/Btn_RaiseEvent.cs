using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Btn_RaiseEvent : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private EventField _eventField;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Btn_RaiseEvent.OnPointerClick");
        _eventField.Raise();
    }
}
