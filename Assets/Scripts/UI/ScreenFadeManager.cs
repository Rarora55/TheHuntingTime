using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TheHunt.Events;

public class ScreenFadeManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private ScreenFadeEvent screenFadeEvent;

    [Header("Settings")]
    [SerializeField] private bool createCanvasOnAwake = true;

    private Canvas fadeCanvas;
    private Image fadeImage;
    private bool isInitialized = false;

    private void Awake()
    {
        if (createCanvasOnAwake)
        {
            Initialize();
        }
    }

    private void OnEnable()
    {
        if (screenFadeEvent != null)
        {
            screenFadeEvent.AddListener(HandleFadeRequest);
        }
    }

    private void OnDisable()
    {
        if (screenFadeEvent != null)
        {
            screenFadeEvent.RemoveListener(HandleFadeRequest);
        }
    }

    private void HandleFadeRequest(FadeRequest request)
    {
        switch (request.fadeType)
        {
            case FadeType.ToBlack:
                StartCoroutine(FadeToBlackCoroutine(request.duration, request.onFadeComplete));
                break;
            
            case FadeType.FromBlack:
                StartCoroutine(FadeFromBlackCoroutine(request.duration, request.onFadeComplete));
                break;
            
            case FadeType.ToBlackAndTeleport:
                StartCoroutine(FadeToBlackAndTeleportCoroutine(
                    request.teleportPosition, 
                    request.teleportTarget, 
                    request.duration, 
                    request.onTeleportComplete
                ));
                break;
        }
    }

    private void Initialize()
    {
        if (isInitialized) return;

        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(transform);
        
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;
        
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform);
        
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        fadeCanvas.gameObject.SetActive(false);
        isInitialized = true;
    }

    private IEnumerator FadeToBlackAndTeleportCoroutine(Vector3 targetPosition, Transform targetTransform, float fadeDuration, System.Action onComplete)
    {
        if (!isInitialized) Initialize();
        
        fadeCanvas.gameObject.SetActive(true);
        
        yield return StartCoroutine(FadeToBlackInternal(fadeDuration));
        
        if (targetTransform != null)
        {
            targetTransform.position = targetPosition;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        yield return StartCoroutine(FadeFromBlackInternal(fadeDuration));
        
        fadeCanvas.gameObject.SetActive(false);
        
        onComplete?.Invoke();
    }
    
    private IEnumerator FadeToBlackCoroutine(float fadeDuration, System.Action onComplete)
    {
        if (!isInitialized) Initialize();
        
        fadeCanvas.gameObject.SetActive(true);
        
        yield return StartCoroutine(FadeToBlackInternal(fadeDuration));
        
        onComplete?.Invoke();
    }
    
    private IEnumerator FadeFromBlackCoroutine(float fadeDuration, System.Action onComplete)
    {
        if (!isInitialized) Initialize();
        
        yield return StartCoroutine(FadeFromBlackInternal(fadeDuration));
        
        fadeCanvas.gameObject.SetActive(false);
        
        onComplete?.Invoke();
    }

    private IEnumerator FadeToBlackInternal(float duration)
    {
        float elapsed = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0, 0, 0, 1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadeImage.color = targetColor;
    }

    private IEnumerator FadeFromBlackInternal(float duration)
    {
        float elapsed = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0, 0, 0, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadeImage.color = targetColor;
    }
}
