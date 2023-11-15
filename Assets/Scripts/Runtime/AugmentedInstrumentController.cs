
namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Entry point for the Augmented Instrument Project!
    /// </summary>
    public sealed class AugmentedInstrumentController : MonoBehaviour
    {
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

        private Vector2 _lastPointerPosition;
        private RaycastCursor _cursor;
        private readonly RhythmSequencer _sequencer = RhythmSequencer.Instance;
        private readonly List<ARInstrument> _instruments = new();

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
            _sequencer.Start(_settings.BPM, audioListener);
        }

        private void OnDestroy()
        {
            _touchDragAction.performed -= OnTouchDrag;
            _touchDecideAction.performed -= OnTouchDecide;
            _kickAction.performed -= OnKick;
        }

        private void OnEnable()
        {
            foreach (InputAction action in AllActions)
            {
                action.Enable();
            }
        }

        private void OnDisable()
        {
            foreach (InputAction action in AllActions)
            {
                action.Disable();
            }
        }


        private void Update()
        {
            _sequencer.Tick();

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
                    OnTapEndNothing();
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
            OnTapEndNothing();
        }

        private void AddInstrument(ref RaycastHit hit)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            var prefab = _instrumentPrefabs[_instruments.Count % _instrumentPrefabs.Length];
            var instrument = Instantiate(prefab, hit.point, rotation);
            instrument.transform.SetParent(hit.transform);

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

        private void OnTapEndNothing()
        {
            Debug.Log($"OnTapEndNothing");
        }
    }
}
