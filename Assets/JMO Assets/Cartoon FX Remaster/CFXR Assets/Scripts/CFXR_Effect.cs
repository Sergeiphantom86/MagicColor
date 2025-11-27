using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CartoonFX
{
    [RequireComponent(typeof(ParticleSystem))]
    [DisallowMultipleComponent]
    public partial class CFXR_Effect : MonoBehaviour
    {
        private const float GLOBAL_CAMERA_SHAKE_MULTIPLIER = 1;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitGlobalOptions()
        {
            AnimatedLight.EditorPreview = EditorPrefs.GetBool("CFXR Light EditorPreview", true);
#if !DISABLE_CAMERA_SHAKE
            CameraShake.EditorPreview = EditorPrefs.GetBool("CFXR CameraShake EditorPreview", true);
#endif
        }
#endif

        public enum ClearBehavior
        {
            None,
            Disable,
            Destroy
        }

        [System.Serializable]
        public class AnimatedLight
        {
            public static bool EditorPreview = true;

            public Light light;

            public bool loop;

            private bool animateIntensity;
            public float IntensityStart = 8f;
            private float intensityDuration = 0.5f;
            private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            private bool perlinIntensity;
            private float perlinIntensitySpeed = 1f;
            private bool fadeIn;
            private float fadeInDuration = 0.5f;
            private bool fadeOut;
            private float fadeOutDuration = 0.5f;

            private bool animateRange;
            private float rangeStart = 8f;
            private float rangeEnd = 0f;
            private float rangeDuration = 0.5f;
            private AnimationCurve rangeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            private bool perlinRange;
            private float perlinRangeSpeed = 1f;

            private bool animateColor;
            private Gradient colorGradient;
            private float colorDuration = 0.5f;
            private AnimationCurve colorCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            private bool perlinColor;
            private float perlinColorSpeed = 1f;

            public float IntensityEnd { get;  set; }

            public void Animate(float time)
            {
#if UNITY_EDITOR
                if (!EditorPreview && !EditorApplication.isPlaying)
                {
                    return;
                }
#endif

                if (light != null)
                {
                    if (animateIntensity)
                    {
                        float delta = loop ? Mathf.Clamp01((time % intensityDuration) / intensityDuration) : Mathf.Clamp01(time / intensityDuration);
                        delta = perlinIntensity ? Mathf.PerlinNoise(Time.time * perlinIntensitySpeed, 0f) : intensityCurve.Evaluate(delta);
                        light.intensity = Mathf.LerpUnclamped(IntensityEnd, IntensityStart, delta);

                        if (fadeIn && time < fadeInDuration)
                        {
                            light.intensity *= Mathf.Clamp01(time / fadeInDuration);
                        }
                    }

                    if (animateRange)
                    {
                        float delta = loop ? Mathf.Clamp01((time % rangeDuration) / rangeDuration) : Mathf.Clamp01(time / rangeDuration);
                        delta = perlinRange ? Mathf.PerlinNoise(Time.time * perlinRangeSpeed, 10f) : rangeCurve.Evaluate(delta);
                        light.range = Mathf.LerpUnclamped(rangeEnd, rangeStart, delta);
                    }

                    if (animateColor)
                    {
                        float delta = loop ? Mathf.Clamp01((time % colorDuration) / colorDuration) : Mathf.Clamp01(time / colorDuration);
                        delta = perlinColor ? Mathf.PerlinNoise(Time.time * perlinColorSpeed, 0f) : colorCurve.Evaluate(delta);
                        light.color = colorGradient.Evaluate(delta);
                    }
                }
            }

            public void AnimateFadeOut(float time)
            {
                if (fadeOut && light != null)
                {
                    light.intensity *= 1.0f - Mathf.Clamp01(time / fadeOutDuration);
                }
            }

            public void Reset()
            {
                if (light != null)
                {
                    if (animateIntensity)
                    {
                        light.intensity = (fadeIn || fadeOut) ? 0 : IntensityEnd;
                    }

                    if (animateRange)
                    {
                        light.range = rangeEnd;
                    }

                    if (animateColor)
                    {
                        light.color = colorGradient.Evaluate(1f);
                    }
                }
            }

#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(AnimatedLight))]
            public class AnimatedLightDrawer : PropertyDrawer
            {
                private SerializedProperty light;
                private SerializedProperty loop;
                private SerializedProperty animateIntensity;
                private SerializedProperty intensityStart;
                private SerializedProperty intensityEnd;
                private SerializedProperty intensityDuration;
                private SerializedProperty intensityCurve;
                private SerializedProperty perlinIntensity;
                private SerializedProperty perlinIntensitySpeed;
                private SerializedProperty fadeIn;
                private SerializedProperty fadeInDuration;
                private SerializedProperty fadeOut;
                private SerializedProperty fadeOutDuration;
                private SerializedProperty animateRange;
                private SerializedProperty rangeStart;
                private SerializedProperty rangeEnd;
                private SerializedProperty rangeDuration;
                private SerializedProperty rangeCurve;
                private SerializedProperty perlinRange;
                private SerializedProperty perlinRangeSpeed;
                private SerializedProperty animateColor;
                private SerializedProperty colorGradient;
                private SerializedProperty colorDuration;
                private SerializedProperty colorCurve;
                private SerializedProperty perlinColor;
                private SerializedProperty perlinColorSpeed;

                private static GUIContent[] ModePopupLabels = new GUIContent[] { new GUIContent("Curve"), new GUIContent("Perlin Noise") };
                private static GUIContent IntensityModeLabel = new GUIContent("Intensity Mode");
                private static GUIContent RangeModeLabel = new GUIContent("Range Mode");
                private static GUIContent ColorModeLabel = new("Color Mode");

                private const float INDENT_WIDTH = 15f;
                private const float PADDING = 4f;

                private void StartIndent(ref Rect rect)
                {
                    EditorGUIUtility.labelWidth -= INDENT_WIDTH;
                    rect.xMin += INDENT_WIDTH;
                }

                private void EndIndent(ref Rect rect)
                {
                    EditorGUIUtility.labelWidth += INDENT_WIDTH;
                    rect.xMin -= INDENT_WIDTH;
                }

                private void FetchProperties(SerializedProperty property)
                {
                    light = property.FindPropertyRelative("light");
                    loop = property.FindPropertyRelative("loop");
                    animateIntensity = property.FindPropertyRelative("animateIntensity");
                    intensityStart = property.FindPropertyRelative("intensityStart");
                    intensityEnd = property.FindPropertyRelative("intensityEnd");
                    intensityDuration = property.FindPropertyRelative("intensityDuration");
                    intensityCurve = property.FindPropertyRelative("intensityCurve");
                    perlinIntensity = property.FindPropertyRelative("perlinIntensity");
                    perlinIntensitySpeed = property.FindPropertyRelative("perlinIntensitySpeed");
                    fadeIn = property.FindPropertyRelative("fadeIn");
                    fadeInDuration = property.FindPropertyRelative("fadeInDuration");
                    fadeOut = property.FindPropertyRelative("fadeOut");
                    fadeOutDuration = property.FindPropertyRelative("fadeOutDuration");
                    animateRange = property.FindPropertyRelative("animateRange");
                    rangeStart = property.FindPropertyRelative("rangeStart");
                    rangeEnd = property.FindPropertyRelative("rangeEnd");
                    rangeDuration = property.FindPropertyRelative("rangeDuration");
                    rangeCurve = property.FindPropertyRelative("rangeCurve");
                    perlinRange = property.FindPropertyRelative("perlinRange");
                    perlinRangeSpeed = property.FindPropertyRelative("perlinRangeSpeed");
                    animateColor = property.FindPropertyRelative("animateColor");
                    colorGradient = property.FindPropertyRelative("colorGradient");
                    colorDuration = property.FindPropertyRelative("colorDuration");
                    colorCurve = property.FindPropertyRelative("colorCurve");
                    perlinColor = property.FindPropertyRelative("perlinColor");
                    perlinColorSpeed = property.FindPropertyRelative("perlinColorSpeed");
                }

                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    FetchProperties(property);

                    Rect rect = EditorGUI.IndentedRect(position);

                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorStyles.helpBox.Draw(rect, GUIContent.none, 0);
                    }

                    EditorGUIUtility.labelWidth -= INDENT_WIDTH;

                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.xMax -= PADDING;
                    rect.y += PADDING;
                    float propSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    EditorGUI.PropertyField(rect, light); rect.y += propSpace;
                    EditorGUI.PropertyField(rect, loop); rect.y += propSpace;

                    EditorGUI.PropertyField(rect, animateIntensity); rect.y += propSpace;
                    if (animateIntensity.boolValue)
                    {
                        StartIndent(ref rect);
                        {
                            EditorGUI.PropertyField(rect, intensityStart); rect.y += propSpace;
                            EditorGUI.PropertyField(rect, intensityEnd); rect.y += propSpace;

                            int val = EditorGUI.Popup(rect, IntensityModeLabel, perlinIntensity.boolValue ? 1 : 0, ModePopupLabels); rect.y += propSpace;
                            if (val == 1 && !perlinIntensity.boolValue)
                            {
                                perlinIntensity.boolValue = true;
                            }
                            else if (val == 0 && perlinIntensity.boolValue)
                            {
                                perlinIntensity.boolValue = false;
                            }

                            StartIndent(ref rect);
                            {
                                if (perlinIntensity.boolValue)
                                {
                                    EditorGUI.PropertyField(rect, perlinIntensitySpeed); rect.y += propSpace;
                                }
                                else
                                {
                                    EditorGUI.PropertyField(rect, intensityDuration); rect.y += propSpace;
                                    EditorGUI.PropertyField(rect, intensityCurve); rect.y += propSpace;
                                }
                            }
                            EndIndent(ref rect);

                            EditorGUI.PropertyField(rect, fadeIn); rect.y += propSpace;
                            if (fadeIn.boolValue)
                            {
                                StartIndent(ref rect);
                                EditorGUI.PropertyField(rect, fadeInDuration); rect.y += propSpace;
                                EndIndent(ref rect);
                            }

                            EditorGUI.PropertyField(rect, fadeOut); rect.y += propSpace;
                            if (fadeOut.boolValue)
                            {
                                StartIndent(ref rect);
                                EditorGUI.PropertyField(rect, fadeOutDuration); rect.y += propSpace;
                                EndIndent(ref rect);
                            }
                        }
                        EndIndent(ref rect);
                    }

                    EditorGUI.PropertyField(rect, animateRange); rect.y += propSpace;
                    if (animateRange.boolValue)
                    {
                        StartIndent(ref rect);
                        {
                            EditorGUI.PropertyField(rect, rangeStart); rect.y += propSpace;
                            EditorGUI.PropertyField(rect, rangeEnd); rect.y += propSpace;

                            int val = EditorGUI.Popup(rect, RangeModeLabel, perlinRange.boolValue ? 1 : 0, ModePopupLabels); rect.y += propSpace;
                            if (val == 1 && !perlinRange.boolValue)
                            {
                                perlinRange.boolValue = true;
                            }
                            else if (val == 0 && perlinRange.boolValue)
                            {
                                perlinRange.boolValue = false;
                            }

                            StartIndent(ref rect);
                            {
                                if (perlinRange.boolValue)
                                {
                                    EditorGUI.PropertyField(rect, perlinRangeSpeed); rect.y += propSpace;
                                }
                                else
                                {
                                    EditorGUI.PropertyField(rect, rangeDuration); rect.y += propSpace;
                                    EditorGUI.PropertyField(rect, rangeCurve); rect.y += propSpace;
                                }
                            }
                            EndIndent(ref rect);
                        }
                        EndIndent(ref rect);
                    }

                    EditorGUI.PropertyField(rect, animateColor); rect.y += propSpace;
                    if (animateColor.boolValue)
                    {
                        StartIndent(ref rect);
                        {
                            EditorGUI.PropertyField(rect, colorGradient); rect.y += propSpace;

                            int val = EditorGUI.Popup(rect, ColorModeLabel, perlinColor.boolValue ? 1 : 0, ModePopupLabels); rect.y += propSpace;
                            if (val == 1 && !perlinColor.boolValue)
                            {
                                perlinColor.boolValue = true;
                            }
                            else if (val == 0 && perlinColor.boolValue)
                            {
                                perlinColor.boolValue = false;
                            }

                            StartIndent(ref rect);
                            {
                                if (perlinColor.boolValue)
                                {
                                    EditorGUI.PropertyField(rect, perlinColorSpeed); rect.y += propSpace;
                                }
                                else
                                {
                                    EditorGUI.PropertyField(rect, colorDuration); rect.y += propSpace;
                                    EditorGUI.PropertyField(rect, colorCurve); rect.y += propSpace;
                                }
                            }
                            EndIndent(ref rect);
                        }
                        EndIndent(ref rect);
                    }

                    EditorGUIUtility.labelWidth += INDENT_WIDTH;

                    if (GUI.changed)
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    FetchProperties(property);

                    float propSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    int count = 5;

                    if (animateIntensity.boolValue)
                    {
                        count += 3;
                        count += perlinIntensity.boolValue ? 1 : 2;
                        count += 1;
                        count += fadeIn.boolValue ? 1 : 0;
                        count += 1;
                        count += fadeOut.boolValue ? 1 : 0;
                    }

                    if (animateRange.boolValue)
                    {
                        count += 3;
                        count += perlinRange.boolValue ? 1 : 2;
                    }

                    if (animateColor.boolValue)
                    {
                        count += 2;
                        count += perlinColor.boolValue ? 1 : 2;
                    }

                    return count * propSpace + PADDING * 2;
                }
            }
#endif
        }

        public static bool GlobalDisableCameraShake;
        public static bool GlobalDisableLights;

        [Tooltip("Defines an action to execute when the Particle System has completely finished playing and emitting particles.")]
        public ClearBehavior clearBehavior = ClearBehavior.Destroy;
        [Space]
        public CameraShake cameraShake;
        [Space]
        public AnimatedLight[] animatedLights;
        [Tooltip("Defines which Particle System to track to trigger light fading out.\nLeave empty if not using fading out.")]
        public ParticleSystem fadeOutReference;

        private float time;
        private ParticleSystem rootParticleSystem;
        [System.NonSerialized] private MaterialPropertyBlock materialPropertyBlock;
        [System.NonSerialized] private Renderer particleRenderer;

        public void ResetState()
        {
            time = 0f;
            fadingOutStartTime = 0f;
            isFadingOut = false;

#if !DISABLE_LIGHTS
            if (animatedLights != null)
            {
                foreach (var animLight in animatedLights)
                {
                    animLight.Reset();
                }
            }
#endif

#if !DISABLE_CAMERA_SHAKE
            if (cameraShake != null && cameraShake.Enabled)
            {
                cameraShake.StopShake();
            }
#endif
        }

