using UnityEngine;
using TheHunt.Events;

public class DeathSystemDebugger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private DeathData deathData;
    [SerializeField] private Animator animator;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    
    void Start()
    {
        if (player == null)
            player = GetComponent<Player>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void OnGUI()
    {
        if (!showDebugInfo)
            return;
            
        GUILayout.BeginArea(new Rect(10, 200, 450, 450));
        GUILayout.Box("🐛 DEATH SYSTEM DEBUG");
        
        if (player != null && player.StateMachine != null)
        {
            string stateName = player.StateMachine.CurrentState?.GetType().Name ?? "NULL";
            Color stateColor = stateName.Contains("Death") ? Color.red : Color.green;
            GUI.color = stateColor;
            GUILayout.Label($"Current State: {stateName}");
            GUI.color = Color.white;
        }
        
        if (deathData != null)
        {
            GUI.color = deathData.IsDead ? Color.red : Color.green;
            GUILayout.Label($"Is Dead: {deathData.IsDead}");
            GUI.color = Color.white;
            GUILayout.Label($"Death Type: {deathData.CurrentDeathType}");
            GUILayout.Label($"Last Safe Position: {deathData.LastSafePosition}");
        }
        
        HealthController health = player?.GetComponent<HealthController>();
        if (health != null)
        {
            float healthPercent = health.CurrentHealth / health.MaxHealth;
            GUI.color = healthPercent > 0.5f ? Color.green : healthPercent > 0 ? Color.yellow : Color.red;
            GUILayout.Label($"Health: {health.CurrentHealth:F1} / {health.MaxHealth}");
            GUI.color = Color.white;
            GUILayout.Label($"Is Invulnerable: {health.IsInvulnerable}");
        }
        
        if (animator != null)
        {
            GUILayout.Space(5);
            GUILayout.Label("Animator Parameters:");
            
            bool hasDeathParam = false;
            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    bool value = animator.GetBool(param.name);
                    
                    if (param.name == "death")
                    {
                        hasDeathParam = true;
                        GUI.color = value ? Color.red : Color.green;
                        GUILayout.Label($"  ★ {param.name}: {value}");
                        GUI.color = Color.white;
                    }
                    else if (value)
                    {
                        GUI.color = Color.cyan;
                        GUILayout.Label($"  • {param.name}: {value}");
                        GUI.color = Color.white;
                    }
                }
            }
            
            if (!hasDeathParam)
            {
                GUI.color = Color.red;
                GUILayout.Label("  ⚠️ 'death' parameter NOT FOUND!");
                GUI.color = Color.white;
            }
        }
        
        GUILayout.Space(10);
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("💀 INSTANT KILL (Health = 0)", GUILayout.Height(30)))
        {
            HealthController healthCtrl = player?.GetComponent<HealthController>();
            if (healthCtrl != null)
            {
                Debug.Log("<color=yellow>━━━━━━━━━━ FORCING PLAYER DEATH ━━━━━━━━━━</color>");
                healthCtrl.TakeDamage(healthCtrl.MaxHealth * 2f);
                Debug.Log($"<color=yellow>Dealt {healthCtrl.MaxHealth * 2f} damage to kill player</color>");
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("⚡ Take 50 Damage", GUILayout.Height(25)))
        {
            HealthController healthCtrl = player?.GetComponent<HealthController>();
            if (healthCtrl != null)
            {
                healthCtrl.TakeDamage(50f);
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🔄 Force Respawn", GUILayout.Height(25)))
        {
            PlayerRespawnHandler respawn = player?.GetComponent<PlayerRespawnHandler>();
            PlayerRespawnEvent respawnEvent = respawn?.GetType()
                .GetField("onPlayerRespawnEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(respawn) as PlayerRespawnEvent;
            
            if (respawnEvent != null)
            {
                Debug.Log("<color=cyan>━━━━━━━━━━ FORCING RESPAWN ━━━━━━━━━━</color>");
                respawnEvent.Raise();
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("🧹 Clear Death State (Force)", GUILayout.Height(25)))
        {
            if (deathData != null)
            {
                deathData.ClearDeathState();
                Debug.Log("<color=cyan>Death state forcefully cleared</color>");
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("🎬 Force Death Animation", GUILayout.Height(25)))
        {
            if (animator != null)
            {
                Debug.Log("<color=magenta>━━━━━━━━━━ FORCING DEATH ANIMATION ━━━━━━━━━━</color>");
                Debug.Log($"<color=magenta>Current 'death' parameter: {animator.GetBool("death")}</color>");
                animator.SetBool("death", true);
                Debug.Log($"<color=magenta>New 'death' parameter: {animator.GetBool("death")}</color>");
                
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"<color=magenta>Current Animator State: {currentState.shortNameHash} (normalized time: {currentState.normalizedTime})</color>");
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.EndArea();
    }
}
