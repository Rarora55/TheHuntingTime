using UnityEngine;
using TheHunt.Inventory;
using TheHunt.Input;
using TheHunt.UI;
using TheHunt.Interaction;

namespace TheHunt.Player
{
    public class PlayerComponentOrganizer : MonoBehaviour
    {
        [Header("════════════ CORE PLAYER ════════════")]
        [SerializeField] private global::Player playerStateMachine;
        [SerializeField] private PlayerInputHandler inputHandler;
        
        [Header("════════════ PHYSICS & COLLISION ════════════")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CapsuleCollider2D capsuleCollider;
        [SerializeField] private PlayerPhysicsController physicsController;
        [SerializeField] private PlayerCollisionController collisionController;
        
        [Header("════════════ ANIMATION & VISUAL ════════════")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerAnimationController animationController;
        [SerializeField] private PlayerOrientationController orientationController;
        
        [Header("════════════ INVENTORY SYSTEM ════════════")]
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private InventoryUIController inventoryUIController;
        [SerializeField] private WeaponInventoryManager weaponManager;
        [SerializeField] private AmmoInventoryManager ammoManager;
        
        [Header("════════════ HEALTH & STAMINA ════════════")]
        [SerializeField] private PlayerHealthIntegration healthIntegration;
        
        [Header("════════════ INTERACTION ════════════")]
        [SerializeField] private PlayerInteractionController interactionController;
        
        [Header("════════════ UI & INPUT CONTEXT ════════════")]
        [SerializeField] private InputContextManager inputContextManager;
        [SerializeField] private DialogService dialogService;
        
        [Header("════════════ DEBUG TOOLS ════════════")]
        [SerializeField] private InventoryDebugger inventoryDebugger;
        [SerializeField] private PlayerCoreDebugger coreDebugger;
        
        void Awake()
        {
            CacheComponents();
        }
        
        private void CacheComponents()
        {
            if (playerStateMachine == null)
                playerStateMachine = GetComponent<global::Player>();
            
            if (inputHandler == null)
                inputHandler = GetComponent<PlayerInputHandler>();
            
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            
            if (capsuleCollider == null)
                capsuleCollider = GetComponent<CapsuleCollider2D>();
            
            if (physicsController == null)
                physicsController = GetComponent<PlayerPhysicsController>();
            
            if (collisionController == null)
                collisionController = GetComponent<PlayerCollisionController>();
            
            if (animator == null)
                animator = GetComponent<Animator>();
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (animationController == null)
                animationController = GetComponent<PlayerAnimationController>();
            
            if (orientationController == null)
                orientationController = GetComponent<PlayerOrientationController>();
            
            if (inventorySystem == null)
                inventorySystem = GetComponent<InventorySystem>();
            
            if (inventoryUIController == null)
                inventoryUIController = GetComponent<InventoryUIController>();
            
            if (weaponManager == null)
                weaponManager = GetComponent<WeaponInventoryManager>();
            
            if (ammoManager == null)
                ammoManager = GetComponent<AmmoInventoryManager>();
            
            if (healthIntegration == null)
                healthIntegration = GetComponent<PlayerHealthIntegration>();
            
            if (interactionController == null)
                interactionController = GetComponent<PlayerInteractionController>();
            
            if (inputContextManager == null)
                inputContextManager = GetComponent<InputContextManager>();
            
            if (dialogService == null)
                dialogService = GetComponent<DialogService>();
            
            if (inventoryDebugger == null)
                inventoryDebugger = GetComponent<InventoryDebugger>();
            
            if (coreDebugger == null)
                coreDebugger = GetComponent<PlayerCoreDebugger>();
        }
        
        public global::Player PlayerStateMachine => playerStateMachine;
        public PlayerInputHandler InputHandler => inputHandler;
        public Rigidbody2D Rigidbody => rb;
        public CapsuleCollider2D CapsuleCollider => capsuleCollider;
        public Animator Animator => animator;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public InventorySystem Inventory => inventorySystem;
        public InventoryUIController InventoryUI => inventoryUIController;
        public WeaponInventoryManager WeaponManager => weaponManager;
        public AmmoInventoryManager AmmoManager => ammoManager;
        public PlayerHealthIntegration Health => healthIntegration;
        public PlayerInteractionController InteractionController => interactionController;
        public InputContextManager InputContext => inputContextManager;
        public DialogService DialogService => dialogService;
    }
}
