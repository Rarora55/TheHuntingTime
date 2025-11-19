using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldItemPickup : MonoBehaviour, IPickupable
{
    [SerializeField] private ConsumableItemData itemData;
    [SerializeField] private int quantity = 1;
    
    [Header("Visuals")]
    [SerializeField] private GameObject pickupVFX;
    [SerializeField] private AudioClip pickupSound;
    
    public IItem ItemData => itemData;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickedUp(other.gameObject);
        }
    }
    
    public void OnPickedUp(GameObject picker)
    {
        Debug.Log($"<color=cyan>[PICKUP] {picker.name} picked up {itemData.ItemName} x{quantity}</color>");
        
        PlayFeedback();
        
        Destroy(gameObject);
    }
    
    void PlayFeedback()
    {
        if (pickupVFX != null)
        {
            Instantiate(pickupVFX, transform.position, Quaternion.identity);
        }
        
        if (pickupSound != null)
        {
        }
    }
}
