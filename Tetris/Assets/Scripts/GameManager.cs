using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Flags]
    public enum eKeys
    {
        MoveDown = 1 << 0,
        MoveLeft = 1 << 1,
        MoveRight = 1 << 2,
        TurnLeft = 1 << 3,
        TurnRight = 1 << 4,
    }

    private eKeys PressKeys;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Debug.Log("Start");
        this.AddComponent<Field>();

        PressKeys = 0;
    }

    #region ì¸óÕèàóù
    public void OnMoveDown(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }
        PressKeys |= eKeys.MoveDown;
    }
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        PressKeys |= eKeys.MoveLeft;
    }
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        PressKeys |= eKeys.MoveRight;
    }
    public void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        PressKeys |= eKeys.TurnLeft;
    }
    public void OnTurnRight(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        PressKeys |= eKeys.TurnRight;
    }

    public void CleaePressKeys()
    {
        PressKeys = 0;
    }
    public eKeys GetPressKeys()
    {
        return PressKeys;
    }
    #endregion
}
