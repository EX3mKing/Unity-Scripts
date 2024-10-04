using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
/// <summary>
/// Gathers <c>INPUT</c> from Unity's new Input System
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool crouch;
    public bool interact;
    public bool IsUsingKeyboardAndMouse
    {
        get
        {
            return pi.currentControlScheme == "Keyboard&Mouse";
        }
    }

    private PlayerInput pi;
    private InputAction sprintAction;

    public UnityEvent Interact;

    private void Awake()
    {
        instance = this;
        pi = GetComponent<PlayerInput>();
        sprintAction = pi.actions["Sprint"];
    }

    private void Update()
    {
        //print(pi.currentControlScheme);
        sprint = sprintAction.inProgress;
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
        //print("move");
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
        //print("look");
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
        //print("jump");
    }
    public void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
        //print("sprint: " + sprint);
    }
    public void OnCrouch(InputValue value)
    {
        crouch = !crouch;
        //print("crouch: " + crouch);
    }

    public void OnInteract(InputValue value)
    {
        interact = value.isPressed;
        if (value.isPressed) Interact.Invoke();
    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }
}
