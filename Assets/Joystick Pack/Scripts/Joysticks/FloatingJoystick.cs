using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{

    private void Awake()
    {
       
    }

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    private void Update()
    {
        
        this.GameObjectToSendMessageTo.SendMessage(this.MethodName, this.Direction);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}