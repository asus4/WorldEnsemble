namespace WorldEnsemble
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

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

        public UnityEvent onArriveStart;
        public UnityEvent onArriveEnd;


        private static readonly int _FlyThroughWeight = Shader.PropertyToID("_FlyThroughWeight");
        private Camera _camera;
        private Transform _transform;
        private Coroutine _tweenCoroutine;
        private float _prevWeight = float.NegativeInfinity;

        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
            _transform = transform;
        }

        private void Update()
        {
            if (_prevWeight == _weight)
            {
                return;
            }

            Vector3 position = Vector3.Lerp(_targetStart.position, _targetEnd.position, _positionEasing.Evaluate(_weight));
            Quaternion rotation = Quaternion.Lerp(_targetStart.rotation, _targetEnd.rotation, _rotationEasing.Evaluate(_weight));
            _transform.SetPositionAndRotation(position, rotation);

            float materialWeight = _materialEasing.Evaluate(_weight);
            Shader.SetGlobalFloat(_FlyThroughWeight, materialWeight);

            _camera.enabled = materialWeight > 0.01f;

            _prevWeight = _weight;
        }

        public void FlyIn()
        {
            Animate(0, 1, 3);
        }

        public void FlyOut()
        {
            Animate(1, 0, 3);
        }

        public void FlyOutIn()
        {
            if (_tweenCoroutine != null)
            {
                StopCoroutine(_tweenCoroutine);
            }
            StartCoroutine(FlyOutInInternal());
        }

        private IEnumerator FlyOutInInternal()
        {
            yield return AnimateInternal(1, 0, 1.5f);
            yield return AnimateInternal(0, 1, 1.5f);
        }

        public void Animate(float from, float to, float duration)
        {
            if (_tweenCoroutine != null)
            {
                StopCoroutine(_tweenCoroutine);
            }
            StartCoroutine(AnimateInternal(from, to, duration));
        }

        private IEnumerator AnimateInternal(float from, float to, float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / duration;
                _weight = Mathf.Lerp(from, to, t);
                yield return new WaitForEndOfFrame();
            }

            _weight = to;
            if (to == 0)
            {
                onArriveStart?.Invoke();
            }
            else
            {
                onArriveEnd?.Invoke();
            }
        }
    }
}
