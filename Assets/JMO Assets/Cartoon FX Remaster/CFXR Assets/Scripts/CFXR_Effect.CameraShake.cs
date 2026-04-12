using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CartoonFX
{
    public partial class CFXR_Effect : MonoBehaviour
    {
        [System.Serializable]
        public class CameraShake
        {
            public enum ShakeSpace
            {
                Screen,
                World
            }

            // Static members
            private static bool s_callbackRegistered;
            private static readonly List<CameraShake> s_cameraShakes = new();
            private static readonly Dictionary<Camera, Vector3> s_camerasPreRenderPosition = new();
            private static readonly Dictionary<Camera, Vector3> _camerasStartPosition = new();

            // Editor settings
#if UNITY_EDITOR
            public static bool EditorPreview { get; set; } = true;
#else
            public static bool EditorPreview => false;
#endif

            // Public properties for inspector
            [SerializeField] private bool enabled = false;
            [SerializeField] private bool useMainCamera = true;
            [SerializeField] private List<Camera> cameras = new List<Camera>();
            [SerializeField] private float delay = 0.0f;
            [SerializeField] private float duration = 1.0f;
            [SerializeField] private ShakeSpace shakeSpace = ShakeSpace.Screen;
            [SerializeField] private Vector3 shakeStrength = new Vector3(0.1f, 0.1f, 0.1f);
            [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.Linear(0, 1, 1, 0);
            [SerializeField, Range(0, 0.1f)] private float shakesDelay = 0;

            // Runtime state
            [System.NonSerialized] public bool IsShaking;
            private Vector3 shakeVector;
            private float delaysTimer;
            private Vector3 _startPosition;

            // Constants
            private const float GLOBAL_CAMERA_SHAKE_MULTIPLIER = 1.0f;

            #region Public API

            public bool Enabled
            {
                get => enabled;
                set => enabled = value;
            }

            public bool UseMainCamera
            {
                get => useMainCamera;
                set => useMainCamera = value;
            }

            public List<Camera> Cameras => cameras;

            public void FetchCameras()
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return;
                }
#endif

                CleanupCameraReferences();
                InitializeCameras();
            }

            public void StartShake()
            {
                if (IsShaking)
                    StopShake();

                IsShaking = true;

                foreach (var cam in cameras)
                {
                    if (cam != null && !_camerasStartPosition.ContainsKey(cam))
                    {
                        _camerasStartPosition[cam] = cam.transform.localPosition;
                    }
                }

                RegisterStaticCallback(this);
            }

            public void StopShake()
            {
                IsShaking = false;
                shakeVector = Vector3.zero;

                foreach (var cam in cameras)
                {
                    if (cam != null && _camerasStartPosition.ContainsKey(cam))
                    {
                        cam.transform.localPosition = _camerasStartPosition[cam];
                        _camerasStartPosition.Remove(cam);
                    }
                }

                UnregisterStaticCallback(this);
            }

            public void Animate(float time)
            {
#if UNITY_EDITOR
                if (!EditorPreview && !EditorApplication.isPlaying)
                {
                    shakeVector = Vector3.zero;
                    return;
                }
#endif

                float totalDuration = duration + delay;
                if (time < totalDuration)
                {
                    if (time < delay)
                    {
                        return;
                    }

                    if (!IsShaking)
                    {
                        StartShake();
                    }

                    ProcessShakeAnimation(time / totalDuration);
                }
                else if (IsShaking)
                {
                    StopShake();
                }
            }

            #endregion

            #region Private Instance Methods

            private void InstanceOnPreRenderCamera(Camera cam)
            {
#if UNITY_EDITOR
                _startPosition = cam.transform.localPosition;

                AddSceneViewCameraIfNeeded(cam);
#endif

                if (IsShaking && s_camerasPreRenderPosition.ContainsKey(cam) && Time.timeScale > 0)
                {
                    s_camerasPreRenderPosition[cam] = cam.transform.localPosition;
                    ApplyShakeToCamera(cam);
                }
            }

            private void InstanceOnPostRenderCamera(Camera cam)
            {
                if (s_camerasPreRenderPosition.ContainsKey(cam))
                {
                    cam.transform.localPosition = s_camerasPreRenderPosition[cam];
                }
            }

            private void ApplyShakeToCamera(Camera cam)
            {
                Vector3 shakeOffset = shakeSpace switch
                {
                    ShakeSpace.Screen => cam.transform.rotation * shakeVector,
                    ShakeSpace.World => shakeVector,
                    _ => Vector3.zero
                };

                cam.transform.localPosition += shakeOffset;
            }

            private void ProcessShakeAnimation(float delta)
            {
                if (ShouldSkipDueToDelay())
                    return;

                Vector3 randomVec = new Vector3(Random.value, Random.value, Random.value);
                Vector3 shakeVec = Vector3.Scale(randomVec, shakeStrength) * (Random.value > 0.5f ? -1 : 1);
                shakeVector = shakeVec * shakeCurve.Evaluate(delta) * GLOBAL_CAMERA_SHAKE_MULTIPLIER;
            }

            private bool ShouldSkipDueToDelay()
            {
                if (shakesDelay <= 0)
                    return false;

                delaysTimer += Time.deltaTime;
                if (delaysTimer < shakesDelay)
                    return true;

                while (delaysTimer >= shakesDelay)
                {
                    delaysTimer -= shakesDelay;
                }

                return false;
            }

            private void CleanupCameraReferences()
            {
                foreach (var cam in cameras)
                {
                    if (cam != null)
                    {
                        s_camerasPreRenderPosition.Remove(cam);
                    }
                }
                cameras.Clear();
            }

            private void InitializeCameras()
            {
                if (useMainCamera && Camera.main != null)
                {
                    cameras.Add(Camera.main);
                }

                foreach (var cam in cameras)
                {
                    if (cam != null && !s_camerasPreRenderPosition.ContainsKey(cam))
                    {
                        s_camerasPreRenderPosition.Add(cam, Vector3.zero);
                    }
                }
            }

#if UNITY_EDITOR
            private void AddSceneViewCameraIfNeeded(Camera cam)
            {
                if (SceneView.currentDrawingSceneView != null &&
                    SceneView.currentDrawingSceneView.camera == cam &&
                    !s_camerasPreRenderPosition.ContainsKey(cam))
                {
                    s_camerasPreRenderPosition.Add(cam, cam.transform.localPosition);
                }
            }
#endif

            #endregion

            #region Static Callback Management

#if UNITY_2019_1_OR_NEWER
            private static void OnPreRenderCameraURP(ScriptableRenderContext context, Camera cam)
            {
                OnPreRenderCamera(cam);
            }

            private static void OnPostRenderCameraURP(ScriptableRenderContext context, Camera cam)
            {
                OnPostRenderCamera(cam);
            }
#endif

            private static void OnPreRenderCamera(Camera cam)
            {
                foreach (var shake in s_cameraShakes)
                {
                    shake.InstanceOnPreRenderCamera(cam);
                }
            }

            private static void OnPostRenderCamera(Camera cam)
            {
                for (int i = s_cameraShakes.Count - 1; i >= 0; i--)
                {
                    s_cameraShakes[i].InstanceOnPostRenderCamera(cam);
                }
            }

            private static void RegisterStaticCallback(CameraShake cameraShake)
            {
                s_cameraShakes.Add(cameraShake);

                if (!s_callbackRegistered)
                {
#if UNITY_2019_1_OR_NEWER
#if UNITY_2019_3_OR_NEWER
                    if (GraphicsSettings.currentRenderPipeline == null)
#else
                    if (GraphicsSettings.renderPipelineAsset == null)
#endif
                    {
                        // Built-in Render Pipeline
                        Camera.onPreRender += OnPreRenderCamera;
                        Camera.onPostRender += OnPostRenderCamera;
                    }
                    else
                    {
                        // URP
                        RenderPipelineManager.beginCameraRendering += OnPreRenderCameraURP;
                        RenderPipelineManager.endCameraRendering += OnPostRenderCameraURP;
                    }
#else
                    Camera.onPreRender += OnPreRenderCamera;
                    Camera.onPostRender += OnPostRenderCamera;
#endif

                    s_callbackRegistered = true;
                }
            }

            private static void UnregisterStaticCallback(CameraShake cameraShake)
            {
                s_cameraShakes.Remove(cameraShake);

                if (s_callbackRegistered && s_cameraShakes.Count == 0)
                {
#if UNITY_2019_1_OR_NEWER
#if UNITY_2019_3_OR_NEWER
                    if (GraphicsSettings.currentRenderPipeline == null)
#else
                    if (GraphicsSettings.renderPipelineAsset == null)
#endif
                    {
                        // Built-in Render Pipeline
                        Camera.onPreRender -= OnPreRenderCamera;
                        Camera.onPostRender -= OnPostRenderCamera;
                    }
                    else
                    {
                        // URP
                        RenderPipelineManager.beginCameraRendering -= OnPreRenderCameraURP;
                        RenderPipelineManager.endCameraRendering -= OnPostRenderCameraURP;
                    }
#else
                    Camera.onPreRender -= OnPreRenderCamera;
                    Camera.onPostRender -= OnPostRenderCamera;
#endif

                    s_callbackRegistered = false;
                }
            }

            #endregion
        }
    }
}