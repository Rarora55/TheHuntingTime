using UnityEngine;

public class DebugStateDisplay : MonoBehaviour
{
    private Player player;
    private GUIStyle style;

    private void Start()
    {
        player = GetComponent<Player>();
        
        style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
    }

    private void OnGUI()
    {
        if (player == null || player.StateMachine == null) return;

        string stateName = player.StateMachine.CurrentState?.GetType().Name ?? "NULL";
        
        GUI.Label(new Rect(10, 10, 400, 30), $"Estado Actual: {stateName}", style);
        GUI.Label(new Rect(10, 40, 400, 30), $"FacingRight: {player.FacingRight}", style);
        GUI.Label(new Rect(10, 70, 400, 30), $"Velocity: {player.CurrentVelocity}", style);
        GUI.Label(new Rect(10, 100, 400, 30), $"TouchingWall: {player.CheckIfTouchingWall()}", style);
        GUI.Label(new Rect(10, 130, 400, 30), $"TouchingLedge: {player.CheckTouchingLedge()}", style);
        GUI.Label(new Rect(10, 160, 400, 30), $"xInput: {player.InputHandler.NormInputX}", style);
        GUI.Label(new Rect(10, 190, 400, 30), $"GrabInput: {player.InputHandler.GrabInput}", style);
    }
}
