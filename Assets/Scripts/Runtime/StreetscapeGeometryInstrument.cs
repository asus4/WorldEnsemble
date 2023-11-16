namespace WorldInstrument
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public sealed class StreetscapeGeometryInstrument : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve _triggerCurve;

        private static readonly int _InstrumentPositionID = Shader.PropertyToID("_InstrumentPosition");
        private static readonly int _InstrumentNormalID = Shader.PropertyToID("_InstrumentNormal");
        private static readonly int _InstrumentTriggerID = Shader.PropertyToID("_InstrumentTrigger");

        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;
        private Coroutine _coroutine;
        private float _triggerDuration;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();

            _triggerDuration = _triggerCurve.keys[_triggerCurve.length - 1].time;
        }

        private void OnDestroy()
        {
            _mpb.Clear();
        }

        public void Trigger(WorldInstrument instrument, double delay)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(TriggerAsync(instrument, (float)delay));
        }

        private IEnumerator TriggerAsync(WorldInstrument instrument, float delay)
        {
            yield return new WaitForSeconds(delay);

            _mpb.SetVector(_InstrumentPositionID, instrument.transform.position);
            _mpb.SetVector(_InstrumentNormalID, instrument.transform.up);
            _mpb.SetFloat(_InstrumentTriggerID, 1.0f);
            _renderer.SetPropertyBlock(_mpb);

            float startTime = Time.time;
            while (true)
            {
                float time = Time.time - startTime;
                if (time >= _triggerDuration)
                {
                    break;
                }
                _mpb.SetFloat(_InstrumentTriggerID, _triggerCurve.Evaluate(time));
                _renderer.SetPropertyBlock(_mpb);
                yield return new WaitForEndOfFrame();
            }
            ResetTrigger();
        }

        private void ResetTrigger()
        {
            _mpb.SetFloat(_InstrumentTriggerID, 0.0f);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}
