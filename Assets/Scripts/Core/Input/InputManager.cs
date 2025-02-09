using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static bool playerGamepad;
    public float DeadZone;

    public enum InputType { GamePad, Keyboard};
    public InputType playerInput;
    public enum InputDeviceType { Xbox, DualShock, Keyboard};
    public static InputDeviceType playerDevice;

    private Vector2 lastStickDir;
    private bool stickFrame;

    // Start is called before the first frame update
    public void Awake()
    {
        if (!instance)
        {
            instance = this;
            playerInput = InputType.Keyboard;
            playerDevice = InputDeviceType.Keyboard;
        }
    }

    public static bool IsStickMovementThisFrame()
    {
        return instance.stickFrame;
    }

    public static bool isUsingController()
    {
        return instance.playerInput != InputType.Keyboard;
    }

    public static InputType getPlayerDevice()
    {
        return instance.playerInput;
    }

    public static bool ActionPress(InputActionType action)
    {
        ButtonControl b = instance.GetAction(action);
        if (b != null)
        {
            return b.wasPressedThisFrame;
        }
        return false;
    }

    public static bool ActionHold(InputActionType action)
    {
        ButtonControl b = instance.GetAction(action);
        if (b != null)
        {
            return b.isPressed;
        }
        return false;
    }

    private ButtonControl GetAction(InputActionType action)
    {
        ButtonControl b = null;
        Gamepad pGamepad = null;
        if (playerInput == InputType.GamePad)
        {
            if (Gamepad.all.Count > 0)
            {
                pGamepad = Gamepad.all[0];
            }
            else
            {
                return b;
            }
        }

        if (playerInput == InputType.Keyboard)
        {
            switch (action)
            {
                case InputActionType.MENU_LEFT:
                    return Keyboard.current.leftArrowKey;
                case InputActionType.MENU_RIGHT:
                    return Keyboard.current.rightArrowKey;
                case InputActionType.MENU_UP:
                    return Keyboard.current.upArrowKey;
                case InputActionType.MENU_DOWN:
                    return Keyboard.current.downArrowKey;
                case InputActionType.MENU_OK:
                    return Keyboard.current.spaceKey;
                case InputActionType.MENU_CANCEL:
                    return Keyboard.current.escapeKey;
                case InputActionType.CANCEL:
                    return Mouse.current.rightButton;
                case InputActionType.ABILITY:
                    return Keyboard.current.spaceKey;
                case InputActionType.ATTACK:
                    return Mouse.current.leftButton;
                case InputActionType.ULTIMATE:
                    return Mouse.current.rightButton;
            }
        }
        else
        {
            switch (action)
            {
                case InputActionType.MENU_LEFT:
                    return pGamepad.leftStick.left;
                case InputActionType.MENU_RIGHT:
                    return pGamepad.leftStick.right;
                case InputActionType.MENU_UP:
                    return pGamepad.leftStick.up;
                case InputActionType.MENU_DOWN:
                    return  pGamepad.leftStick.down;
                case InputActionType.MENU_OK:
                    return pGamepad.buttonSouth;
                case InputActionType.MENU_CANCEL:
                    return pGamepad.buttonEast;
                case InputActionType.CANCEL:
                    return pGamepad.bButton;
                case InputActionType.ABILITY:
                    return pGamepad.leftShoulder;
                case InputActionType.ULTIMATE:
                    return pGamepad.rightShoulder;
            }
        }
        
        return b;
    }

    public static string GetActionButtonIcon(InputActionType action)
    {
        InputDeviceType device = playerDevice;
        /*
        switch (action)
        {
            case InputActionType.CANCEL:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_B_White_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Circle_Light>";
                else return "<sprite name=T_Mouse_Right_Key_White>";
            case InputActionType.MENU_CANCEL:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_B_White_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Circle_Light>";
                else return "<sprite name=T_Mouse_Right_Key_White>";
            case InputActionType.INTERACT:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_X_White_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Square_Light>";
                else return "<sprite name=T_Mouse_Left_Key_White>";
            case InputActionType.MAP:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_Share_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Share_Light>";
                else return "<sprite name=T_M_Key_White>";
            case InputActionType.CROUCH:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_LT_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_L2_Light>";
                else return "<sprite name=T_Ctrl_Key_White>";
            case InputActionType.SPRINT:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_RT_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_R2_Light>";
                else return "<sprite name=T_Shift_Key_White>";
            case InputActionType.MENU_OK:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_A_LT_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Cross_Light>";
                else return "<sprite name=T_Mouse_Left_Key_White>";
            case InputActionType.MAP_MOVE:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_L_2D_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_L_2D_Light>";
                else return "<sprite name=T_Mouse_Left_Key_White>";
            case InputActionType.MAP_CENTER:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_Y_White_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Triangle_Light>";
                else return "<sprite name=T_Space_Key_White>";
            case InputActionType.TOGGLE_MAP_VIEW:
                if (device == InputDeviceType.Xbox) return "<sprite name=T_X_X_White_Light>";
                else if (device == InputDeviceType.DualShock) return "<sprite name=T_P5_Square_Light>";
                else return "<sprite name=T_Shift_Key_White>";
            case InputActionType.MAP_ZOOM:
                if (device == InputDeviceType.Xbox) return "";
                else if (device == InputDeviceType.DualShock) return "";
                else return "<sprite name=T_Mouse_Wheel_Key_White>";
        }*/
        return "";
    }


    public static bool ButtonPress(int player, string button)
    {
        Gamepad pad = null;
        if (player == 0 && playerGamepad)
        {
            pad = Gamepad.all[0];
        }
        if (pad != null)
        {
            switch (button)
            {
                case "A":
                    return pad.aButton.wasPressedThisFrame;
                case "B":
                    return pad.bButton.wasPressedThisFrame;
                case "X":
                    return pad.xButton.wasPressedThisFrame;
                case "Y":
                    return pad.aButton.wasPressedThisFrame;
                case "Select":
                    return pad.selectButton.wasPressedThisFrame;
                case "Start":
                    return pad.startButton.wasPressedThisFrame;
                case "LT":
                    return pad.leftTrigger.wasPressedThisFrame;
                case "RT":
                    return pad.rightTrigger.wasPressedThisFrame;
                case "LB":
                    return pad.leftShoulder.wasPressedThisFrame;
                case "RB":
                    return pad.rightShoulder.wasPressedThisFrame;
            }
        }
        return false;
    }

    public static bool ButtonHold(string button)
    {
        Gamepad pad = Gamepad.all[0];
        if (pad != null)
        {
            switch (button)
            {
                case "A":
                    return pad.aButton.isPressed;
                case "B":
                    return pad.bButton.isPressed;
                case "X":
                    return pad.xButton.isPressed;
                case "Y":
                    return pad.aButton.isPressed;
                case "Select":
                    return pad.selectButton.isPressed;
                case "Start":
                    return pad.startButton.isPressed;
                case "LT":
                    return pad.leftTrigger.isPressed;
                case "RT":
                    return pad.rightTrigger.isPressed;
                case "LB":
                    return pad.leftShoulder.isPressed;
                case "RB":
                    return pad.rightShoulder.isPressed;
            }
        }
        return false;
    }

    public static bool ButtonRelease(int player, string button)
    {
        Gamepad pad = Gamepad.all[0];
        
        if (pad != null)
        {
            switch (button)
            {
                case "A":
                    return pad.aButton.wasReleasedThisFrame;
                case "B":
                    return pad.bButton.wasReleasedThisFrame;
                case "X":
                    return pad.xButton.wasReleasedThisFrame;
                case "Y":
                    return pad.aButton.wasReleasedThisFrame;
                case "Select":
                    return pad.selectButton.wasReleasedThisFrame;
                case "Start":
                    return pad.startButton.wasReleasedThisFrame;
                case "LT":
                    return pad.leftTrigger.wasReleasedThisFrame;
                case "RT":
                    return pad.rightTrigger.wasReleasedThisFrame;
                case "LB":
                    return pad.leftShoulder.wasReleasedThisFrame;
                case "RB":
                    return pad.rightShoulder.wasReleasedThisFrame;
            }
        }
        return false;
    }

    public static Vector2 GetLeftStick()
    {
        Gamepad pGamepad = null;
        if (instance.playerInput == InputType.GamePad)
        {
            if (Gamepad.all.Count > 0)
            {
                pGamepad = Gamepad.all[0];
            }
            else
            {
                instance.lastStickDir = Vector2.zero;
                return Vector2.zero;
            }
        }
        if (instance.playerInput == InputType.Keyboard)
        {
            instance.lastStickDir = Vector2.zero;
            return Vector2.zero;
        }
        Vector2 value = pGamepad.leftStick.ReadValue();
        if (instance.lastStickDir.magnitude < 0.5f && value.magnitude >= 0.5f)
        {
            instance.stickFrame = true;
        }
        else
        {
            instance.stickFrame = false;
        }
        instance.lastStickDir = value;
        return value;
    }

    public static Vector2 GetRightStick()
    {
        Gamepad pGamepad = null;
        if (instance.playerInput == InputType.GamePad)
        {
            if (Gamepad.all.Count > 0)
            {
                pGamepad = Gamepad.all[0];
            }
            else
            {
                return Vector2.zero;
            }
        }
        return pGamepad.rightStick.ReadValue();
    }

    public static void Rumble(float time, float left, float right)
    {
        InputManager.instance.StartCoroutine(InputManager.instance.DoRumble(time, left, right));
    }

    public IEnumerator DoRumble(float time, float left, float right)
    {
        Gamepad pGamepad = null;
        if (playerInput == InputType.GamePad)
        {
            if (Gamepad.all.Count > 0)
            {
                pGamepad = Gamepad.all[0];
            }
            else
            {
                yield break;
            }
        }
        if (playerInput == InputType.Keyboard)
        {
            yield break;
        }
        pGamepad.SetMotorSpeeds(left, right);
        while (time > 0)
        {
            time -= Time.unscaledDeltaTime;
            yield return null;
        }
        pGamepad.SetMotorSpeeds(0f, 0f);
        yield break;
    }

    private InputDeviceType GetDeviceFromInput(InputType inputType)
    {
        if (inputType == InputType.GamePad)
        {
            if (Gamepad.all.Count < 1) return InputDeviceType.Keyboard;
            if (Gamepad.all[0] is XInputController) return InputDeviceType.Xbox;
            if (Gamepad.all[0] is DualShockGamepad) return InputDeviceType.DualShock;
            return InputDeviceType.Xbox;
        }
        return InputDeviceType.Keyboard;
    }

    void Update()
    {
        InputDeviceType oldInputType = playerDevice;
        if (Gamepad.all.Count == 0)
        {
            playerGamepad = false;
        }
        else
        {
            playerGamepad = true;
        }

        if (playerGamepad)
        {
            Vector2 stick = Gamepad.all[0].leftStick.value;
            Vector2 stick2 = Gamepad.all[0].rightStick.value;
            bool movingstick = stick.x < -0.2f || stick.x > 0.2f || stick.y < -0.2f || stick.y > 0.2f;
            bool movingstick2 = stick2.x < -0.2f || stick2.x > 0.2f || stick2.y < -0.2f || stick2.y > 0.2f;
            if (Gamepad.all[0].allControls.Any(x => x is ButtonControl button && x.IsPressed(0.1f) && !x.synthetic) || movingstick || movingstick2)
            {
                playerInput = InputType.GamePad;
            }
        }

        if (Keyboard.current.anyKey.IsPressed(0) || Mouse.current.delta.value != Vector2.zero)
        {
            playerInput = InputType.Keyboard;
        }

        playerDevice = GetDeviceFromInput(playerInput);
    }
}