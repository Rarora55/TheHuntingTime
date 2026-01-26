using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheHunt.Events;

public class DeathUIController : MonoBehaviour
{
    [Header("SO References")]
    [SerializeField] private DeathData deathData;
    [SerializeField] private ShowDeathScreenEvent showDeathScreenEvent;
    [SerializeField] private PlayerRespawnEvent playerRespawnEvent;
    
    [Header("UI References")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TextMeshProUGUI deathTitleText;
    [SerializeField] private TextMeshProUGUI deathMessageText;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private CanvasGroup canvasGroup;

    void Awake()
    {
        if (canvasGroup == null && deathPanel != null)
            canvasGroup = deathPanel.GetComponent<CanvasGroup>();
            
        if (canvasGroup == null && deathPanel != null)
            canvasGroup = deathPanel.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        if (respawnButton != null)
            respawnButton.onClick.AddListener(OnRespawnClicked);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
        
        if (showDeathScreenEvent != null)
        {
            showDeathScreenEvent.AddListener(ShowDeathScreen);
        }
            
        HideDeathScreen();
    }

    void OnDestroy()
    {
        if (respawnButton != null)
            respawnButton.onClick.RemoveListener(OnRespawnClicked);
            
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);
            
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
        
        if (showDeathScreenEvent != null)
        {
            showDeathScreenEvent.RemoveListener(ShowDeathScreen);
        }
    }

    void ShowDeathScreen(DeathType deathType)
    {
        if (deathPanel == null || deathData == null)
            return;
            
        if (deathTitleText != null)
        {
            deathTitleText.text = deathData.GetDeathTitle(deathType);
        }
        
        if (deathMessageText != null)
        {
            deathMessageText.text = deathData.GetDeathMessage(deathType);
        }
        
        deathPanel.SetActive(true);
        
        StopAllCoroutines();
        StartCoroutine(FadeIn());
        
        Time.timeScale = 0f;
        
        Debug.Log($"<color=magenta>[DEATH UI] Death screen shown - Type: {deathType}, Time paused</color>");
    }

    void HideDeathScreen()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        Time.timeScale = 1f;
        
        Debug.Log("<color=green>[DEATH UI] Death screen hidden, Time resumed</color>");
    }

    System.Collections.IEnumerator FadeIn()
    {
        if (canvasGroup == null)
            yield break;
            
        float elapsed = 0f;
        canvasGroup.alpha = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }

    void OnRespawnClicked()
    {
        HideDeathScreen();
        
        if (playerRespawnEvent != null)
        {
            playerRespawnEvent.Raise();
        }
    }

    void OnRestartClicked()
    {
        PlayerRespawnHandler respawnHandler = FindFirstObjectByType<PlayerRespawnHandler>();
        if (respawnHandler != null)
        {
            respawnHandler.RestartLevel();
        }
    }

    void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
