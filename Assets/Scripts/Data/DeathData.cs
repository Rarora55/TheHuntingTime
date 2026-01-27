using UnityEngine;
using TheHunt.Events;

[CreateAssetMenu(fileName = "DeathData", menuName = "TheHunt/Data/Death Data")]
public class DeathData : ScriptableObject
{
    [Header("Death Settings")]
    [Tooltip("Tiempo de espera antes de mostrar la pantalla de muerte (debe ser >= duración de la animación de 2s)")]
    [SerializeField] private float normalDeathDuration = 2.5f;
    [Tooltip("Tiempo de espera para muerte por caída")]
    [SerializeField] private float fallDeathDuration = 2.5f;
    [Tooltip("Altura mínima de caída para causar muerte instantánea")]
    [SerializeField] private float fallDeathThreshold = 20f;
    
    [Header("Death Messages")]
    [SerializeField] private string normalDeathTitle = "HAS MUERTO";
    [SerializeField] private string fallDeathTitle = "CAÍDA MORTAL";
    [SerializeField] private string normalDeathMessage = "Presiona para continuar";
    [SerializeField] private string fallDeathMessage = "Cuidado con las alturas";
    
    [Header("Runtime State (Read Only)")]
    [SerializeField] private DeathType currentDeathType;
    [SerializeField] private Vector3 lastSafePosition;
    [SerializeField] private Vector3 deathPosition;
    [SerializeField] private bool isDead;
    
    public float NormalDeathDuration => normalDeathDuration;
    public float FallDeathDuration => fallDeathDuration;
    public float FallDeathThreshold => fallDeathThreshold;
    
    public string NormalDeathTitle => normalDeathTitle;
    public string FallDeathTitle => fallDeathTitle;
    public string NormalDeathMessage => normalDeathMessage;
    public string FallDeathMessage => fallDeathMessage;
    
    public DeathType CurrentDeathType => currentDeathType;
    public Vector3 LastSafePosition => lastSafePosition;
    public Vector3 DeathPosition => deathPosition;
    public bool IsDead => isDead;
    
    public void SetDeathState(DeathType type, Vector3 position)
    {
        currentDeathType = type;
        deathPosition = position;
        isDead = true;
    }
    
    public void SetLastSafePosition(Vector3 position)
    {
        lastSafePosition = position;
    }
    
    public void ClearDeathState()
    {
        isDead = false;
        currentDeathType = DeathType.Normal;
    }
    
    public float GetDeathDuration(DeathType type)
    {
        return type == DeathType.Fall ? fallDeathDuration : normalDeathDuration;
    }
    
    public string GetDeathTitle(DeathType type)
    {
        return type == DeathType.Fall ? fallDeathTitle : normalDeathTitle;
    }
    
    public string GetDeathMessage(DeathType type)
    {
        return type == DeathType.Fall ? fallDeathMessage : normalDeathMessage;
    }
    
    private void OnEnable()
    {
        ClearDeathState();
        lastSafePosition = Vector3.zero;
    }
}
