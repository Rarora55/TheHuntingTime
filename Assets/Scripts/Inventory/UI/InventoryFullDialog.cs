using UnityEngine;
using TheHunt.UI;

namespace TheHunt.Inventory
{
    public class InventoryFullDialog : MonoBehaviour
    {
        [Header("Dialog Settings")]
        [SerializeField] private string dialogTitle = "Inventario Lleno";
        [SerializeField] [TextArea(3, 5)] private string dialogMessage = "Tu inventario está lleno. No puedes recoger más objetos.\n\nConsidera usar o soltar algunos items.";
        
        [Header("Auto-Find Dependencies")]
        [SerializeField] private bool autoFindInventorySystem = true;
        
        [Header("Manual References (Optional)")]
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private DialogService dialogService;
        
        void Awake()
        {
            if (autoFindInventorySystem && inventorySystem == null)
            {
                inventorySystem = FindFirstObjectByType<InventorySystem>();
            }
                
            if (dialogService == null)
            {
                dialogService = FindFirstObjectByType<DialogService>();
            }
                
            if (inventorySystem == null)
            {
                Debug.LogError("[INVENTORY FULL DIALOG] InventorySystem not found! This component requires an InventorySystem in the scene.");
            }
            
            if (dialogService == null)
            {
                Debug.LogError("[INVENTORY FULL DIALOG] DialogService not found! This component requires a DialogService in the scene.");
            }
        }
        
        void OnEnable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnInventoryFull += ShowFullInventoryDialog;
            }
            else
            {
                Debug.LogWarning("[INVENTORY FULL DIALOG] Cannot subscribe - InventorySystem is null!");
            }
        }
        
        void OnDisable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnInventoryFull -= ShowFullInventoryDialog;
            }
        }
        
        void ShowFullInventoryDialog()
        {
            Debug.Log("<color=red>[INVENTORY FULL DIALOG] ========== INVENTORY FULL EVENT TRIGGERED ==========</color>");
            
            if (dialogService == null)
            {
                Debug.LogWarning("[INVENTORY FULL DIALOG] DialogService not found!");
                return;
            }
            
            Debug.Log($"<color=cyan>[INVENTORY FULL DIALOG] Showing dialog: '{dialogTitle}'</color>");
            dialogService.ShowInfo(dialogTitle, dialogMessage);
            Debug.Log("<color=green>[INVENTORY FULL DIALOG] Dialog shown successfully!</color>");
        }
    }
}
