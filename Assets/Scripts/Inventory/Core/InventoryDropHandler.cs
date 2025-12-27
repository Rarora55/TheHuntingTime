using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventoryDropHandler
    {
        private readonly Transform dropperTransform;
        
        private const float HORIZONTAL_OFFSET = 1.5f;
        private const float RAYCAST_DISTANCE = 50f;

        public InventoryDropHandler(Transform dropperTransform)
        {
            this.dropperTransform = dropperTransform;
        }

        public void DropItem(ItemData itemData, Vector3? customPosition = null)
        {
            if (itemData == null)
            {
                Debug.LogWarning("[DROP HANDLER] Cannot drop null item");
                return;
            }

            if (itemData.PickupPrefab == null)
            {
                Debug.LogWarning($"<color=yellow>[DROP HANDLER] No pickup prefab assigned for {itemData.ItemName}</color>");
                return;
            }

            Vector3 dropPosition = customPosition ?? CalculateDropPosition(itemData.PickupPrefab);
            GameObject droppedObject = Object.Instantiate(itemData.PickupPrefab, dropPosition, Quaternion.identity);
            
            Debug.Log($"<color=green>[DROP HANDLER] Spawned {itemData.ItemName} at {dropPosition}</color>");
        }

        public Vector3 CalculateDropPosition(GameObject prefab)
        {
            Vector3 horizontalOffset = dropperTransform.right * HORIZONTAL_OFFSET;
            Vector3 startPosition = dropperTransform.position + horizontalOffset;
            
            int groundLayer = LayerMask.GetMask("Ground");
            RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, RAYCAST_DISTANCE, groundLayer);
            
            Vector3 groundContactPoint;
            if (hit.collider != null)
            {
                groundContactPoint = hit.point;
                Debug.Log($"<color=cyan>[DROP HANDLER] Ground found at {groundContactPoint}</color>");
            }
            else
            {
                groundContactPoint = startPosition;
                Debug.LogWarning($"<color=yellow>[DROP HANDLER] No ground found, using horizontal position</color>");
            }
            
            Vector3 detectionOffset = GetDetectionPointOffset(prefab);
            
            Vector3 finalPosition = new Vector3(
                groundContactPoint.x,
                groundContactPoint.y - detectionOffset.y,
                groundContactPoint.z
            );
            
            Debug.Log($"<color=green>[DROP HANDLER] Detection offset: {detectionOffset}, Final position: {finalPosition}</color>");
            return finalPosition;
        }
        
        private Vector3 GetDetectionPointOffset(GameObject prefab)
        {
            Transform detectionPoint = FindDetectionPointInPrefab(prefab);
            
            if (detectionPoint != null)
            {
                Vector3 localPos = detectionPoint.localPosition;
                Vector3 scale = prefab.transform.localScale;
                
                Vector3 worldOffset = new Vector3(
                    localPos.x * scale.x,
                    localPos.y * scale.y,
                    localPos.z * scale.z
                );
                
                Debug.Log($"<color=cyan>[DROP HANDLER] Found detection point: local={localPos}, scale={scale}, worldOffset={worldOffset}</color>");
                return worldOffset;
            }
            
            Debug.LogWarning($"<color=yellow>[DROP HANDLER] No 'detectionGround' found in prefab, using zero offset</color>");
            return Vector3.zero;
        }
        
        private Transform FindDetectionPointInPrefab(GameObject prefab)
        {
            Transform root = prefab.transform;
            
            Transform detectionPoint = root.Find("detectionGround");
            if (detectionPoint != null)
                return detectionPoint;
            
            detectionPoint = root.Find("detectioGround");
            if (detectionPoint != null)
                return detectionPoint;
            
            detectionPoint = root.Find("DetectionGround");
            if (detectionPoint != null)
                return detectionPoint;
            
            return null;
        }
    }
}
