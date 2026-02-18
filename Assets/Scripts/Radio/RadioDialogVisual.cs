using UnityEngine;
using System.Collections;

namespace TheHunt.Radio
{
    [RequireComponent(typeof(SpriteRenderer))]
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

        private SpriteRenderer spriteRenderer;
        private Coroutine currentAnimation;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            gameObject.SetActive(false);
        }

        public void ShowDialog()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(DialogAnimationSequence());
        }

        private IEnumerator DialogAnimationSequence()
        {
            gameObject.SetActive(true);
            
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
                    targetColor.a = spriteRenderer.color.a;
                    spriteRenderer.color = targetColor;
                    lastColorChange = elapsed;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            yield return StartCoroutine(FadeOut());
            
            gameObject.SetActive(false);
            currentAnimation = null;
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            Color startColor = greenColors[0];
            startColor.a = 0f;
            spriteRenderer.color = startColor;

            Color targetColor = greenColors[0];

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;
                
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(0f, targetColor.a, t);
                spriteRenderer.color = currentColor;

                yield return null;
            }

            Color finalColor = spriteRenderer.color;
            finalColor.a = targetColor.a;
            spriteRenderer.color = finalColor;
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;
            float startAlpha = startColor.a;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeOutDuration;
                
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(startAlpha, 0f, t);
                spriteRenderer.color = currentColor;

                yield return null;
            }

            Color finalColor = spriteRenderer.color;
            finalColor.a = 0f;
            spriteRenderer.color = finalColor;
        }

        public void SetFadeInDuration(float duration) => fadeInDuration = duration;
        public void SetFadeOutDuration(float duration) => fadeOutDuration = duration;
        public void SetDisplayDuration(float duration) => displayDuration = duration;
        public void SetColorChangeInterval(float interval) => colorChangeInterval = interval;
    }
}
