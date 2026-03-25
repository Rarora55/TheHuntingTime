using UnityEngine;

public class PlayerFireState : WeaponAbilityState
{
    private const float AnimationSampleDelayFrames = 2;
    private const float FallbackAnimationDuration = 0.4f;
    private const float AbsoluteTimeoutSeconds = 1.5f;
    private const float AnimationCompletionThreshold = 0.7f;

    private bool hasFired;
    private bool shotPerformed;
    private float fireTime;
    private float animationDuration;
    private int framesElapsed;

    public PlayerFireState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    protected override void OnInvalidWeapon()
    {
        isAbilityDone = true;
        player.InputHandler.FireEnded();
    }

    protected override void OnWeaponStateEnter()
    {
        hasFired = false;
        shotPerformed = false;
        fireTime = Time.time;
        framesElapsed = 0;

        // animationDuration se resuelve en LogicUpdate tras dejar pasar
        // unos frames para que el Animator complete la transición.
        animationDuration = FallbackAnimationDuration;

        if (!weaponController.CanShoot())
        {
            isAbilityDone = true;
            player.InputHandler.FireEnded();
            return;
        }

        player.InputHandler.FireEnded();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Captura la duración real de la animación tras dejar pasar unos frames
        // para que el Animator haya completado su transición al estado fire.
        if (framesElapsed < AnimationSampleDelayFrames)
        {
            framesElapsed++;
        }
        else if (framesElapsed == AnimationSampleDelayFrames)
        {
            AnimatorStateInfo stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
            float sampledDuration = stateInfo.length;
            animationDuration = sampledDuration > 0f ? sampledDuration : FallbackAnimationDuration;
            framesElapsed++;
        }

        float timeSinceFire = Time.time - fireTime;

        // Timeout de seguridad absoluto: si algo falla, salimos siempre.
        if (timeSinceFire >= AbsoluteTimeoutSeconds)
        {
            ExitFireState();
            return;
        }

        // Ejecuta el disparo tras el delay configurado en PlayerData.
        if (!shotPerformed && timeSinceFire >= playerData.shotDelay)
        {
            PerformShot();
            shotPerformed = true;
        }

        // Espera a que la animación llegue al umbral de compleción antes de salir.
        // Si el disparo nunca se realizó (hasFired false), el timeout absoluto cubre la salida.
        if (hasFired && timeSinceFire >= animationDuration * AnimationCompletionThreshold)
        {
            ExitFireState();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    /// <summary>
    /// Centraliza la lógica de salida del FireState: si AimInput sigue activo
    /// vuelve a AimState, de lo contrario marca la ability como finalizada.
    /// </summary>
    private void ExitFireState()
    {
        if (player.InputHandler.AimInput)
        {
            stateMachine.ChangeState(player.AimState);
        }
        else
        {
            isAbilityDone = true;
        }
    }

    /// <summary>
    /// Ejecuta el disparo a través del WeaponController y aplica el knockback correspondiente.
    /// </summary>
    private void PerformShot()
    {
        if (weaponController == null)
            return;

        weaponController.Shoot();
        hasFired = true;

        if (player.KnockbackController != null)
        {
            player.KnockbackController.ApplyShootKnockback();
        }
    }
}
