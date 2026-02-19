using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheHunt.Radio
{
    public class RadioDialogVisual : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color[] greenColors = new Color[]
        {
            new Color(0f, 1f, 0f, 0.8f),
            new Color(0.2f, 0.8f, 0.2f, 0.8f),
            new Color(0f, 0.6f, 0f, 0.8f)
        };

        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float colorChangeInterval = 0.5f;

        private Image imageComponent;
        private Coroutine currentAnimation;

        private void Awake()
        {
            TryGetImageComponent();
            
            // Don't deactivate GameObject - instead keep it active with alpha 0
            if (imageComponent != null)
            {
                Color c = imageComponent.color;
                c.a = 0;
                imageComponent.color = c;
                Debug.Log("<color=green>[RADIO DIALOG VISUAL] Image initialized with alpha 0</color>");
            }
            
            Debug.Log($"<color=cyan>[RADIO DIALOG VISUAL] Awake complete - GameObject stays active</color>");
        }

        private void TryGetImageComponent()
        {
            if (imageComponent == null)
            {
                imageComponent = GetComponent<Image>();
                
                if (imageComponent == null)
                {
                    Debug.LogError("<color=red>[RADIO DIALOG VISUAL] No UI.Image component found!</color>");
                }
                else
                {
                    Debug.Log($"<color=green>[RADIO DIALOG VISUAL] UI.Image found! Current color: {imageComponent.color}</color>");
                }
            }
        }

        public void ShowDialog()
        {
            Debug.Log("<color=cyan>[RADIO DIALOG VISUAL] ShowDialog called!</color>");
            
            // Try to get the component again if it's null
            TryGetImageComponent();
            
            if (imageComponent == null)
            {
                Debug.LogError("<color=red>[RADIO DIALOG VISUAL] Cannot show dialog - Image component is null!</color>");
                return;
            }
            
            Debug.Log("<color=green>[RADIO DIALOG VISUAL] Starting dialog animation!</color>");
            
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(DialogAnimationSequence());
        }

        private IEnumerator DialogAnimationSequence()
        {
            Debug.Log("<color=green>[RADIO DIALOG VISUAL] Starting animation sequence...</color>");
            // GameObject stays active throughout - we just change alpha
            
            yield return StartCoroutine(FadeIn());
            
            float elapsed = 0f;
            int colorIndex = 0;
            float lastColorChange = 0f;

            while (elapsed < displayDuration)
            {
                if (elapsed - lastColorChange >= colorChangeInterval)
                {
                    colorIndex = (colorIndex + 1) % greenColors.Length;
                    Color targetColor = greenColors[colorIndex];
                    
                    if (imageComponent != null)
                    {
                        targetColor.a = imageComponent.color.a;
                        imageComponent.color = targetColor;
                    }
                    
                    lastColorChange = elapsed;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            yield return StartCoroutine(FadeOut());
            
            Debug.Log("<color=yellow>[RADIO DIALOG VISUAL] Animation sequence finished</color>");
            currentAnimation = null;
        }

        private IEnumerator FadeIn()
        {
            Debug.Log("<color=cyan>[RADIO DIALOG VISUAL] Fade In...</color>");
            
            if (imageComponent == null)
            {
                Debug.LogError("<color=red>[RADIO DIALOG VISUAL] Image component is null!</color>");
                yield break;
            }
            
            float elapsed = 0f;
            Color startColor = greenColors[0];
            startColor.a = 0f;
            imageComponent.color = startColor;

            Color targetColor = greenColors[0];

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;
                
                Color currentColor = imageComponent.color;
                currentColor.a = Mathf.Lerp(0f, targetColor.a, t);
                imageComponent.color = currentColor;

                yield return null;
            }

            Color finalColor = imageComponent.color;
            finalColor.a = targetColor.a;
            imageComponent.color = finalColor;
            
            Debug.Log($"<color=green>[RADIO DIALOG VISUAL] Fade In complete. Color: {imageComponent.color}</color>");
        }

        private IEnumerator FadeOut()
        {
            Debug.Log("<color=cyan>[RADIO DIALOG VISUAL] Fade Out...</color>");
            
            if (imageComponent == null)
                yield break;
                
            float elapsed = 0f;
            Color startColor = imageComponent.color;
            float startAlpha = startColor.a;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeOutDuration;
                
                Color currentColor = imageComponent.color;
                currentColor.a = Mathf.Lerp(startAlpha, 0f, t);
                imageComponent.color = currentColor;

                yield return null;
            }

            Color finalColor = imageComponent.color;
            finalColor.a = 0f;
            imageComponent.color = finalColor;
        }

        public void SetFadeInDuration(float duration) => fadeInDuration = duration;
        public void SetFadeOutDuration(float duration) => fadeOutDuration = duration;
        public void SetDisplayDuration(float duration) => displayDuration = duration;
        public void SetColorChangeInterval(float interval) => colorChangeInterval = interval;
    }
}
