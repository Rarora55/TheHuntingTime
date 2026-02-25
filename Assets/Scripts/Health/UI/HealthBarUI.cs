using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TheHunt.Health
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthController healthController;

        [Header("UI Elements")]
        [SerializeField] private Image fillImage;
        [SerializeField] private Image damageFillImage;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Colors")]
        [SerializeField] private Color healthyColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color warningColor = new Color(0.9f, 0.9f, 0.2f);
        [SerializeField] private Color criticalColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color damageColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);

        [Header("Animation Settings")]
        [SerializeField] private float fillAnimationSpeed = 5f;
        [SerializeField] private float damageDelayDuration = 0.3f;
        [SerializeField] private float damageAnimationSpeed = 2f;
        [SerializeField] private AnimationCurve fillCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Thresholds")]
        [SerializeField] [Range(0f, 1f)] private float warningThreshold = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float criticalThreshold = 0.25f;

        [Header("Effects")]
        [SerializeField] private bool enablePulseEffect = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.2f;

        private float targetFillAmount;
        private float currentFillAmount;
        private float damageFillAmount;
        private Coroutine damageAnimationCoroutine;
        private bool isPulsing;

        private void Awake()
        {
            if (healthController == null)
            {
                healthController = FindFirstObjectByType<HealthController>();
            }

            if (healthController == null)
            {
                Debug.LogWarning("<color=yellow>[HEALTH BAR UI] No HealthController found in Awake - will wait for external assignment</color>");
                // No desactivar el componente, esperar asignación externa
                return;
            }
            else
            {
                Debug.Log($"<color=green>[HEALTH BAR UI] HealthController found in Awake</color>");
            }
        }
        
        /// <summary>
        /// Conecta este HealthBarUI con un HealthController específico.
        /// Usado cuando el HealthBarUI se instancia dinámicamente.
        /// </summary>
        public void ConnectToHealthController(HealthController controller)
        {
            if (controller == null)
            {
                Debug.LogError("<color=red>[HEALTH BAR UI] Cannot connect to null HealthController!</color>");
                return;
            }
            
            // Desuscribirse del controller anterior si existe
            if (healthController != null)
            {
                healthController.OnHealthChanged -= OnHealthChanged;
                healthController.OnDeath -= OnDeath;
            }
            
            // Asignar nuevo controller
            healthController = controller;
            
            // Suscribirse a eventos
            healthController.OnHealthChanged += OnHealthChanged;
            healthController.OnDeath += OnDeath;
            
            // Inicializar la barra con el estado actual
            InitializeHealthBar();
            
            Debug.Log($"<color=green>[HEALTH BAR UI] ✓ Connected to HealthController - Current HP: {healthController.CurrentHealth:F0}/{healthController.MaxHealth:F0}</color>");
        }

        private void Start()
        {
            InitializeHealthBar();
        }

        private void OnEnable()
        {
            // Solo suscribirse si healthController existe y aún no está suscrito
            if (healthController != null)
            {
                // Desuscribirse primero para evitar duplicados
                healthController.OnHealthChanged -= OnHealthChanged;
                healthController.OnDeath -= OnDeath;
                
                // Suscribirse
                healthController.OnHealthChanged += OnHealthChanged;
                healthController.OnDeath += OnDeath;
                
                Debug.Log("<color=cyan>[HEALTH BAR UI] Subscribed to health events in OnEnable</color>");
            }
        }

        private void OnDisable()
        {
            if (healthController != null)
            {
                healthController.OnHealthChanged -= OnHealthChanged;
                healthController.OnDeath -= OnDeath;
            }
        }

        private void Update()
        {
            AnimateFillBar();

            if (isPulsing && enablePulseEffect)
            {
                ApplyPulseEffect();
            }
        }

        private void InitializeHealthBar()
        {
            if (healthController == null)
                return;

            float healthPercentage = healthController.HealthPercentage;
            targetFillAmount = healthPercentage;
            currentFillAmount = healthPercentage;
            damageFillAmount = healthPercentage;

            if (fillImage != null)
            {
                fillImage.fillAmount = healthPercentage;
                fillImage.color = GetHealthColor(healthPercentage);
            }

            if (damageFillImage != null)
            {
                damageFillImage.fillAmount = healthPercentage;
                damageFillImage.color = damageColor;
            }

            UpdateHealthText();
            CheckPulseState(healthPercentage);

            Debug.Log($"<color=cyan>[HEALTH BAR UI] Initialized with {healthController.CurrentHealth:F0}/{healthController.MaxHealth:F0} HP</color>");
        }

        private void OnHealthChanged(float currentHealth, float previousHealth)
        {
            float healthPercentage = healthController.HealthPercentage;
            targetFillAmount = healthPercentage;

            if (currentHealth < previousHealth)
            {
                if (damageAnimationCoroutine != null)
                {
                    StopCoroutine(damageAnimationCoroutine);
                }
                damageAnimationCoroutine = StartCoroutine(AnimateDamage());
            }
            else
            {
                damageFillAmount = healthPercentage;
                if (damageFillImage != null)
                {
                    damageFillImage.fillAmount = healthPercentage;
                }
            }

            UpdateHealthText();
            CheckPulseState(healthPercentage);

            Debug.Log($"<color=cyan>[HEALTH BAR UI] Health changed: {currentHealth:F0}/{healthController.MaxHealth:F0} ({healthPercentage * 100:F0}%)</color>");
        }

        private void OnDeath()
        {
            isPulsing = false;
            Debug.Log("<color=red>[HEALTH BAR UI] Player died!</color>");
        }

        private void AnimateFillBar()
        {
            if (fillImage == null)
                return;

            float difference = Mathf.Abs(currentFillAmount - targetFillAmount);
            if (difference > 0.001f)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * fillAnimationSpeed);
                fillImage.fillAmount = currentFillAmount;
                fillImage.color = GetHealthColor(currentFillAmount);
            }
            else
            {
                currentFillAmount = targetFillAmount;
                fillImage.fillAmount = targetFillAmount;
            }
        }

        private IEnumerator AnimateDamage()
        {
            yield return new WaitForSeconds(damageDelayDuration);

            while (Mathf.Abs(damageFillAmount - targetFillAmount) > 0.001f)
            {
                damageFillAmount = Mathf.Lerp(damageFillAmount, targetFillAmount, Time.deltaTime * damageAnimationSpeed);

                if (damageFillImage != null)
                {
                    damageFillImage.fillAmount = damageFillAmount;
                }

                yield return null;
            }

            damageFillAmount = targetFillAmount;
            if (damageFillImage != null)
            {
                damageFillImage.fillAmount = targetFillAmount;
            }

            damageAnimationCoroutine = null;
        }

        private void ApplyPulseEffect()
        {
            if (fillImage == null)
                return;

            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            float baseIntensity = 1f;
            float intensity = baseIntensity + pulse;

            Color baseColor = GetHealthColor(targetFillAmount);
            fillImage.color = baseColor * intensity;
        }

        private Color GetHealthColor(float healthPercentage)
        {
            if (healthPercentage <= criticalThreshold)
            {
                return criticalColor;
            }
            else if (healthPercentage <= warningThreshold)
            {
                float t = (healthPercentage - criticalThreshold) / (warningThreshold - criticalThreshold);
                return Color.Lerp(criticalColor, warningColor, t);
            }
            else
            {
                float t = (healthPercentage - warningThreshold) / (1f - warningThreshold);
                return Color.Lerp(warningColor, healthyColor, t);
            }
        }

        private void CheckPulseState(float healthPercentage)
        {
            isPulsing = healthPercentage <= criticalThreshold;
        }

        private void UpdateHealthText()
        {
            if (healthText == null || healthController == null)
                return;

            healthText.text = $"{healthController.CurrentHealth:F0} / {healthController.MaxHealth:F0}";
        }

        public void SetHealthController(HealthController controller)
        {
            if (healthController != null)
            {
                healthController.OnHealthChanged -= OnHealthChanged;
                healthController.OnDeath -= OnDeath;
            }

            healthController = controller;

            if (healthController != null)
            {
                healthController.OnHealthChanged += OnHealthChanged;
                healthController.OnDeath += OnDeath;
                InitializeHealthBar();
            }
        }
    }
}
