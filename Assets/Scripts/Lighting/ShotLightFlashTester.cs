using UnityEngine;
using UnityEngine.InputSystem;

namespace TheHunt.Lighting
{
    public class ShotLightFlashTester : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private ShotLightFlash shotLightFlash;

        [Header("Test Input")]
        [SerializeField] private Key testKey = Key.F;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[testKey].wasPressedThisFrame)
            {
                if (shotLightFlash != null)
                {
                    shotLightFlash.TriggerFlash();
                    Debug.Log($"<color=green>[SHOT LIGHT TESTER] Flash activado con tecla {testKey}</color>");
                }
            }
        }
    }
}