#if !DISABLE_CAMERA_SHAKE || !DISABLE_CLEAR_BEHAVIOR
        private void Awake()
        {
#if !DISABLE_CAMERA_SHAKE
            if (cameraShake != null && cameraShake.Enabled)
            {
                cameraShake.FetchCameras();
            }
#endif
#if !DISABLE_CLEAR_BEHAVIOR
            startFrameOffset = GlobalStartFrameOffset++;
#endif
            particleRenderer = this.GetComponent<ParticleSystemRenderer>();
            if (particleRenderer.sharedMaterial != null && particleRenderer.sharedMaterial.IsKeywordEnabled("_CFXR_LIGHTING_WPOS_OFFSET"))
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }

#if !DISABLE_LIGHTS && !DISABLE_LIGHTS_LINEAR_REMAPPING
            if (!GraphicsSettings.lightsUseLinearIntensity && animatedLights != null)
            {
                foreach (var animLight in animatedLights)
                {
                    animLight.IntensityStart = Mathf.LinearToGammaSpace(animLight.IntensityStart);
                    animLight.IntensityEnd = Mathf.LinearToGammaSpace(animLight.IntensityEnd);
                }
            }
#endif
        }
#endif

        private void OnEnable()
        {
            foreach (var animLight in animatedLights)
            {
                if (animLight.light != null)
                {
#if !DISABLE_LIGHTS
                    animLight.light.enabled = !GlobalDisableLights;
#else
                    animLight.light.enabled = false;
#endif
                }
            }
        }

        private void OnDisable()
        {
            ResetState();
        }

