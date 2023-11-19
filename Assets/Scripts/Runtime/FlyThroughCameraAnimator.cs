namespace WorldInstrument
{
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public sealed class FlyThroughCameraAnimator : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetStart;

        [SerializeField]
        private Transform _targetEnd;

        [SerializeField]
        private AnimationCurve _positionEasing;

        [SerializeField]
        private AnimationCurve _rotationEasing;

        [SerializeField]
        private AnimationCurve _materialEasing;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _weight = 1.0f;

        private Camera _camera;
        private Transform _transform;
        private static readonly int _FlyThroughWeight = Shader.PropertyToID("_FlyThroughWeight");

        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
            _transform = transform;
        }

        private void Update()
        {
            Vector3 position = Vector3.Lerp(_targetStart.position, _targetEnd.position, _positionEasing.Evaluate(_weight));
            Quaternion rotation = Quaternion.Lerp(_targetStart.rotation, _targetEnd.rotation, _rotationEasing.Evaluate(_weight));
            _transform.SetPositionAndRotation(position, rotation);

            float materialWeight = _materialEasing.Evaluate(_weight);
            Shader.SetGlobalFloat(_FlyThroughWeight, materialWeight);

            _camera.enabled = materialWeight > 0.01f;
        }
    }
}
