
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
        private InputAction _touchAction;
        [SerializeField]
        private InputAction _kickAction;

        [SerializeField]
        private GameObject _debugInstrumentPrefab;

        private double _startDspTime;

        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

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
                Debug.Log($"Hit: {hit}");
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 1f);
                // hit.transform.GetComponent<Instrument>().Play();
                // Put a debug instrument
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                var go = Instantiate(_debugInstrumentPrefab, hit.point, rotation);
                go.transform.SetParent(hit.transform);
            }
        }

        private void OnKick(InputAction.CallbackContext ctx)
        {
            Debug.Log($"OnKick: {ctx}");
        }

    }
}
