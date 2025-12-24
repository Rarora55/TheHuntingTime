using UnityEngine;
using System;

namespace TheHunt.UI
{
    public class DialogService : MonoBehaviour
    {
        [Header("Dialog Type")]
        [SerializeField] private bool usePrefab = false;
        
        [Header("Code-Based Dialog (Legacy)")]
        [SerializeField] private SimpleConfirmationDialog simpleDialog;
        
        [Header("Prefab-Based Dialog")]
        [SerializeField] private GameObject confirmationDialogPrefab;
        [SerializeField] private GameObject infoDialogPrefab;
        
        private PrefabConfirmationDialog confirmationDialogInstance;
        private InfoDialog infoDialogInstance;
        
        public bool IsDialogOpen
        {
            get
            {
                if (usePrefab)
                {
                    bool confirmationOpen = confirmationDialogInstance != null && confirmationDialogInstance.IsOpen;
                    bool infoOpen = infoDialogInstance != null && infoDialogInstance.IsOpen;
                    return confirmationOpen || infoOpen;
                }
                else
                {
                    return simpleDialog != null && simpleDialog.IsOpen;
                }
            }
        }
        
        void Awake()
        {
            Debug.Log($"<color=yellow>[DIALOG SERVICE] ========== AWAKE ==========</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] usePrefab: {usePrefab}</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] confirmationDialogPrefab: {(confirmationDialogPrefab != null ? confirmationDialogPrefab.name : "NULL")}</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] infoDialogPrefab: {(infoDialogPrefab != null ? infoDialogPrefab.name : "NULL")}</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] simpleDialog: {(simpleDialog != null ? "assigned" : "NULL")}</color>");
            
            if (usePrefab)
            {
                Debug.Log("<color=cyan>[DIALOG SERVICE] Using PREFAB mode</color>");
                
                if (confirmationDialogPrefab != null && confirmationDialogInstance == null)
                {
                    Debug.Log("<color=cyan>[DIALOG SERVICE] Instantiating confirmation dialog prefab...</color>");
                    
                    GameObject instance = Instantiate(confirmationDialogPrefab, transform);
                    Debug.Log($"<color=cyan>[DIALOG SERVICE] Instance created: {instance.name}</color>");
                    
                    confirmationDialogInstance = instance.GetComponent<PrefabConfirmationDialog>();
                    
                    if (confirmationDialogInstance == null)
                    {
                        Debug.LogError("[DIALOG SERVICE] Prefab does not have PrefabConfirmationDialog component!");
                        
                        Component[] components = instance.GetComponents<Component>();
                        string componentList = "";
                        foreach (var comp in components)
                        {
                            componentList += comp.GetType().Name + ", ";
                        }
                        Debug.LogError($"[DIALOG SERVICE] Components on instance: {componentList}");
                    }
                    else
                    {
                        Debug.Log("<color=green>[DIALOG SERVICE] Connected to PrefabConfirmationDialog ✓</color>");
                    }
                }
                else if (confirmationDialogPrefab == null)
                {
                    Debug.LogWarning("[DIALOG SERVICE] Confirmation Dialog Prefab is not assigned!");
                }
                
                if (infoDialogPrefab != null && infoDialogInstance == null)
                {
                    Debug.Log("<color=cyan>[DIALOG SERVICE] Instantiating info dialog prefab...</color>");
                    
                    GameObject instance = Instantiate(infoDialogPrefab, transform);
                    Debug.Log($"<color=cyan>[DIALOG SERVICE] Instance created: {instance.name}</color>");
                    
                    infoDialogInstance = instance.GetComponent<InfoDialog>();
                    
                    if (infoDialogInstance == null)
                    {
                        Debug.LogError("[DIALOG SERVICE] Prefab does not have InfoDialog component!");
                    }
                    else
                    {
                        Debug.Log("<color=green>[DIALOG SERVICE] Connected to InfoDialog ✓</color>");
                    }
                }
                else if (infoDialogPrefab == null)
                {
                    Debug.LogWarning("[DIALOG SERVICE] Info Dialog Prefab is not assigned!");
                }
            }
            else
            {
                Debug.Log("<color=cyan>[DIALOG SERVICE] Using CODE-BASED mode</color>");
                
                if (simpleDialog == null)
                {
                    simpleDialog = GetComponent<SimpleConfirmationDialog>();
                    
                    if (simpleDialog == null)
                    {
                        simpleDialog = GetComponentInChildren<SimpleConfirmationDialog>();
                    }
                    
                    if (simpleDialog == null)
                    {
                        Debug.LogError("[DIALOG SERVICE] No SimpleConfirmationDialog found!");
                    }
                    else
                    {
                        Debug.Log("<color=green>[DIALOG SERVICE] Connected to SimpleConfirmationDialog ✓</color>");
                    }
                }
            }
            
            Debug.Log($"<color=yellow>[DIALOG SERVICE] ===========================</color>");
        }
        
