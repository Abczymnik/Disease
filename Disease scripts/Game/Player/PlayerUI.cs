using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    public static PlayerControls inputActions = new PlayerControls();

    void Start()
    {
        SwitchActionMap(inputActions.Gameplay);
    }

    //Switch action map in one place w/o multiple instances
    public static void SwitchActionMap(InputActionMap actionMap)
    {
        if(actionMap.enabled) { return; }

        inputActions.Disable();
        actionMap.Enable();
    }

    public static void BlockInput()
    {
        inputActions.Disable();
    }
}
 