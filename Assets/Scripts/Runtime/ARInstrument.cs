

namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// An instrument that is instantiated in the AR world.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class ARInstrument : MonoBehaviour, ISequencerListener
    {
        [SerializeField]
        private SixteenthBeat _beat;

        [SerializeField]
        private ParticleSystem _particle;

        public SixteenthBeat BeatMask => _beat;

        private AudioSource _audioSource;

        private int _lastSixteenthBeat = -1;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Tick(in SequencerTimes times)
        {
            int currentSixteenthBeat = (int)times.sixteenthBeat;
            if (currentSixteenthBeat == _lastSixteenthBeat)
            {
                return;
            }

            _lastSixteenthBeat = currentSixteenthBeat;

            if (!BeatMask.HasFlag(times.nextSixteenthBeat))
            {
                return;
            }

            double delay = times.durationUntilNextSixteenthBeat;

            // Debug.Log($"PlaySound: {gameObject.name}, delay: {delay:F2}");
            _audioSource.PlayScheduled(delay);

            if (_particle != null)
            {
                var main = _particle.main;
                main.startDelayMultiplier = (float)delay;
                _particle.Play();
            }
        }
    }
}
