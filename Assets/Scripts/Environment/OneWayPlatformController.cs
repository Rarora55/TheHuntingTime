using UnityEngine;
using System.Collections;

public class OneWayPlatformController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float disableTime = 0.5f;
    [SerializeField] private bool allowDropThrough = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    
    private PlatformEffector2D platformEffector;
    private PlayerInputHandler inputHandler;
    private Collider2D playerCollider;
    private bool isPlayerOnPlatform;
    
    private void Awake()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
        
        if (platformEffector == null)
        {
            Debug.LogError($"[ONE WAY PLATFORM] {gameObject.name} no tiene Platform Effector 2D!");
        }
    }
    
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inputHandler = player.GetComponent<PlayerInputHandler>();
            playerCollider = player.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogWarning("[ONE WAY PLATFORM] No se encontró Player en la escena");
        }
    }
    
    private void Update()
    {
        if (!allowDropThrough || !isPlayerOnPlatform) return;
        
        if (inputHandler != null)
        {
            if (inputHandler.NormInputY < 0 && inputHandler.JumpInput)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"<color=yellow>[ONE WAY PLATFORM] Bajando de plataforma: {gameObject.name}</color>");
                }
                
                StartCoroutine(DisablePlatformTemporarily());
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=green>[ONE WAY PLATFORM] Player entró en plataforma: {gameObject.name}</color>");
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=cyan>[ONE WAY PLATFORM] Player salió de plataforma: {gameObject.name}</color>");
            }
        }
    }
    
    private IEnumerator DisablePlatformTemporarily()
    {
        if (platformEffector == null) yield break;
        
        float originalRotation = platformEffector.rotationalOffset;
        platformEffector.rotationalOffset = 180f;
        
        if (showDebugLogs)
        {
            Debug.Log($"<color=yellow>[ONE WAY PLATFORM] Plataforma deshabilitada por {disableTime}s</color>");
        }
        
        yield return new WaitForSeconds(disableTime);
        
        platformEffector.rotationalOffset = originalRotation;
        
        if (showDebugLogs)
        {
            Debug.Log($"<color=green>[ONE WAY PLATFORM] Plataforma reactivada</color>");
        }
    }
}
