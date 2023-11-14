namespace AugmentedInstrument
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Global rhythm machine.
    /// </summary>
    public sealed class RhythmSequencer
    {
        private static class LoudnessMeter
        {
            private static readonly float[] _outputSamples = new float[512];
            // TODO: should replace with circular buffer
            private const int HISTORY_COUNT = 128;
            private static readonly Queue<float> _history = new(HISTORY_COUNT); // roughly 2 seconds

            public static float GetLoudness()
            {
                AudioListener.GetOutputData(_outputSamples, 0);
                float loudness = _outputSamples.Sum(x => Mathf.Abs(x)) / _outputSamples.Length;

                _history.Enqueue(loudness);
                if (_history.Count > HISTORY_COUNT)
                {
                    _history.Dequeue();
                }
                // Normalize into 0.0 - 1.0
                float min = float.MaxValue;
                float max = float.MinValue;
                foreach (float value in _history)
                {
                    min = Mathf.Min(min, value);
                    max = Mathf.Max(max, value);
                }
                return Mathf.InverseLerp(min, max, loudness);
            }
        }

        public static RhythmSequencer Instance { get; } = new RhythmSequencer();

        private double _quarterNoteDuration;

        private Transform _audioListenerTransform;

        private double _startDspTime;

        private readonly List<ARInstrument> _instruments = new();


        public double DspTime => AudioSettings.dspTime - _startDspTime;
        public double SixteenthNoteDuration => _quarterNoteDuration / 4.0;


        // DSP time for shader
        private static readonly int _SequencerTimesID = Shader.PropertyToID("_SequencerTimes");

        public void Start(double bpm, AudioListener audioListener)
        {
            _audioListenerTransform = audioListener.transform;

            _startDspTime = AudioSettings.dspTime;
            _quarterNoteDuration = 60.0 / bpm;
        }

        public void Tick()
        {
            double dspTime = DspTime;
            double sixteenthNoteDuration = SixteenthNoteDuration;
            double sixteenthBeat = dspTime / sixteenthNoteDuration % 16.0;

            float loudness = LoudnessMeter.GetLoudness();

            SequencerTimes times = new(
                dspTime: dspTime,
                loudness: loudness,
                sixteenthBeat: sixteenthBeat,
                durationUntilNextSixteenthBeat: sixteenthNoteDuration - (dspTime % sixteenthNoteDuration)
            );

            // Send to global shader value
            Shader.SetGlobalVector(_SequencerTimesID, times.AsVector4);

            foreach (var instrument in _instruments)
            {
                float distance = Vector3.Distance(
                    instrument.transform.position, _audioListenerTransform.position);
                instrument.Tick(times, distance);
            }

            // Debug.Log($"bar: {totalBars:F2}, 4: {quarterBeat:F2}, 16: {sixteenthBeat:F2}, next: {nextSixteenthBeat}, delay: {delay:F2}");
        }

        public void RegisterReceiver(ARInstrument instrument)
        {
            _instruments.Add(instrument);
        }
    }
}