#if !DISABLE_LIGHTS || !DISABLE_CAMERA_SHAKE || !DISABLE_CLEAR_BEHAVIOR
        private const int CHECK_EVERY_N_FRAME = 20;
        private static int GlobalStartFrameOffset = 0;
        private int startFrameOffset;

        private void Update()
        {
#if !DISABLE_LIGHTS || !DISABLE_CAMERA_SHAKE
            time += Time.deltaTime;

            Animate(time);

            if (fadeOutReference != null && !fadeOutReference.isEmitting && (fadeOutReference.isPlaying || isFadingOut))
            {
                FadeOut(time);
            }
#endif
#if !DISABLE_CLEAR_BEHAVIOR
            if (clearBehavior != ClearBehavior.None)
            {
                if (rootParticleSystem == null)
                {
                    rootParticleSystem = this.GetComponent<ParticleSystem>();
                }

                if ((Time.renderedFrameCount + startFrameOffset) % CHECK_EVERY_N_FRAME == 0)
                {
                    if (!rootParticleSystem.IsAlive(true))
                    {
                        if (clearBehavior == ClearBehavior.Destroy)
                        {
                            GameObject.Destroy(this.gameObject);
                        }
                        else
                        {
                            this.gameObject.SetActive(false);
                        }
                    }
                }
            }
#endif
            if (materialPropertyBlock != null)
            {
                particleRenderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetVector("_GameObjectWorldPosition", this.transform.position);
                particleRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
#endif

#if !DISABLE_LIGHTS || !DISABLE_CAMERA_SHAKE
        public void Animate(float time)
        {
#if !DISABLE_LIGHTS
            if (animatedLights != null && !GlobalDisableLights)
            {
                foreach (var animLight in animatedLights)
                {
                    animLight.Animate(time);
                }
            }
#endif

#if !DISABLE_CAMERA_SHAKE
            if (cameraShake != null && cameraShake.Enabled && !GlobalDisableCameraShake)
            {
#if UNITY_EDITOR
                if (!cameraShake.IsShaking)
                {
                    cameraShake.FetchCameras();
                }
#endif
                cameraShake.Animate(time);
            }
#endif
        }
#endif

#if !DISABLE_LIGHTS
        private bool isFadingOut;
        private float fadingOutStartTime;

        public void FadeOut(float time)
        {
            if (animatedLights == null)
            {
                return;
            }

            if (!isFadingOut)
            {
                isFadingOut = true;
                fadingOutStartTime = time;
            }

            foreach (var animLight in animatedLights)
            {
                animLight.AnimateFadeOut(time - fadingOutStartTime);
            }
        }
#endif

#if UNITY_EDITOR
        [System.NonSerialized] private ParticleSystem _parentParticle;

        private ParticleSystem ParentParticle
        {
            get
            {
                if (_parentParticle == null)
                {
                    _parentParticle = GetComponent<ParticleSystem>();
                }
                return _parentParticle;
            }
        }

        [System.NonSerialized] public bool editorUpdateRegistered;

        [System.NonSerialized] private bool particleWasStopped;
        [System.NonSerialized] private float particleTime;
        [System.NonSerialized] private float particleTimeUnwrapped;

        private void OnDestroy()
        {
            UnregisterEditorUpdate();
        }

        public void RegisterEditorUpdate()
        {
            var type = PrefabUtility.GetPrefabAssetType(this.gameObject);
            var status = PrefabUtility.GetPrefabInstanceStatus(this.gameObject);

            if ((type == PrefabAssetType.Regular || type == PrefabAssetType.Variant) && status == PrefabInstanceStatus.NotAPrefab)
            {
                return;
            }

            if (!editorUpdateRegistered)
            {
                EditorApplication.update += OnEditorUpdate;
                editorUpdateRegistered = true;
            }
        }

        public void UnregisterEditorUpdate()
        {
            if (editorUpdateRegistered)
            {
                editorUpdateRegistered = false;
                EditorApplication.update -= OnEditorUpdate;
            }
            ResetState();
        }

        private void OnEditorUpdate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (this == null)
            {
                return;
            }

            var renderer = this.GetComponent<ParticleSystemRenderer>();
            if (renderer.sharedMaterial != null && renderer.sharedMaterial.IsKeywordEnabled("_CFXR_LIGHTING_WPOS_OFFSET"))
            {
                if (materialPropertyBlock == null)
                {
                    materialPropertyBlock = new MaterialPropertyBlock();
                }

                renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetVector("_GameObjectWorldPosition", this.transform.position);
                renderer.SetPropertyBlock(materialPropertyBlock);
            }

            float delta = ParentParticle.time - particleTime;

            if (delta < 0 && ParentParticle.isPlaying)
            {
                delta = ParentParticle.main.duration + delta;
                if (delta > 0.1 || delta < 0)
                {
                    ResetState();
                    particleTimeUnwrapped = 0;
                    delta = 0;
                }
            }
            particleTimeUnwrapped += delta;

            if (particleTime != ParentParticle.time)
            {
#if !DISABLE_CAMERA_SHAKE
                if (cameraShake != null && cameraShake.Enabled && ParentParticle.time < particleTime && ParentParticle.time < 0.05f)
                {
                    cameraShake.StartShake();
                }
#endif
#if !DISABLE_LIGHTS || !DISABLE_CAMERA_SHAKES
                Animate(particleTimeUnwrapped);

                if (!ParentParticle.isEmitting)
                {
                    FadeOut(particleTimeUnwrapped);
                }
#endif
            }

            if (particleWasStopped != ParentParticle.isStopped)
            {
                if (ParentParticle.isStopped)
                {
                    ResetState();
                }
                particleTimeUnwrapped = 0;
            }

            particleWasStopped = ParentParticle.isStopped;
            particleTime = ParentParticle.time;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CFXR_Effect))]
    [CanEditMultipleObjects]
    public class CFXR_Effect_Editor : Editor
    {
        private bool? lightEditorPreview;
        private bool? shakeEditorPreview;

        private GUIStyle _PaddedRoundedRect;

        private GUIStyle PaddedRoundedRect
        {
            get
            {
                if (_PaddedRoundedRect == null)
                {
                    _PaddedRoundedRect = new GUIStyle(EditorStyles.helpBox);
                    _PaddedRoundedRect.padding = new RectOffset(4, 4, 4, 4);
                }
                return _PaddedRoundedRect;
            }
        }

        public override void OnInspectorGUI()
        {
            GlobalOptionsGUI();

#if DISABLE_CAMERA_SHAKE
            EditorGUILayout.HelpBox("Camera Shake has been globally disabled in the code.\nThe properties remain to avoid data loss but the shaking won't be applied for any effect.", MessageType.Info);
#endif

            base.OnInspectorGUI();
        }

        private void GlobalOptionsGUI()
        {
            EditorGUILayout.BeginVertical(PaddedRoundedRect);
            {
                GUILayout.Label("Editor Preview:", EditorStyles.boldLabel);

                if (lightEditorPreview == null)
                {
                    lightEditorPreview = EditorPrefs.GetBool("CFXR Light EditorPreview", true);
                }
                bool lightPreview = EditorGUILayout.Toggle("Light Animations", lightEditorPreview.Value);
                if (lightPreview != lightEditorPreview.Value)
                {
                    lightEditorPreview = lightPreview;
                    EditorPrefs.SetBool("CFXR Light EditorPreview", lightPreview);
                    CFXR_Effect.AnimatedLight.EditorPreview = lightPreview;
                }

#if !DISABLE_CAMERA_SHAKE
                if (shakeEditorPreview == null)
                {
                    shakeEditorPreview = EditorPrefs.GetBool("CFXR CameraShake EditorPreview", true);
                }
                bool shakePreview = EditorGUILayout.Toggle("Camera Shake", shakeEditorPreview.Value);
                if (shakePreview != shakeEditorPreview.Value)
                {
                    shakeEditorPreview = shakePreview;
                    EditorPrefs.SetBool("CFXR CameraShake EditorPreview", shakePreview);
                    CFXR_Effect.CameraShake.EditorPreview = shakePreview;
                }
#endif
            }
            EditorGUILayout.EndVertical();
        }

        private void OnEnable()
        {
            if (this.targets == null)
            {
                return;
            }

            foreach (var t in this.targets)
            {
                var cfxr_effect = t as CFXR_Effect;
                if (cfxr_effect != null)
                {
                    if (IsPrefabSource(cfxr_effect.gameObject))
                    {
                        return;
                    }
                    cfxr_effect.RegisterEditorUpdate();
                }
            }
        }

        private void OnDisable()
        {
            if (this.targets == null)
            {
                return;
            }

            foreach (var t in this.targets)
            {
                var cfxr_effect = t as CFXR_Effect;

                if (cfxr_effect != null)
                {
                    if (IsPrefabSource(cfxr_effect.gameObject))
                    {
                        return;
                    }
                    cfxr_effect.UnregisterEditorUpdate();
                }
            }
        }

        private static bool IsPrefabSource(GameObject gameObject)
        {
            var assetType = PrefabUtility.GetPrefabAssetType(gameObject);
            var prefabType = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            return ((assetType == PrefabAssetType.Regular || assetType == PrefabAssetType.Variant) && prefabType == PrefabInstanceStatus.NotAPrefab);
        }
    }
#endif
}