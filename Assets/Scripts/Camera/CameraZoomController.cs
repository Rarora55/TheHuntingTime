using UnityEngine;
using Unity.Cinemachine;

namespace TheHunt.Camera
{
    public class CameraZoomController : MonoBehaviour
    {
        [Header("Camera Size Settings")]
        [Tooltip("Tamaño ortográfico de la cámara (menor = más zoom)")]
        [Range(2f, 10f)]
        [SerializeField] private float orthographicSize = 3.5f;

        [Header("References")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private UnityEngine.Camera mainCamera;

        [Header("Runtime Adjustment")]
        [Tooltip("Permitir ajuste en tiempo de ejecución")]
        [SerializeField] private bool allowRuntimeAdjustment = false;

        [Tooltip("Velocidad de transición del zoom")]
        [SerializeField] private float zoomTransitionSpeed = 2f;

        private float currentSize;
        private float targetSize;

        void Awake()
        {
            if (virtualCamera == null)
            {
                virtualCamera = FindFirstObjectByType<CinemachineCamera>();
            }

            if (mainCamera == null)
            {
                mainCamera = UnityEngine.Camera.main;
            }
        }

        void Start()
        {
            ApplyCameraSize();
            currentSize = orthographicSize;
            targetSize = orthographicSize;
        }

        void Update()
        {
            if (allowRuntimeAdjustment)
            {
                if (Mathf.Abs(currentSize - targetSize) > 0.01f)
                {
                    currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomTransitionSpeed);
                    ApplyCameraSizeSmooth(currentSize);
                }
            }
        }

        void OnValidate()
        {
            if (Application.isPlaying)
            {
                targetSize = orthographicSize;
            }
            else
            {
                ApplyCameraSize();
            }
        }

        private void ApplyCameraSize()
        {
            if (virtualCamera != null)
            {
                virtualCamera.Lens.OrthographicSize = orthographicSize;
            }

            if (mainCamera != null)
            {
                mainCamera.orthographicSize = orthographicSize;
            }
        }

        private void ApplyCameraSizeSmooth(float size)
        {
            if (virtualCamera != null)
            {
                virtualCamera.Lens.OrthographicSize = size;
            }

            if (mainCamera != null)
            {
                mainCamera.orthographicSize = size;
            }
        }

        public void SetZoom(float newSize)
        {
            targetSize = Mathf.Clamp(newSize, 2f, 10f);
            orthographicSize = targetSize;
        }

        public void SetZoomImmediate(float newSize)
        {
            orthographicSize = Mathf.Clamp(newSize, 2f, 10f);
            currentSize = orthographicSize;
            targetSize = orthographicSize;
            ApplyCameraSize();
        }

        public float GetCurrentZoom()
        {
            return currentSize;
        }
    }
}
