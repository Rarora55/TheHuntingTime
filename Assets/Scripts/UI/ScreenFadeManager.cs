using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeManager : MonoBehaviour
{
    private static ScreenFadeManager instance;
    public static ScreenFadeManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ScreenFadeManager");
                instance = go.AddComponent<ScreenFadeManager>();
                DontDestroyOnLoad(go);
                instance.Initialize();
            }
            return instance;
        }
    }

    private Canvas fadeCanvas;
    private Image fadeImage;
    private bool isInitialized = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
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

    public void FadeToBlackAndTeleport(Vector3 targetPosition, GameObject playerObject, float fadeDuration = 0.5f)
    {
        StartCoroutine(FadeToBlackAndTeleportCoroutine(targetPosition, playerObject, fadeDuration));
    }
    
    public void FadeToBlack(float fadeDuration, System.Action onComplete = null)
    {
        StartCoroutine(FadeToBlackCoroutine(fadeDuration, onComplete));
    }
    
    public void FadeFromBlack(float fadeDuration, System.Action onComplete = null)
    {
        StartCoroutine(FadeFromBlackCoroutine(fadeDuration, onComplete));
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

    private IEnumerator FadeToBlackAndTeleportCoroutine(Vector3 targetPosition, GameObject playerObject, float fadeDuration)
    {
        if (!isInitialized) Initialize();
        
        fadeCanvas.gameObject.SetActive(true);
        
        yield return StartCoroutine(FadeToBlackInternal(fadeDuration));
        
        if (playerObject != null)
        {
            playerObject.transform.position = targetPosition;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        yield return StartCoroutine(FadeFromBlackInternal(fadeDuration));
        
        fadeCanvas.gameObject.SetActive(false);
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
