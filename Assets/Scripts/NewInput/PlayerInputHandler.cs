using UnityEngine;
using UnityEngine.InputSystem;
using TheHunt.Interaction;
using TheHunt.Inventory;
using TheHunt.UI;
using TheHunt.Input;
using TheHunt.Equipment;

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
    public bool PushPullInput { get; private set; }

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
    private SecondaryEquipmentController secondaryEquipmentController;
    private PlayerInput playerInput;
    private InputAction weaponSlotSelectionAction;

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
        secondaryEquipmentController = GetComponent<SecondaryEquipmentController>();
        playerInput = GetComponent<PlayerInput>();
        
        TrySetupWeaponSlotSelectionAction();
    }
    
    private void TrySetupWeaponSlotSelectionAction()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            weaponSlotSelectionAction = playerInput.actions.FindAction("WeaponSlotSelection");
            
            if (weaponSlotSelectionAction != null)
            {
                weaponSlotSelectionAction.performed += OnWeaponSlotSelectionPerformed;
                Debug.Log("<color=green>[INPUT HANDLER] WeaponSlotSelection action subscribed successfully!</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[INPUT HANDLER] WeaponSlotSelection action not found in input actions!</color>");
            }
        }
    }
    
    private void OnDestroy()
    {
        if (weaponSlotSelectionAction != null)
        {
            weaponSlotSelectionAction.performed -= OnWeaponSlotSelectionPerformed;
        }
    }
    
    private void OnWeaponSlotSelectionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"<color=magenta>[INPUT HANDLER] WeaponSlotSelection performed - controller: {inventoryUIController != null}, isOpen: {inventoryUIController?.IsOpen}</color>");
        
        if (inventoryUIController != null && inventoryUIController.IsOpen)
        {
            inventoryUIController.ToggleWeaponSlotSelectionMode();
        }
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
        {
            GrabInput = true;
            Debug.Log("<color=green>[INPUT] Grab Input STARTED - GrabInput = TRUE</color>");
        }

        if(context.canceled)
        {
            GrabInput = false;
            Debug.Log("<color=red>[INPUT] Grab Input CANCELED - GrabInput = FALSE</color>");
        }

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
                }
                return;
            }
            
            if (inventoryUIController != null && inventoryUIController.IsOpen)
            {
                Debug.Log($"<color=yellow>[INPUT HANDLER] Inventory is open, blocking interaction</color>");
                return;
            }
            
            Debug.Log($"<color=cyan>[INPUT HANDLER] OnInteractInput - simpleConfirmable: {simpleConfirmableInteraction != null}, confirmable: {confirmableInteraction != null}, controller: {interactionController != null}</color>");
            
            if (simpleConfirmableInteraction != null)
            {
                Debug.Log($"<color=magenta>[INPUT HANDLER] Calling simpleConfirmableInteraction.RequestInteraction()</color>");
                simpleConfirmableInteraction.RequestInteraction();
            }
            else if (confirmableInteraction != null)
            {
                Debug.Log($"<color=magenta>[INPUT HANDLER] Calling confirmableInteraction.RequestInteraction()</color>");
                confirmableInteraction.RequestInteraction();
            }
            else if (interactionController != null)
            {
                Debug.Log($"<color=green>[INPUT HANDLER] Calling interactionController.TryInteract()</color>");
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
    
    public void OnWeaponSlotSelectionInput(InputAction.CallbackContext context)
    {
        Debug.Log($"<color=magenta>[INPUT HANDLER] OnWeaponSlotSelectionInput - performed: {context.performed}, controller: {inventoryUIController != null}, isOpen: {inventoryUIController?.IsOpen}</color>");
        
        if (context.performed && inventoryUIController != null && inventoryUIController.IsOpen)
        {
            inventoryUIController.ToggleWeaponSlotSelectionMode();
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
    
    public void OnFlashlightInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
            return;
            
        if (inventoryUIController != null && inventoryUIController.IsOpen)
            return;
        
        if (context.performed && secondaryEquipmentController != null)
        {
            secondaryEquipmentController.ToggleFlashlight();
        }
    }
    
    public void OnPushPullInput(InputAction.CallbackContext context)
    {
        if (IsDialogOpen())
        {
            PushPullInput = false;
            return;
        }
            
        if (inventoryUIController != null && inventoryUIController.IsOpen)
        {
            PushPullInput = false;
            return;
        }
        
        if (context.started)
        {
            PushPullInput = true;
        }
        
        if (context.canceled)
        {
            PushPullInput = false;
        }
    }

    public void JumpEnded() => JumpInput = false;
    
    public void FireEnded() 
    {
        FireInput = false;
        fireInputQueued = false;
    }
    
    public void ReloadEnded()
    {
        ReloadInput = false;
        reloadInputQueued = false;
    }

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }


}
