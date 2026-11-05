using System.Linq;
using Games.Reefscape.Enums;
using Games.Reefscape.Scoring.Scorers;
using MoSimCore.BaseClasses.GameManagement;
using MoSimCore.Enums;
using UnityEngine;

namespace Prefabs.Reefscape.Robots.Mods.SpectrumMod._3847
{
    /// Every strip shows the same 30-pixel pattern, drawn into a texture each frame. Patterns are Spectrum's
    /// (SpectrumLEDs ombre/breathe/rainbow/countdown); which state shows what was picked for the sim.
    public class GammaRayLeds : MonoBehaviour
    {
        [Tooltip("The renderers of the LED strips (the inner 'LED Strip' child of the LED Strip prefab)")]
        [SerializeField] private Renderer[] strips;
        [SerializeField] private Shader ledShader;
        [Tooltip("LedLeft/LedRight: 60 LEDs split into two 30 LED views")]
        [SerializeField] private int ledsPerStrip = 30;
        [Tooltip("Emission multiplier. Too high and every colour clips to white in the tonemapper. Live in Play mode")]
        [SerializeField] private float intensity = 2;
        [Tooltip("Tick if bounce/ombre/countdown run the wrong way along the strip")]
        [SerializeField] private bool reverse;
        [Tooltip("Zones.netAlgaeZone: robot center this far (m) from the barge's center line, +-0.3. 9.618 - field center 8.774")]
        [SerializeField] private float netZoneDistance = 0.844f;
        [SerializeField] private float netZoneTolerance = 0.3f;
        [Tooltip("Auto-align counts as aligned (green) once its position error is under this many meters")]
        [SerializeField] private float alignedDistance = 0.05f;
        [Tooltip("Seconds per breathe cycle")]
        [SerializeField] private float breathePeriod = 2;
        [Tooltip("Breathe's faintest brightness (0-1 of full)")]
        [SerializeField, Range(0, 1)] private float breatheMin = 0.25f;

        [Header("Robot Signal Light: solid while disabled, blinking while enabled")]
        [SerializeField] private Renderer rsl;
        [Tooltip("Which of the RSL renderer's material slots is the lens; it gets its own glowing copy. The slot's material " +
                 "must already have emission on (RSL.mat): builds only include shader variants that materials use")]
        [SerializeField] private int rslSlot = 0;
        [SerializeField] private Color rslColor = new Color(1f, 0.45f, 0f);
        [SerializeField] private float rslIntensity = 4;
        [Tooltip("Seconds on, then the same off, while enabled")]
        [SerializeField] private float rslBlinkTime = 0.25f;

        private static readonly Color Purple = new Color32(130, 103, 185, 255); // SpectrumLEDs.purple
        private static readonly Color CoralWhite = new Color32(200, 200, 200, 255);
        private static readonly Color Aquamarine = new Color32(60, 255, 190, 255);
        private static readonly Color Orange = new Color32(255, 100, 0, 255);
        private static readonly Color Green = new Color32(0, 255, 0, 255);

        private GammaRay _robot;
        private ReefscapeAutoAlign _autoAlign;
        private readonly System.Collections.Generic.List<Material> _mats = new();
        private Texture2D _tex;
        private Color[] _px;
        private Collider[] _barges;
        private GameState _lastGameState = GameState.End;
        private float _autoStart;
        private Material _rslMat;

        private void Start()
        {
            _robot = GetComponent<GammaRay>();
            _autoAlign = GetComponent<ReefscapeAutoAlign>();
            _px = new Color[ledsPerStrip];
            _tex = new Texture2D(ledsPerStrip, 1);
            _tex.filterMode = FilterMode.Point;
            _tex.wrapMode = TextureWrapMode.Clamp;

            // The strip mesh has no UVs along its length, so ledShader (GammaRay/LedStrip) places the pixels by
            // position along the strip's longest side; each strip gets its own material for its own extent.
            foreach (var strip in strips)
            {
                var b = strip.GetComponent<MeshFilter>().sharedMesh.bounds;
                int axis = b.size.x >= b.size.y && b.size.x >= b.size.z ? 0 : b.size.y >= b.size.z ? 1 : 2;
                var dir = Vector3.zero;
                dir[axis] = 1;
                var mat = new Material(ledShader);
                mat.SetTexture("_Texture2D", _tex);
                mat.SetVector("_Axis", dir);
                mat.SetFloat("_Min", b.min[axis]);
                mat.SetFloat("_Max", b.max[axis]);
                strip.sharedMaterial = mat;
                _mats.Add(mat);
            }

            if (rsl && rslSlot < rsl.sharedMaterials.Length)
            {
                var mats = rsl.materials; // instances, so other parts sharing the material don't light up
                _rslMat = mats[rslSlot];
                _rslMat.SetColor("_BaseColor", rslColor);
                _rslMat.EnableKeyword("_EMISSION");
                rsl.materials = mats;
            }

            _barges = FindObjectsByType<BargeScorer>(FindObjectsSortMode.None)
                .Select(b => b.GetComponent<Collider>()).Where(c => c).ToArray();
        }

