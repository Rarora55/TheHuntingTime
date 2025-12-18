using UnityEngine;
using UnityEngine.InputSystem;

public class HealthDebugger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthController healthController;

    [Header("Debug Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float healAmount = 15f;

    [Header("Input (Optional - for testing)")]
    [SerializeField] private Key damageKey = Key.Minus;
    [SerializeField] private Key healKey = Key.Equals;
    [SerializeField] private Key resetKey = Key.R;

    private void Awake()
    {
        if (healthController == null)
        {
            healthController = GetComponent<HealthController>();
        }

        if (healthController == null)
        {
            healthController = FindFirstObjectByType<HealthController>();
        }
    }

    private void Update()
    {
        if (healthController == null)
            return;

        if (Keyboard.current[damageKey].wasPressedThisFrame)
        {
            TakeDamage();
        }

        if (Keyboard.current[healKey].wasPressedThisFrame)
        {
            Heal();
        }

        if (Keyboard.current[resetKey].wasPressedThisFrame)
        {
            ResetHealth();
        }
    }

    public void TakeDamage()
    {
        if (healthController != null)
        {
            healthController.TakeDamage(damageAmount);
            Debug.Log($"<color=orange>[DEBUG] Applied {damageAmount} damage. Current HP: {healthController.CurrentHealth:F0}</color>");
        }
    }

    public void Heal()
    {
        if (healthController != null)
        {
            healthController.Heal(healAmount);
            Debug.Log($"<color=green>[DEBUG] Healed {healAmount} HP. Current HP: {healthController.CurrentHealth:F0}</color>");
        }
    }

    public void ResetHealth()
    {
        if (healthController != null)
        {
            healthController.HealToFull();
            Debug.Log($"<color=cyan>[DEBUG] Health reset to full: {healthController.CurrentHealth:F0}</color>");
        }
    }

    public void KillPlayer()
    {
        if (healthController != null)
        {
            healthController.TakeDamage(healthController.MaxHealth);
            Debug.Log("<color=red>[DEBUG] Player killed</color>");
        }
    }

    private void OnGUI()
    {
        if (healthController == null)
            return;

        GUILayout.BeginArea(new Rect(10, 100, 250, 200));
        GUILayout.Label("<b>Health Debug Controls</b>");
        GUILayout.Label($"HP: {healthController.CurrentHealth:F0} / {healthController.MaxHealth:F0}");
        GUILayout.Label($"Percentage: {healthController.HealthPercentage * 100:F0}%");
        GUILayout.Space(10);

        if (GUILayout.Button($"Take {damageAmount} Damage [-]"))
        {
            TakeDamage();
        }

        if (GUILayout.Button($"Heal {healAmount} HP [=]"))
        {
            Heal();
        }

        if (GUILayout.Button("Reset to Full [R]"))
        {
            ResetHealth();
        }

        if (GUILayout.Button("Kill Player"))
        {
            KillPlayer();
        }

        GUILayout.EndArea();
    }
}
