using UnityEngine;
using UnityEngine.InputSystem;
using TheHunt.Interaction;
using TheHunt.Inventory;
using TheHunt.UI;
using TheHunt.Input;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RamMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool RunInput { get; private set; }
    public bool GrabInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool ReloadInput { get; private set; }
    public bool AimInput { get; private set; }

    [SerializeField] private float inputHoldTime = 0.2f;
    [SerializeField] private float jumpInputStartTime;
    
    private bool fireInputQueued = false;
    private bool reloadInputQueued = false;
    
    [Header("Dependencies")]
    [SerializeField] private InputContextManager inputContextManager;

    private PlayerInteractionController interactionController;
    private ConfirmableInteraction confirmableInteraction;
    private SimpleConfirmableInteraction simpleConfirmableInteraction;
    private InventoryUIController inventoryUIController;
    private DialogService dialogService;
    private PlayerWeaponController weaponController;

    private void Awake()
    {
        if (inputContextManager == null)
        {
            inputContextManager = FindFirstObjectByType<InputContextManager>();
        }
        
        interactionController = GetComponent<PlayerInteractionController>();
        confirmableInteraction = GetComponent<ConfirmableInteraction>();
        simpleConfirmableInteraction = GetComponent<SimpleConfirmableInteraction>();
        inventoryUIController = GetComponent<InventoryUIController>();
        dialogService = FindFirstObjectByType<DialogService>();
        weaponController = GetComponent<PlayerWeaponController>();
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        ProcessQueuedInputs();
    }
    
    private void ProcessQueuedInputs()
    {
        if (fireInputQueued)
        {
            FireInput = true;
            fireInputQueued = false;
        }
        
        if (reloadInputQueued)
        {
            ReloadInput = true;
            reloadInputQueued = false;
        }
    }
    
    private bool IsDialogOpen()
    {
        return inputContextManager != null && 
               inputContextManager.IsInContext(InputContext.Dialog);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        
        if (IsDialogOpen())
        {
            if (dialogService != null && context.performed)
            {
                dialogService.OnNavigate(input.x);
            }
            
            RamMovementInput = Vector2.zero;
            NormInputX = 0;
            NormInputY = 0;
            return;
        }
        
        RamMovementInput = input;
        if(Mathf.Abs(RamMovementInput.x) > 0.5f)
        {
        NormInputX = (int)(RamMovementInput * Vector2.right).normalized.x;
        }else
        {
            NormInputX = 0;
        }
        if (Mathf.Abs(RamMovementInput.y) > 0.5f)
        {
            NormInputY = (int)(RamMovementInput * Vector2.up).normalized.y;
        }
        else
        {
            NormInputY= 0;
        }

    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
            return;

        if (context.started)
        {
            JumpInput = true;
            jumpInputStartTime = Time.time;
        }

    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
            return;
            
        if (context.started)
            GrabInput = true;

        if(context.canceled)
            GrabInput = false;

    }

    public void OnRunInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
            return;
            
        if (context.started)
            RunInput = true;

        if (context.canceled)
            RunInput = false;
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsDialogOpen())
            {
                if (dialogService != null)
                {
                    dialogService.OnConfirmInput();
                    Debug.Log("<color=cyan>[INPUT] E key sent to dialog for confirmation</color>");
                }
                return;
            }
            
            if (inventoryUIController != null && inventoryUIController.IsOpen)
            {
                Debug.Log("<color=yellow>[INPUT] Cannot interact while inventory is open</color>");
                return;
            }
            
            if (simpleConfirmableInteraction != null)
            {
                simpleConfirmableInteraction.RequestInteraction();
            }
            else if (confirmableInteraction != null)
            {
                confirmableInteraction.RequestInteraction();
            }
            else if (interactionController != null)
            {
                interactionController.TryInteract();
            }
        }
    }

    public void OnInventoryToggleInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsDialogOpen())
            {
                if (dialogService != null)
                {
                    dialogService.OnCancelInput();
                    Debug.Log("<color=yellow>[INPUT] Inventory toggle cancelled dialog</color>");
                }
                return;
            }
            
            if (inventoryUIController != null)
            {
                inventoryUIController.ToggleInventory();
            }
        }
    }

    public void OnInventoryNavigateInput(InputAction.CallbackContext context)
    {
        if (inventoryUIController == null)
            return;

        if (context.performed)
        {
            float navigation = context.ReadValue<float>();

            if (inventoryUIController.IsInContextMenu)
            {
                inventoryUIController.NavigateContextMenu(navigation);
            }
            else if (inventoryUIController.IsOpen)
            {
                inventoryUIController.NavigateInventory(navigation);
            }
        }
    }

    public void OnInventoryInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryUIController != null && inventoryUIController.IsOpen)
        {
            inventoryUIController.InteractWithCurrentItem();
        }
    }

    public void OnInventoryCancelInput(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryUIController != null)
        {
            inventoryUIController.CancelCurrentAction();
        }
    }
    
    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
        {
            FireInput = false;
            fireInputQueued = false;
            return;
        }
        
        if (inventoryUIController != null && inventoryUIController.IsOpen)
        {
            FireInput = false;
            fireInputQueued = false;
            return;
        }
        
        if (context.started || context.performed)
        {
            fireInputQueued = true;
        }
    }
    
    public void OnReloadInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
        {
            ReloadInput = false;
            reloadInputQueued = false;
            return;
        }
            
        if (inventoryUIController != null && inventoryUIController.IsOpen)
        {
            ReloadInput = false;
            reloadInputQueued = false;
            return;
        }
        
        if (context.started || context.performed)
        {
            reloadInputQueued = true;
        }
    }
    
    public void OnWeaponSwapInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
            return;
            
        if (inventoryUIController != null && inventoryUIController.IsOpen)
            return;
        
        if (context.performed && weaponController != null)
        {
            weaponController.SwapWeapons();
        }
    }
    
    public void OnAimInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
        {
            AimInput = false;
            return;
        }
        
        if (inventoryUIController != null && inventoryUIController.IsOpen)
        {
            AimInput = false;
            return;
        }
        
        if (context.started)
        {
            AimInput = true;
        }
        
        if (context.canceled)
        {
            AimInput = false;
        }
    }

    public void JumpEnded() => JumpInput = false;
    
    public void FireEnded() 
    {
        FireInput = false;
    }
    
    public void ReloadEnded()
    {
        ReloadInput = false;
    }

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }


}
