using UnityEngine;

namespace TheHunt.Lighting
{
    public class DuskLightManualTest : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private DayNightPeriodLight duskLight;
        
        [Header("Testing")]
        [SerializeField] private KeyCode fadeInKey = KeyCode.I;
        [SerializeField] private KeyCode fadeOutKey = KeyCode.O;

        private void Start()
        {
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
            Debug.Log("<color=yellow>🔦 DUSK LIGHT MANUAL TEST</color>");
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
            Debug.Log($"<color=white>I</color> - Fade IN (simular atardecer)");
            Debug.Log($"<color=white>O</color> - Fade OUT (simular noche)");
            Debug.Log("<color=cyan>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>");
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(fadeInKey))
            {
                Debug.Log("<color=orange>🌇 Simulando entrada a ATARDECER - Fade IN</color>");
                SimulatePeriodChange(DayPeriod.Dusk);
            }

            if (UnityEngine.Input.GetKeyDown(fadeOutKey))
            {
                Debug.Log("<color=blue>🌙 Simulando entrada a NOCHE - Fade OUT</color>");
                SimulatePeriodChange(DayPeriod.Night);
            }
        }

        private void SimulatePeriodChange(DayPeriod period)
        {
            if (duskLight != null)
            {
                duskLight.SendMessage("OnPeriodChanged", period, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
