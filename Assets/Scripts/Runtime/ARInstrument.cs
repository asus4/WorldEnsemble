

namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

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

        public SixteenthBeat BeatMask => _beat;

        private AudioSource _audioSource;
        private RhythmMachine _rhythmMachine = RhythmMachine.Instance;

        private double _lastPlayedDspTime = -1.0;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(double delay = 0.0)
        {
            if (_rhythmMachine.DspTime - _lastPlayedDspTime < _rhythmMachine.SixteenthNoteDuration)
            {
                return;
            }
            Debug.Log($"PlaySound: {gameObject.name}, delay: {delay:F2}");
            _audioSource.PlayScheduled(delay);
            _lastPlayedDspTime = _rhythmMachine.DspTime + delay;

            if (_particle != null)
            {
                var main = _particle.main;
                main.startDelayMultiplier = (float)delay;
                _particle.Play();
            }
        }
    }
}
