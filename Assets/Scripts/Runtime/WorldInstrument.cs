
namespace WorldInstrument
{
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// An instrument located in the AR world.
    /// </summary>
    public sealed class WorldInstrument : MonoBehaviour
    {
        [SerializeField]
        private AppSettings _settings;

        [SerializeField]
        private SixteenthBeat _beat;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private ParticleSystem _particle;

        private int _lastSixteenthBeat = -1;
        private StreetscapeGeometryInstrument _parentStreetscape;


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
            double offset = distance / _settings.offset16BeatByDistanceMeter;
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
            delay += distance / _settings.soundSpeedInAir;

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
