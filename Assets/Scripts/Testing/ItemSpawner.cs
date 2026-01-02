using UnityEngine;

namespace TheHunt.Testing
{
    public class ItemSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private KeyCode spawnKey = KeyCode.R;
        [SerializeField] private bool destroyPrevious = true;

        [Header("Info")]
        [SerializeField] private string itemName = "Item";
        [SerializeField] private bool showGizmo = true;

        private GameObject currentItem;

        private void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(spawnKey))
            {
                SpawnItem();
            }
        }

        public void SpawnItem()
        {
            if (itemPrefab == null)
            {
                Debug.LogWarning($"<color=yellow>[ITEM SPAWNER] No prefab assigned to {gameObject.name}</color>");
                return;
            }

            if (destroyPrevious && currentItem != null)
            {
                Destroy(currentItem);
            }

            currentItem = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"<color=green>[ITEM SPAWNER] Spawned {itemName} at {spawnPoint.position}</color>");
        }

        public void ClearItem()
        {
            if (currentItem != null)
            {
                Destroy(currentItem);
                currentItem = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmo) return;

            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, 0.3f);
            Gizmos.DrawLine(position + Vector3.up * 0.5f, position - Vector3.up * 0.5f);
            Gizmos.DrawLine(position + Vector3.right * 0.5f, position - Vector3.right * 0.5f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(position + Vector3.up * 0.7f, $"Spawn: {itemName}\n[{spawnKey}]");
#endif
        }
    }
}
