using System.Collections;
using UnityEngine;

public class ActionKeys : MonoBehaviour
{
    public const KeyCode MOUSE_ENABLE = KeyCode.F1;
    public const KeyCode JUMP = KeyCode.Space;
    public const KeyCode INTERACT = KeyCode.E;
    public const KeyCode USE_MEDIPACK = KeyCode.Alpha1;
    public const KeyCode PUNCH = KeyCode.P;
    public const KeyCode KICK = KeyCode.K;
    public const KeyCode CROUCH = KeyCode.C;
    public const KeyCode LEDGE_RELEASE = KeyCode.LeftShift;
    public const KeyCode CLIMB_LEDGE = KeyCode.Space;
    public const KeyCode SPRINT = KeyCode.LeftShift;
    public const KeyCode WALK_DOWN_LEDGE = KeyCode.LeftControl;
    public const KeyCode OPEN_STATS_PANEL = KeyCode.Tab;
    public const KeyCode OPEN_BACKPACK = KeyCode.Escape;

    public const KeyCode FORWARD_KEY = KeyCode.W;
    public const KeyCode BACKWARD_KEY = KeyCode.S;
    public const KeyCode LEFT_KEY = KeyCode.A;
    public const KeyCode RIGHT_KEY = KeyCode.D;

    public static bool IsKeyPressed(KeyCode key)
    {
        return UnityEngine.Input.GetKeyDown(key);
    }

    public static bool IsKeyPressedWithControl(KeyCode keyCode)
    {
        return UnityEngine.Input.GetKey(KeyCode.LeftControl)
            && UnityEngine.Input.GetKeyDown(keyCode);
    }
}
