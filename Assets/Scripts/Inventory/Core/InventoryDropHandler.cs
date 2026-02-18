using UnityEngine;

namespace TheHunt.Inventory
{
    public class InventoryDropHandler
    {
        private readonly Transform dropperTransform;
        private Transform dropPoint;
        
        private const float HORIZONTAL_OFFSET = 1.5f;
        private const float RAYCAST_DISTANCE = 50f;

        public InventoryDropHandler(Transform dropperTransform)
        {
            this.dropperTransform = dropperTransform;
            
            // Buscar el punto de drop como hijo del dropper
            if (dropperTransform != null)
            {
                dropPoint = dropperTransform.Find("Drop");
                
                if (dropPoint != null)
                {
                    Debug.Log($"<color=green>[DROP HANDLER] Found Drop point at local position: {dropPoint.localPosition}</color>");
                }
                else
                {
                    Debug.LogWarning("<color=yellow>[DROP HANDLER] No 'Drop' child found. Using default offset.</color>");
                }
            }
        }

        public void DropItem(ItemData itemData, ItemInstance itemInstance = null, Vector3? customPosition = null)
        {
            if (itemData == null)
            {
                Debug.LogWarning("[DROP HANDLER] Cannot drop null item");
                return;
            }

            // Obtener escala original de metadata si existe
            Vector3 originalScale = Vector3.one;
            if (itemInstance != null && itemInstance.metadata != null && itemInstance.metadata.ContainsKey("originalScale"))
            {
                originalScale = (Vector3)itemInstance.metadata["originalScale"];
                Debug.Log($"<color=cyan>[DROP HANDLER] Using saved scale: {originalScale}</color>");
            }

            GameObject prefabToSpawn = itemData.PickupPrefab;
            
            // Si no tiene pickup prefab asignado, crear uno dinámicamente
            if (prefabToSpawn == null)
            {
                Debug.LogWarning($"<color=yellow>[DROP HANDLER] {itemData.ItemName} has no PickupPrefab. Creating dynamic pickup...</color>");
                DropItemDynamically(itemData, originalScale, customPosition);
                return;
            }

            Vector3 dropPosition = customPosition ?? CalculateDropPosition(prefabToSpawn);
            GameObject droppedObject = Object.Instantiate(prefabToSpawn, dropPosition, Quaternion.identity);
            
            // Aplicar escala original
            droppedObject.transform.localScale = originalScale;
            
            Debug.Log($"<color=green>[DROP HANDLER] Spawned {itemData.ItemName} at {dropPosition} with scale {originalScale}</color>");
        }
        
        private void DropItemDynamically(ItemData itemData, Vector3 originalScale, Vector3? customPosition = null)
        {
            // Calcular posición de drop
            Vector3 dropPosition = customPosition ?? CalculateDropPositionSimple();
            
            // Crear GameObject
            GameObject pickupObject = new GameObject($"{itemData.ItemName} (Pickup)");
            pickupObject.transform.position = dropPosition;
            pickupObject.transform.localScale = originalScale;
            pickupObject.layer = LayerMask.NameToLayer("Interactable");
            pickupObject.tag = "Interactable";
            
            // Añadir SpriteRenderer si el item tiene icono
            if (itemData.ItemIcon != null)
            {
                SpriteRenderer spriteRenderer = pickupObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = itemData.ItemIcon;
                spriteRenderer.sortingOrder = 1;
            }
            
            // Añadir Collider2D (trigger)
            CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
            
            // Añadir PickupItem component
            PickupItem pickupComponent = pickupObject.AddComponent<PickupItem>();
            
            // Usar reflection para asignar el itemData privado
            var itemDataField = typeof(PickupItem).GetField("itemData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (itemDataField != null)
            {
                itemDataField.SetValue(pickupComponent, itemData);
            }
            
            var promptField = typeof(PickupItem).GetField("interactionPrompt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (promptField != null)
            {
                promptField.SetValue(pickupComponent, $"Pick up");
            }
            
            Debug.Log($"<color=green>[DROP HANDLER] Created dynamic pickup for {itemData.ItemName} at {dropPosition} with scale {originalScale}</color>");
        }
        
        private Vector3 CalculateDropPositionSimple()
        {
            // Si hay un punto de drop definido, usarlo
            if (dropPoint != null)
            {
                Vector3 worldDropPosition = dropPoint.position;
                Debug.Log($"<color=green>[DROP HANDLER] Using Drop point at world position: {worldDropPosition}</color>");
                return worldDropPosition;
            }
            
            // Fallback: usar offset horizontal
            Vector3 horizontalOffset = dropperTransform.right * HORIZONTAL_OFFSET;
            Vector3 startPosition = dropperTransform.position + horizontalOffset;
            
            int groundLayer = LayerMask.GetMask("Ground");
            RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, RAYCAST_DISTANCE, groundLayer);
            
            if (hit.collider != null)
            {
                return new Vector3(hit.point.x, hit.point.y + 0.5f, 0f);
            }
            
            return startPosition;
        }

        public Vector3 CalculateDropPosition(GameObject prefab)
        {
            // Si hay un punto de drop definido, usarlo directamente
            if (dropPoint != null)
            {
                Vector3 worldDropPosition = dropPoint.position;
                Debug.Log($"<color=green>[DROP HANDLER] Using Drop point at world position: {worldDropPosition}</color>");
                return worldDropPosition;
            }
            
            // Fallback: usar cálculo con raycast
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
