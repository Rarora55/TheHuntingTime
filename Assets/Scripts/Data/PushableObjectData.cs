using UnityEngine;

[CreateAssetMenu(fileName = "newPushableObjectData", menuName = "Data/Pushable Object Data")]
public class PushableObjectData : ScriptableObject
{
    [Header("Object Info")]
    public string objectName;
    
    [Header("Weight System")]
    [Tooltip("Peso base del objeto. Afecta la velocidad del jugador al empujar/tirar.")]
    public float baseWeight = 1.0f;
    
    [Header("Push & Pull Permissions")]
    public bool canBePushed = true;
    public bool canBePulled = true;
    
    [Header("Audio (Optional)")]
    public AudioClip pushSound;
    public AudioClip pullSound;
    public AudioClip pushLoopSound;
    public AudioClip pullLoopSound;
}
