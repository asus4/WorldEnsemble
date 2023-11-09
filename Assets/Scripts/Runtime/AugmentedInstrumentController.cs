
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

        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

        private readonly List<ARInstrument> _instruments = new();
        private double _startDspTime;
        private Vector2 _lastPointerPosition;
        private RaycastCursor _cursor;

        private InputAction[] AllActions => new InputAction[]
        {
            _touchDragAction,
            _touchDecideAction,
            _kickAction,
        };

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _startDspTime = AudioSettings.dspTime;

            _touchDragAction.performed += OnTouchDrag;
            _touchDecideAction.performed += OnTouchDecide;
            _kickAction.performed += OnKick;

            _cursor = Instantiate(_cursorPrefab);
            _cursor.transform.SetParent(transform);
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
            float dspTime = (float)(AudioSettings.dspTime - _startDspTime);
            Shader.SetGlobalFloat(_DspTimeID, dspTime);
        }

        private void OnTouchDrag(InputAction.CallbackContext ctx)
        {
            _lastPointerPosition = ctx.ReadValue<Vector2>();

            // Raycast to the world
            Ray ray = Camera.main.ScreenPointToRay(_lastPointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue, 0.05f);

                _cursor.SetRaycastHit(ray, hit);
                // Put a instrument
                // Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                // var instrument = Instantiate(_instrumentPrefab, hit.point, rotation);
                // instrument.transform.SetParent(hit.transform);

                // _instruments.Add(instrument);
            }
            else
            {
                _cursor.SetRaycastHitNone();
            }
        }

        private void OnTouchDecide(InputAction.CallbackContext ctx)
        {
            Debug.Log($"OnTouchDecide: {ctx}");

            // Raycast to the world
            Ray ray = Camera.main.ScreenPointToRay(_lastPointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Put instrument at: {hit}");
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1f);

                // Put a instrument
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                var instrument = Instantiate(_instrumentPrefab, hit.point, rotation);
                instrument.transform.SetParent(hit.transform);

                _instruments.Add(instrument);
            }
            _cursor.SetRaycastHitNone();
        }

        private void OnKick(InputAction.CallbackContext ctx)
        {
            Debug.Log($"OnKick: {ctx}");
        }

    }
}