        private void Update()
        {
            var gm = BaseGameManager.Instance;
            if (gm.GameState == GameState.Auto && _lastGameState != GameState.Auto) _autoStart = Time.time;
            _lastGameState = gm.GameState;

            bool aligning = _robot.AutoAlignLeftAction.IsPressed() || _robot.AutoAlignRightAction.IsPressed();
            bool atReefLevel = _robot.CurrentSetpoint is ReefscapeSetpoints.L1 or ReefscapeSetpoints.L2 or
                ReefscapeSetpoints.L3 or ReefscapeSetpoints.L4;

            if (gm.RobotState == RobotState.Disabled) Rainbow();
            else if (gm.GameState == GameState.Auto) Countdown(Time.time - _autoStart, 15);
            // Teleop, highest priority first
            else if (_robot.CurrentSetpoint == ReefscapeSetpoints.Barge && InNetZone()) Breathe(Green);
            else if (aligning && atReefLevel && _autoAlign && _autoAlign.getDistance() <= alignedDistance) Breathe(Green);
            else if (aligning) Breathe(Orange);
            else if (_robot.IntakingCoral) Breathe(CoralWhite);
            else if (_robot.IntakingAlgae) Breathe(Aquamarine);
            else Ombre(Purple, Color.white);

            if (reverse) System.Array.Reverse(_px);
            _tex.SetPixels(_px);
            _tex.Apply(false);
            foreach (var mat in _mats) mat.SetFloat("_intensity", intensity);

            if (_rslMat)
            {
                bool lit = gm.RobotState == RobotState.Disabled || Time.time % (2 * rslBlinkTime) < rslBlinkTime;
                _rslMat.SetColor("_EmissionColor", lit ? rslColor * rslIntensity : Color.black);
            }
        }

        private bool InNetZone()
        {
            // The barge's center line runs along the long side of its collider; measure across the short side
            foreach (var barge in _barges)
            {
                var b = barge.bounds;
                var d = transform.position - b.center;
                float across = b.size.x < b.size.z ? Mathf.Abs(d.x) : Mathf.Abs(d.z);
                if (Mathf.Abs(across - netZoneDistance) <= netZoneTolerance) return true;
            }
            return false;
        }

        /// SpectrumLEDs.breathe: the whole strip fades between c and a fainter c
        private void Breathe(Color c)
        {
            float b = Mathf.Lerp(breatheMin, 1, 0.5f - 0.5f * Mathf.Cos(Time.time * 2 * Mathf.PI / breathePeriod));
            for (int i = 0; i < _px.Length; i++) _px[i] = new Color(c.r * b, c.g * b, c.b * b);
        }

        /// SpectrumLEDs.rainbow: one full rainbow along the strip, scrolling at 0.58 strip lengths per second
        private void Rainbow()
        {
            int n = _px.Length;
            float shift = Time.time * 0.58f % 1f;
            for (int i = 0; i < n; i++) _px[i] = Color.HSVToRGB(((float)i / n + shift) % 1f, 1, 1);
        }

        /// SpectrumLEDs.ombre: gradient start->end scrolling at 0.58 strip lengths per second
        private void Ombre(Color start, Color end)
        {
            int n = _px.Length;
            float shift = Time.time * 0.58f % 1f;
            for (int i = 0; i < n; i++) _px[i] = Color.Lerp(start, end, (i + n * shift) / n % 1f);
        }

        /// SpectrumLEDs.countdown over a rainbow: LEDs turn off one by one from the end of the strip
        private void Countdown(float elapsed, float duration)
        {
            Rainbow();
            int n = _px.Length;
            float progress = elapsed / duration;
            int off = (int)(n * progress);
            for (int i = 0; i < n; i++) if (progress >= 1 || n - i <= off) _px[i] = Color.black;
        }
    }
}
