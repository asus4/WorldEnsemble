
namespace AugmentedInstrument
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Entry point for the Augmented Instrument Project!
    /// </summary>
    public sealed class AugmentedInstrumentController : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField]
        private InputAction _touchDragAction;
        [SerializeField]
        private InputAction _touchDecideAction;
        [SerializeField]
        private InputAction _kickAction;

        [Header("Prefabs")]
        [SerializeField]
        private ARInstrument _instrumentPrefab;
        [SerializeField]
        private RaycastCursor _cursorPrefab;


        private Vector2 _lastPointerPosition;
        private RaycastCursor _cursor;
        private readonly RhythmMachine _rhythmMachine = RhythmMachine.Instance;

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
            _rhythmMachine.Start(120, audioListener);
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
            _rhythmMachine.Tick();

        }

        private void OnTouchDrag(InputAction.CallbackContext ctx)
        {
            _lastPointerPosition = ctx.ReadValue<Vector2>();
            Raycast(false);
        }

        private void OnTouchDecide(InputAction.CallbackContext ctx)
        {
            Debug.Log($"OnTouchDecide: {ctx}");
            Raycast(true);
        }

        private void Raycast(bool needPlacement)
        {
            // Raycast to streetscape geometries
            Ray ray = Camera.main.ScreenPointToRay(_lastPointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue);

                if (needPlacement)
                {
                    // Put a instrument
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    var instrument = Instantiate(_instrumentPrefab, hit.point, rotation);
                    instrument.transform.SetParent(hit.transform);

                    _rhythmMachine.RegisterInstrument(instrument);
                    _cursor.SetRaycastHitNone();

                    RunHaptics(0.1f);
                }
                else
                {
                    _cursor.SetRaycastHit(ray, hit);
                }
            }
            else
            {
                _cursor.SetRaycastHitNone();
            }
        }

        private void OnKick(InputAction.CallbackContext ctx)
        {
            // Keep this for testing on Editor
            Debug.Log($"OnKick: {ctx}");
        }

        private void RunHaptics(float seconds)
        {
            StartCoroutine(RunHapticsInternal(seconds));
        }

        private static IEnumerator RunHapticsInternal(float seconds)
        {
            InputSystem.ResumeHaptics();
            yield return new WaitForSeconds(seconds);
            InputSystem.PauseHaptics();
        }

    }
}
