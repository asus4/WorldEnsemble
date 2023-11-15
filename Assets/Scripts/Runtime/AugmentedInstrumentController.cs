
namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using MemoryPack.Internal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Entry point for the Augmented Instrument Project!
    /// </summary>
    public sealed class AugmentedInstrumentController : MonoBehaviour
    {
        [System.Serializable]
        public sealed class TapPointEvent : UnityEvent<Vector3> { }


        [SerializeField]
        private AppSettings _settings;

        [Header("Input Actions")]
        [SerializeField]
        private InputAction _touchDragAction;
        [SerializeField]
        private InputAction _touchDecideAction;
        [SerializeField]
        private InputAction _kickAction;

        [Header("Prefabs")]
        [SerializeField]
        private ARInstrument[] _instrumentPrefabs;
        [SerializeField]
        private RaycastCursor _cursorPrefab;

        [Header("Events")]
        public TapPointEvent onTapAir;

        private readonly List<ARInstrument> _instruments = new();
        private Vector2 _lastPointerPosition;
        private RaycastCursor _cursor;
        private RhythmSequencer _sequencer;


        private InputAction[] AllActions => new InputAction[]
        {
            _touchDragAction,
            _touchDecideAction,
            _kickAction,
        };

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _touchDragAction.performed += OnTouchDrag;
            _touchDecideAction.performed += OnTouchDecide;
            _kickAction.performed += OnKick;

            _cursor = Instantiate(_cursorPrefab);
            _cursor.transform.SetParent(transform);
        }

        private void Start()
        {
            AudioListener audioListener = FindObjectOfType<AudioListener>();
            _sequencer = new RhythmSequencer();
            _sequencer.Start(_settings.BPM, audioListener);
        }

        private void OnDestroy()
        {
            _sequencer?.Dispose();

            _touchDragAction.performed -= OnTouchDrag;
            _touchDecideAction.performed -= OnTouchDecide;
            _kickAction.performed -= OnKick;
        }

        private void OnDisable()
        {
            SetInputEnable(false);
        }

        private void Update()
        {
            _sequencer.Tick();
        }

        // Called from UI
        [Preserve]
        public void SetInputEnable(bool isEnable)
        {
            foreach (InputAction action in AllActions)
            {
                if (isEnable)
                {
                    action.Enable();
                }
                else
                {
                    action.Disable();
                }
            }
        }

        private void OnTouchDrag(InputAction.CallbackContext ctx)
        {
            _lastPointerPosition = ctx.ReadValue<Vector2>();
            Raycast(false);
        }

        private void OnTouchDecide(InputAction.CallbackContext ctx)
        {
            Raycast(true);
        }

        private void Raycast(bool idTouchEnd)
        {
            // Raycast to streetscape geometries
            Ray ray = Camera.main.ScreenPointToRay(_lastPointerPosition);

            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                // No hit, assuming sky?
                _cursor.SetRaycastHitNone();
                if (idTouchEnd)
                {
                    OnTapEndNothing(ray);
                }
                return;
            }
            // else, found a RaycastHit

            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue);

            if (idTouchEnd)
            {
                _cursor.SetRaycastHitNone();
                AddInstrument(ref hit);
                RunHaptics();
            }
            else
            {
                _cursor.SetRaycastHit(ray, hit);
            }
        }

        private void OnKick(InputAction.CallbackContext ctx)
        {
            // Keep this for testing on Editor
            Ray ray = Camera.main.ScreenPointToRay(_lastPointerPosition);
            OnTapEndNothing(ray);
        }

        private void AddInstrument(ref RaycastHit hit)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var prefab = _instrumentPrefabs[_instruments.Count % _instrumentPrefabs.Length];
            var instrument = Instantiate(prefab, hit.point, rotation);
            instrument.transform.SetParent(hit.transform);
            instrument.Trigger(0.1);

            _instruments.Add(instrument);
            _sequencer.RegisterReceiver(instrument);
            Debug.Log($"Placed: {instrument.name}", instrument);
        }

        private void RunHaptics()
        {
            Handheld.Vibrate();
            // InputSystem haptics doesn't work?
            // StartCoroutine(RunHapticsInternal(seconds));
        }

        private static IEnumerator RunHapticsInternal(float seconds)
        {
            InputSystem.ResumeHaptics();
            yield return new WaitForSeconds(seconds);
            InputSystem.PauseHaptics();
        }

        private void OnTapEndNothing(Ray ray)
        {
            const float distance = 30f;
            Vector3 point = ray.GetPoint(distance);
            onTapAir.Invoke(point);

            Debug.Log($"OnTapEndNothing");

        }
    }
}
