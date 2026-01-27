using UnityEngine;

public class ManualDeathSpriteChanger : MonoBehaviour
{
    [Header("Death Sprite (Temporary Fix)")]
    [SerializeField] private Sprite deathSprite;
    
    private SpriteRenderer spriteRenderer;
    private Player player;
    private bool hasChangedSprite = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
    }
    
    void Update()
    {
        if (player == null || spriteRenderer == null)
            return;
        
        if (player.StateMachine.CurrentState is PlayerDeathState)
        {
            if (!hasChangedSprite && deathSprite != null)
            {
                spriteRenderer.sprite = deathSprite;
                hasChangedSprite = true;
                Debug.Log("<color=green>[MANUAL SPRITE] Death sprite applied as temporary fix!</color>");
            }
        }
        else
        {
            hasChangedSprite = false;
        }
    }
}
