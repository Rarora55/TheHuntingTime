using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TheHunt.Lighting
{
    public class TorchLight : BaseLightController
    {
        [Header("Torch Specific")]
        [SerializeField] private ParticleSystem fireParticles;
        [SerializeField] private AudioSource fireSound;
        [SerializeField] private bool autoFlicker = true;

        protected override void Awake()
        {
            base.Awake();

            if (autoFlicker)
            {
                behavior = LightBehavior.Flickering;
                flickerSpeed = Random.Range(4f, 6f);
                flickerAmount = Random.Range(0.15f, 0.25f);
            }
        }

        public override void TurnOn()
        {
            base.TurnOn();

            if (fireParticles != null && !fireParticles.isPlaying)
            {
                fireParticles.Play();
            }

            if (fireSound != null && !fireSound.isPlaying)
            {
                fireSound.Play();
            }
        }

        public override void TurnOff()
        {
            base.TurnOff();

            if (fireParticles != null)
            {
                fireParticles.Stop();
            }

            if (fireSound != null)
            {
                fireSound.Stop();
            }
        }
    }
}
