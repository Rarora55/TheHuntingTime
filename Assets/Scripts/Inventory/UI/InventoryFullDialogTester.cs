using UnityEngine;
using TheHunt.UI;

namespace TheHunt.Inventory
{
    public class InventoryFullDialogTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private KeyCode testKey = KeyCode.F8;
        
        private DialogService dialogService;
        
        void Awake()
        {
            dialogService = FindFirstObjectByType<DialogService>();
        }
        
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(testKey))
            {
                TestInventoryFullDialog();
            }
        }
        
        void TestInventoryFullDialog()
        {
            if (dialogService == null)
            {
                Debug.LogWarning("[TEST] DialogService not found!");
                return;
            }
            
            Debug.Log("<color=magenta>[TEST] Showing Inventory Full dialog directly...</color>");
            dialogService.ShowInfo("Inventario Lleno", "Tu inventario está lleno. No puedes recoger más objetos.\n\nConsidera usar o soltar algunos items.");
            Debug.Log("<color=green>[TEST] Dialog command sent!</color>");
        }
    }
}
