using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    
    [Header("Aim Settings")]
    [SerializeField] private AimMode aimMode = AimMode.MousePosition;
    [SerializeField] private bool rotateFirePoint = true;
    [SerializeField] private bool visualizeAim = true;
    
    [Header("Joystick Settings")]
    [SerializeField] private float joystickDeadzone = 0.2f;
    
    private Vector2 currentAimDirection;
    private Vector2 joystickAimInput;
    private IPlayerOrientation orientationController;
    
    public Vector2 AimDirection => currentAimDirection;
    
    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (firePoint == null)
        {
            Debug.LogWarning("[AIM CONTROLLER] FirePoint not assigned! Looking for child 'FirePoint'...");
            Transform found = transform.Find("FirePoint");
            if (found != null)
                firePoint = found;
        }
    }
    
    void Start()
    {
        Player player = GetComponent<Player>();
        if (player != null && player.Orientation != null)
        {
            orientationController = player.Orientation;
        }
    }
    
    void Update()
    {
        UpdateAimDirection();
        
        if (rotateFirePoint && firePoint != null)
        {
            UpdateFirePointRotation();
        }
    }
    
    void UpdateAimDirection()
    {
        switch (aimMode)
        {
            case AimMode.MousePosition:
                UpdateAimFromMouse();
                break;
                
            case AimMode.Joystick:
                UpdateAimFromJoystick();
                break;
                
            case AimMode.Hybrid:
                UpdateAimHybrid();
                break;
                
            case AimMode.FacingDirection:
                UpdateAimFromFacing();
                break;
        }
    }
    
    void UpdateAimFromMouse()
    {
        if (mainCamera == null)
            return;
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            currentAimDirection = direction;
        }
    }
    
    void UpdateAimFromJoystick()
    {
        if (joystickAimInput.magnitude > joystickDeadzone)
        {
            currentAimDirection = joystickAimInput.normalized;
        }
        else if (orientationController != null)
        {
            currentAimDirection = Vector2.right * orientationController.FacingDirection;
        }
    }
    
    void UpdateAimHybrid()
    {
        if (joystickAimInput.magnitude > joystickDeadzone)
        {
            currentAimDirection = joystickAimInput.normalized;
        }
        else
        {
            UpdateAimFromMouse();
        }
    }
    
    void UpdateAimFromFacing()
    {
        if (orientationController != null)
        {
            currentAimDirection = Vector2.right * orientationController.FacingDirection;
        }
    }
    
    void UpdateFirePointRotation()
    {
        if (currentAimDirection.sqrMagnitude < 0.01f)
            return;
        
        float angle = Mathf.Atan2(currentAimDirection.y, currentAimDirection.x) * Mathf.Rad2Deg;
        
        if (orientationController != null && orientationController.FacingDirection < 0)
        {
            angle = 180f - angle;
        }
        
        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    public void OnAim(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        joystickAimInput = input;
    }
    
    public void SetAimMode(AimMode mode)
    {
        aimMode = mode;
    }
    
    public Vector2 GetAimDirectionWorld()
    {
        return currentAimDirection;
    }
    
    void OnDrawGizmos()
    {
        if (!visualizeAim || !Application.isPlaying)
            return;
        
        if (currentAimDirection.sqrMagnitude > 0.01f)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position;
            Vector3 end = start + (Vector3)currentAimDirection * 2f;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, 0.1f);
        }
    }
}

public enum AimMode
{
    MousePosition,
    Joystick,
    Hybrid,
    FacingDirection
}
