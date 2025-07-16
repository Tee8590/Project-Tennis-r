using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    private InputSystem_Actions inputSystem;
    private InputAction fireAction;
    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion
    public InputAction FireAction => inputSystem.Player.Jump;

    private Camera mainCamera;


    private void Awake()
    {
        inputSystem = new InputSystem_Actions();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        inputSystem.Enable();
    }
    private void OnDisable()
    {
        inputSystem.Disable();
    }
    void Start()
    {
        inputSystem.Touch.PrimaryTouch.started += ctx => StartTouchPrimary(ctx);
        inputSystem.Touch.PrimaryTouch.canceled += ctx => EndTouchPrimary(ctx);
    }

    public void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null) 
            OnStartTouch(Utils.ScreenToWorld(mainCamera, inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);

    }

    public void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) 
            OnEndTouch(Utils.ScreenToWorld(mainCamera, inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);

    }
    //position of the finger, for Trail Rendered
    public Vector2 PrimaryPosition(float zDepth = 10f)
    {
        return Utils.ScreenToWorld(mainCamera, inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>(), zDepth);
    }
    
}