        public void ShowConfirmation(string title, string description, Action onConfirm, Action onCancel = null)
        {
            if (usePrefab)
            {
                if (confirmationDialogInstance == null)
                {
                    Debug.LogError("[DIALOG SERVICE] Confirmation dialog instance is null!");
                    return;
                }
                
                confirmationDialogInstance.Show(title, description, onConfirm, onCancel);
            }
            else
            {
                if (simpleDialog == null)
                {
                    Debug.LogError("[DIALOG SERVICE] Simple dialog instance is null!");
                    return;
                }
                
                simpleDialog.Show(title, description, onConfirm, onCancel);
            }
        }
        
        public void ShowInfo(string title, string message, Action onClose = null)
        {
            if (usePrefab)
            {
                if (infoDialogInstance == null)
                {
                    Debug.LogError("[DIALOG SERVICE] Info dialog instance is null!");
                    return;
                }
                
                infoDialogInstance.Show(title, message, onClose);
            }
            else
            {
                Debug.LogWarning("[DIALOG SERVICE] ShowInfo() only works in prefab mode. Please set usePrefab = true.");
            }
        }
        
        public void HideDialog()
        {
            if (usePrefab)
            {
                if (confirmationDialogInstance != null)
                    confirmationDialogInstance.Hide();
                
                if (infoDialogInstance != null)
                    infoDialogInstance.Hide();
            }
            else if (!usePrefab && simpleDialog != null)
            {
                simpleDialog.Hide();
            }
        }
        
        public void OnNavigate(float direction)
        {
            if (usePrefab)
            {
                if (confirmationDialogInstance != null && confirmationDialogInstance.IsOpen)
                {
                    confirmationDialogInstance.OnNavigate(direction);
                }
                else if (infoDialogInstance != null && infoDialogInstance.IsOpen)
                {
                    // Info dialogs don't have navigation
                }
            }
            else if (!usePrefab && simpleDialog != null && simpleDialog.IsOpen)
            {
                simpleDialog.OnNavigate(direction);
            }
        }
        
        public void OnConfirmInput()
        {
            if (usePrefab)
            {
                if (confirmationDialogInstance != null && confirmationDialogInstance.IsOpen)
                {
                    confirmationDialogInstance.OnConfirmInput();
                }
                else if (infoDialogInstance != null && infoDialogInstance.IsOpen)
                {
                    infoDialogInstance.OnConfirmInput();
                }
            }
            else if (!usePrefab && simpleDialog != null && simpleDialog.IsOpen)
            {
                simpleDialog.OnConfirmInput();
            }
        }
        
        public void OnCancelInput()
        {
            if (usePrefab)
            {
                if (confirmationDialogInstance != null && confirmationDialogInstance.IsOpen)
                {
                    confirmationDialogInstance.OnCancelInput();
                }
                else if (infoDialogInstance != null && infoDialogInstance.IsOpen)
                {
                    infoDialogInstance.OnCancelInput();
                }
            }
            else if (!usePrefab && simpleDialog != null && simpleDialog.IsOpen)
            {
                simpleDialog.OnCancelInput();
            }
        }
    }
}
