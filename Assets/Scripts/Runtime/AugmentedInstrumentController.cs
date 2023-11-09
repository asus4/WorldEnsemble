
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
        private InputAction _touchAction;
        [SerializeField]
        private InputAction _kickAction;

        [Header("Prefabs")]
        [SerializeField]
        private ARInstrument _instrumentPrefab;

        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

        private readonly List<ARInstrument> _instruments = new();
        private double _startDspTime;


        private InputAction[] AllActions => new InputAction[]
        {
            _touchAction, _kickAction
        };

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _startDspTime = AudioSettings.dspTime;

            _touchAction.performed += OnTouch;
            _kickAction.performed += OnKick;
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

        private void OnTouch(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = ctx.ReadValue<Vector2>();
            Debug.Log($"OnTouch: {ctx}, pos: {screenPos}");

            // Raycast to the world
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Put instrument at: {hit}");
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 1f);

                // Put a instrument
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                var instrument = Instantiate(_instrumentPrefab, hit.point, rotation);
                instrument.transform.SetParent(hit.transform);

                _instruments.Add(instrument);
            }
        }

        private void OnKick(InputAction.CallbackContext ctx)
        {
            Debug.Log($"OnKick: {ctx}");
        }

    }
}
