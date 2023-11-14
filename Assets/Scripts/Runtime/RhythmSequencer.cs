namespace AugmentedInstrument
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface ISequencerListener
    {
        void Tick(in SequencerTimes times);
    }

    /// <summary>
    /// Global rhythm machine.
    /// </summary>
    public sealed class RhythmSequencer
    {
        public static RhythmSequencer Instance { get; } = new RhythmSequencer();

        private double _bpm;
        private double _quarterNoteDuration;

        private Transform _audioListenerTransform;

        private double _startDspTime;
        private readonly float[] _outputSamples = new float[256];
        private readonly List<ISequencerListener> _receivers = new();

        public double BarDuration => _quarterNoteDuration * 4.0;
        public double QuarterNoteDuration => _quarterNoteDuration;
        public double SixteenthNoteDuration => _quarterNoteDuration / 4.0;


        public double DspTime => AudioSettings.dspTime - _startDspTime;

        // DSP time for shader
        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

        public void Start(double bpm, AudioListener audioListener)
        {
            _audioListenerTransform = audioListener.transform;

            _bpm = bpm;
            _startDspTime = AudioSettings.dspTime;
            _quarterNoteDuration = 60.0 / bpm;
        }

        public void Tick()
        {
            double dspTime = DspTime;
            double sixteenthNoteDuration = SixteenthNoteDuration;
            double sixteenthBeat = dspTime / sixteenthNoteDuration % 16.0;

            AudioListener.GetOutputData(_outputSamples, 0);
            float loudness = _outputSamples.Sum(x => Mathf.Abs(x)) / _outputSamples.Length;

            SequencerTimes times = new(
                dspTime: dspTime,
                loudness: loudness,
                sixteenthBeat: sixteenthBeat,
                nextSixteenthBeat: (SixteenthBeat)(1 << ((int)sixteenthBeat + 1) % 16),
                durationUntilNextSixteenthBeat: sixteenthNoteDuration - (dspTime % sixteenthNoteDuration)
            );

            // Send to global shader value
            Shader.SetGlobalVector(_DspTimeID, times.AsVector4);

            foreach (var instrument in _receivers)
            {
                instrument.Tick(times);
            }

            // Debug.Log($"bar: {totalBars:F2}, 4: {quarterBeat:F2}, 16: {sixteenthBeat:F2}, next: {nextSixteenthBeat}, delay: {delay:F2}");
        }

        public void RegisterReceiver(ISequencerListener receiver)
        {
            _receivers.Add(receiver);
        }

    }
}
