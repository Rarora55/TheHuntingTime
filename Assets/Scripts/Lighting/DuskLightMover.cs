using UnityEngine;

namespace TheHunt.Lighting
{
    public class DuskLightMover : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private DayNightCycle dayNightCycle;
        [SerializeField] private Transform lightTransform;
        [SerializeField] private Transform baseReference;
        
        [Header("Posiciones (relativas a Base)")]
        [SerializeField] private Transform positionA;
        [SerializeField] private Transform positionCentral;
        [SerializeField] private Transform positionB;

        [Header("Configuración de Movimiento")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float centralPauseDuration = 2f;
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Intensidad por Posición")]
        [SerializeField] private float intensityAtA = 0f;
        [SerializeField] private float intensityAtCentral = 5.89f;
        [SerializeField] private float intensityAtB = 0f;
        [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Referencias de Luz")]
        [SerializeField] private UnityEngine.Rendering.Universal.Light2D targetLight;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private DayPeriod currentPeriod;
        private DayPeriod lastPeriod;
        
        private Vector3 startWorldPos;
        private Vector3 centralWorldPos;
        private Vector3 endWorldPos;
        
        private float movementProgress = 0f;
        private float centralPauseTimer = 0f;
        private bool isInCentralPause = false;
        private bool isMoving = false;

        private enum MovementPhase
        {
            Inactive,
            MovingToCentral,
            PauseAtCentral,
            MovingToEnd,
            Completed
        }

        private MovementPhase currentPhase = MovementPhase.Inactive;

        private void Awake()
        {
            if (dayNightCycle == null)
            {
                dayNightCycle = FindFirstObjectByType<DayNightCycle>();
            }

            if (lightTransform == null)
            {
                lightTransform = transform;
            }

            if (targetLight == null)
            {
                targetLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            }
        }

        private void Start()
        {
            if (dayNightCycle != null)
            {
                dayNightCycle.PeriodChangedEvent.AddListener(OnPeriodChanged);
                currentPeriod = dayNightCycle.CurrentPeriod;
                lastPeriod = currentPeriod;
            }

            CalculateWorldPositions();
            
            if (currentPeriod != DayPeriod.Dusk)
            {
                lightTransform.position = startWorldPos;
                if (targetLight != null)
                {
                    targetLight.intensity = 0f;
                }
            }
        }

        private void OnDestroy()
        {
            if (dayNightCycle != null)
            {
                dayNightCycle.PeriodChangedEvent.RemoveListener(OnPeriodChanged);
            }
        }

        private void Update()
        {
            if (!isMoving) return;

            switch (currentPhase)
            {
                case MovementPhase.MovingToCentral:
                    UpdateMovementToCentral();
                    break;

                case MovementPhase.PauseAtCentral:
                    UpdateCentralPause();
                    break;

                case MovementPhase.MovingToEnd:
                    UpdateMovementToEnd();
                    break;
            }

            UpdateIntensity();
        }

        private void CalculateWorldPositions()
        {
            if (positionA != null)
            {
                startWorldPos = positionA.position;
            }
            else
            {
                startWorldPos = lightTransform.position + Vector3.up * 5f;
            }

            if (positionCentral != null)
            {
                centralWorldPos = positionCentral.position;
            }
            else
            {
                centralWorldPos = lightTransform.position;
            }

            if (positionB != null)
            {
                endWorldPos = positionB.position;
            }
            else
            {
                endWorldPos = lightTransform.position + Vector3.down * 5f;
            }

            if (showDebugLogs)
            {
                Debug.Log($"<color=cyan>[DUSK MOVER] Posiciones calculadas:\nA: {startWorldPos}\nCentral: {centralWorldPos}\nB: {endWorldPos}</color>");
            }
        }

        private void OnPeriodChanged(DayPeriod newPeriod)
        {
            bool wasInDusk = (lastPeriod == DayPeriod.Dusk);
            bool isNowInDusk = (newPeriod == DayPeriod.Dusk);

            if (isNowInDusk && !wasInDusk)
            {
                StartDuskMovement();
            }
            // NO terminar el movimiento cuando cambia a Night - dejar que termine naturalmente

            lastPeriod = newPeriod;
            currentPeriod = newPeriod;
        }

        private void StartDuskMovement()
        {
            CalculateWorldPositions();
            
            lightTransform.position = startWorldPos;
            movementProgress = 0f;
            centralPauseTimer = 0f;
            isInCentralPause = false;
            isMoving = true;
            currentPhase = MovementPhase.MovingToCentral;

            if (targetLight != null)
            {
                targetLight.intensity = intensityAtA;
            }

            if (showDebugLogs)
            {
                Debug.Log($"<color=orange>[DUSK MOVER] 🌇 Iniciando movimiento de atardecer</color>");
                Debug.Log($"<color=cyan>[DUSK MOVER] Intensidades: A={intensityAtA}, Central={intensityAtCentral}, B={intensityAtB}</color>");
            }
        }

        private void EndDuskMovement()
        {
            isMoving = false;
            currentPhase = MovementPhase.Inactive;
            lightTransform.position = startWorldPos;

            if (targetLight != null)
            {
                targetLight.intensity = intensityAtA;
            }

            if (showDebugLogs)
            {
                Debug.Log("<color=blue>[DUSK MOVER] 🌙 Finalizando movimiento de atardecer</color>");
            }
        }

        private void UpdateMovementToCentral()
        {
            movementProgress += Time.deltaTime * moveSpeed;
            float t = Mathf.Clamp01(movementProgress);
            float curvedT = movementCurve.Evaluate(t);

            lightTransform.position = Vector3.Lerp(startWorldPos, centralWorldPos, curvedT);

            if (t >= 1f)
            {
                currentPhase = MovementPhase.PauseAtCentral;
                centralPauseTimer = 0f;
                
                if (showDebugLogs)
                {
                    Debug.Log("<color=yellow>[DUSK MOVER] ⏸️ Pausa en posición central</color>");
                }
            }
        }

        private void UpdateCentralPause()
        {
            centralPauseTimer += Time.deltaTime;

            if (centralPauseTimer >= centralPauseDuration)
            {
                currentPhase = MovementPhase.MovingToEnd;
                movementProgress = 0f;
                
                if (showDebugLogs)
                {
                    Debug.Log("<color=orange>[DUSK MOVER] ▶️ Reanudando movimiento hacia B</color>");
                }
            }
        }

        private void UpdateMovementToEnd()
        {
            movementProgress += Time.deltaTime * moveSpeed;
            float t = Mathf.Clamp01(movementProgress);
            float curvedT = movementCurve.Evaluate(t);

            lightTransform.position = Vector3.Lerp(centralWorldPos, endWorldPos, curvedT);

            if (t >= 1f)
            {
                currentPhase = MovementPhase.Completed;
                isMoving = false;
                
                // Cuando termina el movimiento, volver a posición inicial para el próximo ciclo
                lightTransform.position = startWorldPos;
                if (targetLight != null)
                {
                    targetLight.intensity = intensityAtA;
                }
                
                if (showDebugLogs)
                {
                    Debug.Log("<color=green>[DUSK MOVER] ✓ Movimiento completado - Reset a posición A</color>");
                }
            }
        }

        private void UpdateIntensity()
        {
            if (targetLight == null) return;

            // Usar progreso total continuo del recorrido completo
            float totalProgress = GetTotalProgress();
            
            // El recorrido se divide en:
            // 0.0 - 0.4: A → Central (40%)
            // 0.4 - 0.6: Pausa en Central (20%)
            // 0.6 - 1.0: Central → B (40%)
            
            float intensity = 0f;
            
            if (totalProgress < 0.4f)
            {
                // Fase A → Central (movimiento SOLAMENTE)
                // Normalizar progreso de 0 a 1
                float normalizedProgress = Mathf.Clamp01(totalProgress / 0.4f);
                float curvedProgress = intensityCurve.Evaluate(normalizedProgress);
                intensity = Mathf.Lerp(intensityAtA, intensityAtCentral, curvedProgress);
            }
            else if (totalProgress <= 0.6f)
            {
                // Pausa en Central - MANTENER intensidad máxima sin evaluaciones
                intensity = intensityAtCentral;
            }
            else
            {
                // Fase Central → B (movimiento SOLAMENTE)
                // Normalizar progreso de 0 a 1
                float normalizedProgress = Mathf.Clamp01((totalProgress - 0.6f) / 0.4f);
                
                // Fade out MUY gradual
                // Mantener alto más tiempo, luego bajar suavemente
                float customProgress;
                if (normalizedProgress <= 0.3f)
                {
                    // Mantener máximo durante el primer 30%
                    customProgress = 0f;
                }
                else
                {
                    // Bajar gradualmente en el último 70%
                    customProgress = (normalizedProgress - 0.3f) / 0.7f;
                    // Curva cúbica para fade aún más gradual
                    customProgress = Mathf.Pow(customProgress, 3f);
                }
                
                intensity = Mathf.Lerp(intensityAtCentral, intensityAtB, customProgress);
            }

            targetLight.intensity = intensity;

            if (showDebugLogs && Time.frameCount % 30 == 0)
            {
                Debug.Log($"<color=gray>[DUSK MOVER] Phase: {currentPhase}, Total: {totalProgress:F3}, Intensity: {intensity:F2}, movementProgress: {movementProgress:F3}, pauseTimer: {centralPauseTimer:F3}</color>");
            }
        }

        private float GetPhaseProgress()
        {
            switch (currentPhase)
            {
                case MovementPhase.MovingToCentral:
                case MovementPhase.MovingToEnd:
                    return Mathf.Clamp01(movementProgress);

                case MovementPhase.PauseAtCentral:
                    return Mathf.Clamp01(centralPauseTimer / centralPauseDuration);

                default:
                    return 0f;
            }
        }

        private float GetTotalProgress()
        {
            switch (currentPhase)
            {
                case MovementPhase.Inactive:
                    return 0f;

                case MovementPhase.MovingToCentral:
                    return Mathf.Clamp01(movementProgress) * 0.4f;

                case MovementPhase.PauseAtCentral:
                    float pauseProgress = Mathf.Clamp01(centralPauseTimer / centralPauseDuration);
                    return 0.4f + (pauseProgress * 0.2f);

                case MovementPhase.MovingToEnd:
                    return 0.6f + Mathf.Clamp01(movementProgress) * 0.4f;

                case MovementPhase.Completed:
                    return 1f;

                default:
                    return 0f;
            }
        }

        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public void SetCentralPauseDuration(float duration)
        {
            centralPauseDuration = duration;
        }

        private void OnDrawGizmosSelected()
        {
            if (positionA != null && positionCentral != null && positionB != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(positionA.position, 0.3f);
                Gizmos.DrawLine(positionA.position, positionCentral.position);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(positionCentral.position, 0.5f);
                Gizmos.DrawLine(positionCentral.position, positionB.position);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(positionB.position, 0.3f);
            }
        }
    }
}
