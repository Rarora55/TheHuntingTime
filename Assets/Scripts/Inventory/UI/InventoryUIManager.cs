using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventoryUIManager : MonoBehaviour
    {
        [Header("Prefab Reference")]
        [SerializeField] private GameObject inventoryCanvasPrefab;
        
        [Header("Runtime References")]
        private GameObject inventoryCanvasInstance;
        private InventoryPanelUI inventoryPanel;
        private WeaponEquipmentPanel weaponPanel;
        private ItemExaminationPanel examinationPanel;
        
        public InventoryPanelUI InventoryPanel => inventoryPanel;
        public WeaponEquipmentPanel WeaponPanel => weaponPanel;
        public ItemExaminationPanel ExaminationPanel => examinationPanel;
        
        private void Awake()
        {
            if (inventoryCanvasPrefab == null)
            {
                Debug.LogError("[INVENTORY UI MANAGER] InventoryCanvasPrefab is not assigned!");
                return;
            }
            
            InstantiateInventoryUI();
        }
        
        private void InstantiateInventoryUI()
        {
            inventoryCanvasInstance = Instantiate(inventoryCanvasPrefab);
            inventoryCanvasInstance.name = "InventoryCanvas";
            
            inventoryPanel = inventoryCanvasInstance.GetComponentInChildren<InventoryPanelUI>(true);
            weaponPanel = inventoryCanvasInstance.GetComponentInChildren<WeaponEquipmentPanel>(true);
            examinationPanel = inventoryCanvasInstance.GetComponentInChildren<ItemExaminationPanel>(true);
            
            if (inventoryPanel == null)
                Debug.LogWarning("[INVENTORY UI MANAGER] InventoryPanelUI not found in prefab!");
            
            if (weaponPanel == null)
                Debug.LogWarning("[INVENTORY UI MANAGER] WeaponEquipmentPanel not found in prefab!");
            
            if (examinationPanel == null)
                Debug.LogWarning("[INVENTORY UI MANAGER] ItemExaminationPanel not found in prefab!");
            
            Debug.Log("<color=cyan>[INVENTORY UI MANAGER] Inventory UI instantiated successfully</color>");
        }
        
        private void OnDestroy()
        {
            if (inventoryCanvasInstance != null)
            {
                Destroy(inventoryCanvasInstance);
            }
        }
    }
}
