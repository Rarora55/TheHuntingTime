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
        
        void OnToggleFlashlight(InputAction.CallbackContext context)
        {
            if (equipmentController != null)
            {
                equipmentController.ToggleFlashlight();
            }
        }
        
        public void ToggleFlashlightManual()
        {
            if (equipmentController != null)
            {
                equipmentController.ToggleFlashlight();
            }
        }
    }
}
