using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerCoreDebugger : Editor
{
    private bool showSystems = true;
    private bool showStates = true;
    private bool showEvents = true;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Player player = (Player)target;
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Player Debug Info is only available in Play Mode", 
                MessageType.Info);
            return;
        }
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Player Core Systems Debug", EditorStyles.boldLabel);
        
        DrawSystemsSection(player);
        DrawStatesSection(player);
        DrawEventsSection(player);
    }
    
    private void DrawSystemsSection(Player player)
    {
        showSystems = EditorGUILayout.Foldout(showSystems, "Core Systems", true);
        if (!showSystems) return;
        
        EditorGUI.indentLevel++;
        
        DrawSystemInfo("Physics", 
            $"Velocity: {(player.Physics != null ? player.Physics.CurrentVelocity.ToString() : player.CurrentVelocity.ToString())}", 
            player.Physics != null);
        
        DrawSystemInfo("Collision", 
            $"Grounded: {player.CheckIsGrounded()} | Wall: {player.CheckIfTouchingWall()}", 
            player.Collision != null);
        
        DrawSystemInfo("Orientation", 
            $"Facing: {(player.FacingRight == 1 ? "Right" : "Left")} ({player.FacingRight})", 
            player.Orientation != null);
        
        DrawSystemInfo("Animation", 
            "Animator Active", 
            player.Animation != null);
        
        EditorGUI.indentLevel--;
    }
    
    private void DrawStatesSection(Player player)
    {
        showStates = EditorGUILayout.Foldout(showStates, "State Machine", true);
        if (!showStates) return;
        
        EditorGUI.indentLevel++;
        
        string currentStateName = player.StateMachine?.CurrentState?.GetType().Name ?? "None";
        EditorGUILayout.LabelField("Current State:", currentStateName);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Available States:", EditorStyles.miniLabel);
        EditorGUI.indentLevel++;
        DrawStateInfo("Idle", player.IdleState);
        DrawStateInfo("Move", player.MoveState);
        DrawStateInfo("Air", player.AirState);
        DrawStateInfo("Jump", player.JumpState);
        DrawStateInfo("Land", player.LandState);
        DrawStateInfo("WallClimb", player.WallClimbState);
        DrawStateInfo("WallGrap", player.WallGrapState);
        DrawStateInfo("WallSliced", player.WallSlicedState);
        DrawStateInfo("WallLedge", player.WallLedgeState);
        DrawStateInfo("CrouchIdle", player.CrouchIdleState);
        DrawStateInfo("CrouchMove", player.CrouchMoveState);
        EditorGUI.indentLevel--;
        
        EditorGUI.indentLevel--;
    }
    
    private void DrawEventsSection(Player player)
    {
        showEvents = EditorGUILayout.Foldout(showEvents, "Event System", true);
        if (!showEvents) return;
        
        EditorGUI.indentLevel++;
        
        if (player.Events != null)
        {
            EditorGUILayout.LabelField("Events Status:", "Active ✓");
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Available Events:", EditorStyles.miniLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("• OnStateChanged");
            EditorGUILayout.LabelField("• OnGroundedChanged");
            EditorGUILayout.LabelField("• OnFlipped");
            EditorGUILayout.LabelField("• OnVelocityChanged");
            EditorGUILayout.LabelField("• OnAnimationTrigger");
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUILayout.LabelField("Events Status:", "Inactive ✗");
        }
        
        EditorGUI.indentLevel--;
    }
    
    private void DrawSystemInfo(string name, string info, bool isActive)
    {
        Color originalColor = GUI.color;
        GUI.color = isActive ? Color.green : Color.red;
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"[{name}]", GUILayout.Width(100));
        EditorGUILayout.LabelField(info);
        EditorGUILayout.EndHorizontal();
        
        GUI.color = originalColor;
    }
    
    private void DrawStateInfo(string name, PlayerState state)
    {
        bool isActive = state != null;
        Color originalColor = GUI.color;
        GUI.color = isActive ? new Color(0.7f, 1f, 0.7f) : Color.gray;
        
        EditorGUILayout.LabelField($"• {name}", isActive ? "✓" : "✗");
        
        GUI.color = originalColor;
    }
}
#endif
