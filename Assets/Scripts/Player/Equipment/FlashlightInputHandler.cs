using UnityEngine;
using UnityEngine.InputSystem;

namespace TheHunt.Equipment
{
    public class FlashlightInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SecondaryEquipmentController equipmentController;
        
        [Header("Input Settings")]
        [SerializeField] private InputActionReference toggleFlashlightAction;
        
        void Awake()
        {
            if (equipmentController == null)
            {
                equipmentController = GetComponent<SecondaryEquipmentController>();
            }
        }
        
        void OnEnable()
        {
            if (toggleFlashlightAction != null)
            {
                toggleFlashlightAction.action.Enable();
                toggleFlashlightAction.action.performed += OnToggleFlashlight;
            }
        }
        
        void OnDisable()
        {
            if (toggleFlashlightAction != null)
            {
                toggleFlashlightAction.action.performed -= OnToggleFlashlight;
                toggleFlashlightAction.action.Disable();
            }
        }
        
        void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current.lKey.wasPressedThisFrame)
            {
                Debug.Log("<color=cyan>[FLASHLIGHT INPUT] L key pressed - Toggling flashlight</color>");
                ToggleFlashlightManual();
            }
        }
        
        void OnToggleFlashlight(InputAction.CallbackContext context)
        {
            Debug.Log("<color=cyan>[FLASHLIGHT INPUT] Input action performed</color>");
            if (equipmentController != null)
            {
                equipmentController.ToggleFlashlight();
            }
        }
        
        public void ToggleFlashlightManual()
        {
            Debug.Log($"<color=cyan>[FLASHLIGHT INPUT] Manual toggle - equipmentController: {equipmentController != null}</color>");
            if (equipmentController != null)
            {
                equipmentController.ToggleFlashlight();
            }
            else
            {
                Debug.LogError("<color=red>[FLASHLIGHT INPUT] equipmentController is NULL!</color>");
            }
        }
    }
}
