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
        [SerializeField] private GameObject dialogPrefab;
        
        private PrefabConfirmationDialog prefabDialogInstance;
        
        public bool IsDialogOpen
        {
            get
            {
                if (usePrefab)
                    return prefabDialogInstance != null && prefabDialogInstance.IsOpen;
                else
                    return simpleDialog != null && simpleDialog.IsOpen;
            }
        }
        
        void Awake()
        {
            Debug.Log($"<color=yellow>[DIALOG SERVICE] ========== AWAKE ==========</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] usePrefab: {usePrefab}</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] dialogPrefab: {(dialogPrefab != null ? dialogPrefab.name : "NULL")}</color>");
            Debug.Log($"<color=yellow>[DIALOG SERVICE] simpleDialog: {(simpleDialog != null ? "assigned" : "NULL")}</color>");
            
            if (usePrefab)
            {
                Debug.Log("<color=cyan>[DIALOG SERVICE] Using PREFAB mode</color>");
                
                if (dialogPrefab != null && prefabDialogInstance == null)
                {
                    Debug.Log("<color=cyan>[DIALOG SERVICE] Instantiating dialog prefab...</color>");
                    
                    GameObject instance = Instantiate(dialogPrefab, transform);
                    Debug.Log($"<color=cyan>[DIALOG SERVICE] Instance created: {instance.name}</color>");
                    
                    prefabDialogInstance = instance.GetComponent<PrefabConfirmationDialog>();
                    
                    if (prefabDialogInstance == null)
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
                else if (dialogPrefab == null)
                {
                    Debug.LogError("[DIALOG SERVICE] Dialog Prefab is not assigned!");
                }
                else if (prefabDialogInstance != null)
                {
                    Debug.Log("<color=yellow>[DIALOG SERVICE] Prefab instance already exists</color>");
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
                if (prefabDialogInstance == null)
                {
                    Debug.LogError("[DIALOG SERVICE] Prefab dialog instance is null!");
                    return;
                }
                
                prefabDialogInstance.Show(title, description, onConfirm, onCancel);
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
        
        public void HideDialog()
        {
            if (usePrefab && prefabDialogInstance != null)
            {
                prefabDialogInstance.Hide();
            }
            else if (!usePrefab && simpleDialog != null)
            {
                simpleDialog.Hide();
            }
        }
        
        public void OnNavigate(float direction)
        {
            if (usePrefab && prefabDialogInstance != null && prefabDialogInstance.IsOpen)
            {
                prefabDialogInstance.OnNavigate(direction);
            }
            else if (!usePrefab && simpleDialog != null && simpleDialog.IsOpen)
            {
                simpleDialog.OnNavigate(direction);
            }
        }
        
        public void OnConfirmInput()
        {
            if (usePrefab && prefabDialogInstance != null && prefabDialogInstance.IsOpen)
            {
                prefabDialogInstance.OnConfirmInput();
            }
            else if (!usePrefab && simpleDialog != null && simpleDialog.IsOpen)
            {
                simpleDialog.OnConfirmInput();
            }
        }
        
        public void OnCancelInput()
        {
            if (usePrefab && prefabDialogInstance != null && prefabDialogInstance.IsOpen)
            {
                prefabDialogInstance.OnCancelInput();
            }
            else if (!usePrefab && simpleDialog != null && simpleDialog.IsOpen)
            {
                simpleDialog.OnCancelInput();
            }
        }
    }
}
