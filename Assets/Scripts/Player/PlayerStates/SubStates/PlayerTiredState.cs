using UnityEngine;

/// <summary>
/// State entered when the player's stamina is fully depleted.
/// Locks all movement for a configurable duration (PlayerData.tiredDuration),
/// plays the "Tired" animation, then transitions back to IdleState.
/// </summary>
public class PlayerTiredState : PlayerState
{
    private const string TiredAnimBool = "tired";

    public PlayerTiredState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    // ── Lifecycle ────────────────────────────────────────────────────────────

    public override void Enter()
    {
        base.Enter();
        StopPlayerCompletely();

        // Force the animation directly by name in addition to the bool,
        // to ensure it plays regardless of transition configuration in the controller.
        player.anim.Play("Tired", 0, 0f);

        Debug.Log($"<color=magenta>[TIRED STATE] Player is tired for {playerData.tiredDuration}s | anim 'tired' = {player.anim.GetBool("tired")}</color>");
    }

    public override void LogicUpdate()
    {
        // No DoChecks call — we intentionally ignore all input until duration expires.
        if (Time.time - startTime >= playerData.tiredDuration)
        {
            Debug.Log("<color=green>[TIRED STATE] Recovery complete. Returning to idle.</color>");
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        // Keep the player frozen in place (gravity still applies via Rigidbody2D).
        StopPlayerCompletely();
    }

    public override void Exit()
    {
        base.Exit();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void StopPlayerCompletely()
    {
        player.RB.linearVelocity = new Vector2(0f, player.RB.linearVelocity.y);
    }
}
