using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 새로운 Input System 사용: 키보드 입력 감지
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed && KeyAction != null)
            KeyAction.Invoke();

        // 새로운 Input System 사용: 마우스 입력 감지
        if (MouseAction != null)
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                    MouseAction.Invoke(Define.MouseEvent.Click);
                _pressed = false;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
