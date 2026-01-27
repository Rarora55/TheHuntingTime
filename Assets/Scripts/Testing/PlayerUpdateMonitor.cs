using UnityEngine;

public class PlayerUpdateMonitor : MonoBehaviour
{
    private Player player;
    private int frameCount = 0;
    private bool monitoring = false;
    
    void Start()
    {
        player = GetComponent<Player>();
    }
    
    void Update()
    {
        if (player == null || player.StateMachine == null)
            return;
            
        if (player.StateMachine.CurrentState is PlayerDeathState)
        {
            if (!monitoring)
            {
                frameCount = 0;
                monitoring = true;
                Debug.Log("<color=cyan>[UPDATE MONITOR] DeathState detected - Starting to monitor Update() calls</color>");
            }
            
            frameCount++;
            
            if (frameCount <= 10)
            {
                Debug.Log($"<color=cyan>[UPDATE MONITOR] Frame {frameCount} | TimeScale: {Time.timeScale:F2} | DeltaTime: {Time.deltaTime:F4} | UnscaledDeltaTime: {Time.unscaledDeltaTime:F4}</color>");
            }
            
            if (frameCount == 11)
            {
                Debug.Log("<color=yellow>[UPDATE MONITOR] Stopping verbose logging (10 frames captured)</color>");
            }
        }
        else
        {
            if (monitoring)
            {
                Debug.Log($"<color=green>[UPDATE MONITOR] Exited DeathState after {frameCount} frames</color>");
                monitoring = false;
            }
        }
    }
}
