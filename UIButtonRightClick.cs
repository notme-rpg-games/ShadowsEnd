using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Events;

public class UIButtonRightClick : MonoBehaviour, IPointerClickHandler {

    public UnityEngine.Events.UnityEvent onRightClick;

    [System.Serializable]
    public class ButtonRClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    private ButtonRClickedEvent m_OnRightClick = new ButtonRClickedEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Invoke the right click event
            onRightClick.Invoke();
        }
    }
}