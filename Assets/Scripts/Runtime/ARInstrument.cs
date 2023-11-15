namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// An instrument that is instantiated in the AR world.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class ARInstrument : MonoBehaviour
    {
        [SerializeField]
        private SixteenthBeat _beat;

        [SerializeField]
        private ParticleSystem _particle;

        private const double _OFFSET_BEAT_BY_DISTANCE = 2.0; // 2meter

        private AudioSource _audioSource;

        private int _lastSixteenthBeat = -1;
        private StreetscapeGeometryInstrument _parentStreetscape;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _parentStreetscape = GetComponentInParent<StreetscapeGeometryInstrument>();
            Assert.IsNotNull(_parentStreetscape);
        }

        private void OnDestroy()
        {
            _audioSource = null;
            _parentStreetscape = null;
        }

        public void Tick(in SequencerTimes times, float distance)
        {
            // Offset beat by distance
            double offset = distance / _OFFSET_BEAT_BY_DISTANCE;
            int currentSixteenthBeat = (int)(times.sixteenthBeat + offset) % 16;

            if (currentSixteenthBeat == _lastSixteenthBeat)
            {
                return;
            }

            _lastSixteenthBeat = currentSixteenthBeat;

            if (!_beat.HasFlag(SequencerTimes.NextSixteenthBeat(currentSixteenthBeat)))
            {
                return;
            }

            double delay = times.durationUntilNextSixteenthBeat;
            // Offset speed of the sound in air: 346 m/s
            delay += distance / 346.0;

            Trigger(delay);
        }

        public void Trigger(double delay)
        {
            _audioSource.PlayScheduled(delay);

            if (_parentStreetscape != null)
            {
                _parentStreetscape.Trigger(this, delay);
            }

            if (_particle != null)
            {
                var main = _particle.main;
                main.startDelayMultiplier = (float)delay;
                _particle.Play();
            }
        }
    }
}
