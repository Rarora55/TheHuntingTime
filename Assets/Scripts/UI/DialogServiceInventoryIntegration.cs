using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.UI
{
    [RequireComponent(typeof(DialogService))]
    public class DialogServiceInventoryIntegration : MonoBehaviour
    {
        [Header("Inventory Full Dialog Settings")]
        [SerializeField] private bool enableInventoryFullDialog = true;
        [SerializeField] private string inventoryFullTitle = "Inventario Lleno";
        [SerializeField] [TextArea(3, 5)] private string inventoryFullMessage = "Tu inventario está lleno. No puedes recoger más objetos.\n\nConsidera usar o soltar algunos items.";
        
        private DialogService dialogService;
        private InventorySystem inventorySystem;
        
        void Awake()
        {
            dialogService = GetComponent<DialogService>();
            
            if (enableInventoryFullDialog)
            {
                inventorySystem = FindFirstObjectByType<InventorySystem>();
                
                if (inventorySystem == null)
                {
                    Debug.LogWarning("[DIALOG SERVICE INTEGRATION] InventorySystem not found. Inventory full dialog will not work.");
                }
            }
        }
        
        void OnEnable()
        {
            if (enableInventoryFullDialog && inventorySystem != null)
            {
                inventorySystem.OnInventoryFull += OnInventoryFull;
            }
        }
        
        void OnDisable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnInventoryFull -= OnInventoryFull;
            }
        }
        
        void OnInventoryFull()
        {
            if (dialogService != null)
            {
                dialogService.ShowInfo(inventoryFullTitle, inventoryFullMessage);
            }
        }
    }
}
