using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Input
{
    public KeyCode primary;
    public KeyCode alternate;

    public bool Pressed()
    {
        return ControlNotPressed()
            && (UnityEngine.Input.GetKey(primary) || UnityEngine.Input.GetKey(alternate));
    }

    private bool ControlNotPressed()
    {
        return !UnityEngine.Input.GetKey(KeyCode.LeftControl);
    }

    public bool PressedDown()
    {
        return ControlNotPressed()
            && (UnityEngine.Input.GetKeyDown(primary) || UnityEngine.Input.GetKeyDown(alternate));
    }

    public bool PressedUp()
    {
        return ControlNotPressed()
            && (UnityEngine.Input.GetKeyUp(primary) || UnityEngine.Input.GetKeyUp(alternate));
    }
}

public class PlayerInputs : MonoBehaviour
{
    public const string MouseXString = "Mouse X";
    public const string MouseYString = "Mouse Y";
    public const string MouseScrollString = "Mouse ScrollWheel";

    public Input forward;
    public Input backward;
    public Input left;
    public Input right;
    public Input sprint;

    public int MoveAxisForwardRaw
    {
        get
        {
            if (forward.Pressed() && backward.Pressed())
            {
                return 0;
            }
            else if (forward.Pressed())
            {
                return 1;
            }
            else if (backward.Pressed())
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public int MoveAxisRightRaw
    {
        get
        {
            if (right.Pressed() && left.Pressed())
            {
                return 0;
            }
            else if (right.Pressed())
            {
                return 1;
            }
            else if (left.Pressed())
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public static float MouseXInput
    {
        get => UnityEngine.Input.GetAxis(MouseXString);
    }
    public static float MouseYInput
    {
        get => UnityEngine.Input.GetAxis(MouseYString);
    }
    public static float MouseScrollInput
    {
        get => UnityEngine.Input.GetAxis(MouseScrollString);
    }
}
