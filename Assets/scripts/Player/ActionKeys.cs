using System.Collections;
using UnityEngine;

public class ActionKeys : MonoBehaviour
{
    //keys with no modifier
    public const KeyCode JUMP = KeyCode.Space;
    public const KeyCode PICKUP_OBJECT = KeyCode.E;
    public const KeyCode USE_MEDIPACK = KeyCode.Alpha1;
    public const KeyCode PUNCH = KeyCode.P;
    public const KeyCode KICK = KeyCode.K;
    public const KeyCode CROUCH = KeyCode.C;
    public const KeyCode LEDGE_RELEASE = KeyCode.LeftShift;
    public const KeyCode CLIMB_LEDGE = KeyCode.Space;
    public const KeyCode SPRINT = KeyCode.LeftShift;

    //keys with control key
    public const KeyCode OPEN_STATS_PANEL = KeyCode.A;

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
