using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    private InputAction mousePosition;
    private Rigidbody rigBody;

    public Vector2 screenResolution { get; set; }
    public Vector2 LastCursorPosition { get; set; }

    private void Awake()
    {
        mousePosition = PlayerUI.inputActions.Menu.Pointer;
        mousePosition.performed += MoveCursorToTarget;
        rigBody = GetComponent<Rigidbody>();
        rigBody.constraints = RigidbodyConstraints.FreezePositionY;
        screenResolution = new Vector2(Screen.currentResolution.width,Screen.currentResolution.height);
    }

    private void OnDisable()
    {
        mousePosition.performed -= MoveCursorToTarget;
    }

    private void Start()
    {
        CursorSwitch.HideCursor();
        PlayerUI.SwitchActionMap(PlayerUI.inputActions.Menu); //Enable Menu actionMap
        LastCursorPosition = screenResolution * 0.5f;
    }

    private void FixedUpdate()
    {
        rigBody.MovePosition(RemapCursorToScene(LastCursorPosition));
    }

    private void MoveCursorToTarget(InputAction.CallbackContext context)
    {
        LastCursorPosition = mousePosition.ReadValue<Vector2>();
    }
    
    //Remap cursor position on screen to limited area at game view
    private Vector3 RemapCursorToScene(Vector2 cursorPosition)
    {
        //CursorY = SceneZ
        float cursorMinX = 0f;
        float cursorMinY = 0f;
        float cursorMaxX = screenResolution.x;
        float cursorMaxY = screenResolution.y;
        float sceneMinX = 25f;
        float sceneMinZ = 2f;
        float sceneMaxX = 33.3f;
        float sceneMaxZ = 12f;

        if (cursorMaxX == 0 || cursorMaxY == 0) return Vector3.zero;

        float xValue = (cursorPosition.x - cursorMinX) * (sceneMaxX - sceneMinX) / (cursorMaxX - cursorMinX) + sceneMinX;
        float zValue = (cursorPosition.y - cursorMinY) * (sceneMaxZ - sceneMinZ) / (cursorMaxY - cursorMinY) + sceneMinZ;
        float yValue = 2f;

        return new Vector3(xValue, yValue, zValue);
    }
}