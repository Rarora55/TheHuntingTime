using UnityEngine;

namespace TheHunt.Lighting.Examples
{
    public class DayNightCycleTester : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private DayNightCycle dayNightCycle;

        [Header("Testing Controls")]
        [SerializeField] private KeyCode pauseKey = KeyCode.P;
        [SerializeField] private KeyCode speedUpKey = KeyCode.Plus;
        [SerializeField] private KeyCode slowDownKey = KeyCode.Minus;
        [SerializeField] private KeyCode testModeKey = KeyCode.T;
        
        [Header("Quick Time Sets")]
        [SerializeField] private KeyCode setDawnKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode setDayKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode setDuskKey = KeyCode.Alpha3;
        [SerializeField] private KeyCode setNightKey = KeyCode.Alpha4;
        [SerializeField] private KeyCode skipPeriodKey = KeyCode.Space;

        private bool isPaused = false;
        private bool isTestMode = false;
        private float normalCycleDuration = 2400f;
        private float testCycleDuration = 120f;

        private void Start()
        {
            if (dayNightCycle == null)
            {
                dayNightCycle = FindFirstObjectByType<DayNightCycle>();
            }

            if (dayNightCycle == null)
            {
                Debug.LogError("<color=red>[DAY/NIGHT TESTER] No DayNightCycle found!</color>");
                enabled = false;
                return;
            }

            PrintControls();
        }

        private void Update()
        {
            if (dayNightCycle == null) return;

            HandleInput();
            DisplayCurrentTime();
        }

        private void HandleInput()
        {
            if (UnityEngine.Input.GetKeyDown(testModeKey))
            {
                ToggleTestMode();
            }

            if (UnityEngine.Input.GetKeyDown(pauseKey))
            {
                TogglePause();
            }

            if (UnityEngine.Input.GetKeyDown(speedUpKey))
            {
                IncreaseSpeed();
            }

            if (UnityEngine.Input.GetKeyDown(slowDownKey))
            {
                DecreaseSpeed();
            }

            if (UnityEngine.Input.GetKeyDown(setDawnKey))
            {
                dayNightCycle.SetToDawn();
                Debug.Log("<color=orange>[TESTER] 🌅 Set to DAWN (Amanecer)</color>");
            }

            if (UnityEngine.Input.GetKeyDown(setDayKey))
            {
                dayNightCycle.SetToDay();
                Debug.Log("<color=yellow>[TESTER] ☀️ Set to DAY (Día)</color>");
            }

            if (UnityEngine.Input.GetKeyDown(setDuskKey))
            {
                dayNightCycle.SetToDusk();
                Debug.Log("<color=orange>[TESTER] 🌇 Set to DUSK (Atardecer)</color>");
            }

            if (UnityEngine.Input.GetKeyDown(setNightKey))
            {
                dayNightCycle.SetToNight();
                Debug.Log("<color=blue>[TESTER] 🌙 Set to NIGHT (Noche)</color>");
            }

            if (UnityEngine.Input.GetKeyDown(skipPeriodKey))
            {
                dayNightCycle.SkipToNextPeriod();
                Debug.Log("<color=cyan>[TESTER] ⏭️ Skipped to next period</color>");
            }
        }

        private void ToggleTestMode()
        {
            isTestMode = !isTestMode;
            
            if (isTestMode)
            {
                Debug.Log("<color=green>[TESTER] 🏃 TEST MODE: Ciclo acelerado (2 minutos)</color>");
            }
            else
            {
                Debug.Log("<color=cyan>[TESTER] 🚶 NORMAL MODE: Ciclo real (40 minutos)</color>");
            }
        }

        private void TogglePause()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                dayNightCycle.PauseCycle();
                Debug.Log("<color=red>[TESTER] ⏸️ PAUSED</color>");
            }
            else
            {
                dayNightCycle.ResumeCycle();
                Debug.Log("<color=green>[TESTER] ▶️ RESUMED</color>");
            }
        }

        private void IncreaseSpeed()
        {
            float currentSpeed = GetCurrentSpeed();
            float newSpeed = currentSpeed * 2f;
            dayNightCycle.SetCycleSpeed(newSpeed);
            Debug.Log($"<color=cyan>[TESTER] ⏩ Speed: {newSpeed}x</color>");
        }

        private void DecreaseSpeed()
        {
            float currentSpeed = GetCurrentSpeed();
            float newSpeed = Mathf.Max(0.25f, currentSpeed * 0.5f);
            dayNightCycle.SetCycleSpeed(newSpeed);
            Debug.Log($"<color=cyan>[TESTER] ⏪ Speed: {newSpeed}x</color>");
        }

        private float GetCurrentSpeed()
        {
            return 1.0f;
        }

        private void DisplayCurrentTime()
        {
            string time = dayNightCycle.GetFormattedTime();
            string period = GetPeriodNameES(dayNightCycle.CurrentPeriod);
            
            string icon = dayNightCycle.CurrentPeriod switch
            {
                DayPeriod.Dawn => "🌅",
                DayPeriod.Day => "☀️",
                DayPeriod.Dusk => "🌇",
                DayPeriod.Night => "🌙",
                _ => "⏰"
            };

            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;

            string modeText = isTestMode ? " [TEST x20]" : "";
            GUI.Label(new Rect(10, 10, 500, 30), $"{icon} {time} - {period}{modeText}", style);
        }

        private string GetPeriodNameES(DayPeriod period)
        {
            return period switch
            {
                DayPeriod.Dawn => "Amanecer",
                DayPeriod.Day => "Día",
                DayPeriod.Dusk => "Atardecer",
                DayPeriod.Night => "Noche",
                _ => "Desconocido"
            };
        }

        private void PrintControls()
        {
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
            Debug.Log("<color=yellow>🎮 DAY/NIGHT CYCLE TESTER - CONTROLES</color>");
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
            Debug.Log($"<color=white>T</color> - Test Mode ON/OFF (ciclo x20 más rápido)");
            Debug.Log($"<color=white>P</color> - Pausar/Reanudar ciclo");
            Debug.Log($"<color=white>+</color> - Acelerar (2x)");
            Debug.Log($"<color=white>-</color> - Ralentizar (0.5x)");
            Debug.Log($"<color=white>1</color> - Ir a Amanecer 🌅");
            Debug.Log($"<color=white>2</color> - Ir a Día ☀️");
            Debug.Log($"<color=white>3</color> - Ir a Atardecer 🌇");
            Debug.Log($"<color=white>4</color> - Ir a Noche 🌙");
            Debug.Log($"<color=white>ESPACIO</color> - Saltar al siguiente periodo");
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
            Debug.Log("<color=green>Duraciones: Día 15min | Atardecer 5min | Noche 15min | Amanecer 5min</color>");
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
        }

        private void OnGUI()
        {
            DisplayCurrentTime();
            DrawControlsHint();
        }

        private void DrawControlsHint()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 14;
            style.normal.textColor = new Color(1, 1, 1, 0.7f);
            style.alignment = TextAnchor.UpperLeft;

            string hint = "Press H for controls";
            GUI.Label(new Rect(10, 45, 300, 20), hint, style);

            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                PrintControls();
            }
        }
    }
}
