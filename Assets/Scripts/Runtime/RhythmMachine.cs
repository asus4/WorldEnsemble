namespace AugmentedInstrument
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Global rhythm machine.
    /// </summary>
    public sealed class RhythmMachine
    {
        public readonly struct SequencerParameters
        {
            public readonly double dspTime;
            public readonly double loudness;
            public readonly double sixteenthBeat;
            public readonly SixteenthBeat nextSixteenthBeat;
            public readonly double durationUntilNextSixteenthBeat;

            public SequencerParameters(
                double dspTime,
                double loudness,
                double sixteenthBeat,
                SixteenthBeat nextSixteenthBeat,
                double durationUntilNextSixteenthBeat
            )
            {
                this.dspTime = dspTime;
                this.loudness = loudness;
                this.sixteenthBeat = sixteenthBeat;
                this.nextSixteenthBeat = nextSixteenthBeat;
                this.durationUntilNextSixteenthBeat = durationUntilNextSixteenthBeat;
            }

            public readonly Vector4 AsVector4 =>
                new(
                    (float)dspTime,
                    (float)loudness,
                    (float)sixteenthBeat,
                    (float)durationUntilNextSixteenthBeat
                );
        }

        public static RhythmMachine Instance { get; } = new RhythmMachine();

        private double _bpm;
        private double _quarterNoteDuration;

        private AudioListener _audioListener;
        private Transform _audioListenerTransform;

        private double _startDspTime;
        private readonly float[] _outputSamples = new float[256];
        private readonly List<ARInstrument> _instruments = new();

        public double BarDuration => _quarterNoteDuration * 4.0;
        public double QuarterNoteDuration => _quarterNoteDuration;
        public double SixteenthNoteDuration => _quarterNoteDuration / 4.0;


        public double DspTime => AudioSettings.dspTime - _startDspTime;

        // DSP time for shader
        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

        public void Start(double bpm, AudioListener audioListener)
        {
            _audioListener = audioListener;
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

            SequencerParameters times = new(
                dspTime: dspTime,
                loudness: loudness,
                sixteenthBeat: sixteenthBeat,
                nextSixteenthBeat: (SixteenthBeat)(1 << ((int)sixteenthBeat + 1) % 16),
                durationUntilNextSixteenthBeat: sixteenthNoteDuration - (dspTime % sixteenthNoteDuration)
            );

            // Send to global shader value
            Shader.SetGlobalVector(_DspTimeID, times.AsVector4);

            foreach (var instrument in _instruments)
            {
                instrument.Tick(times);
            }

            // Debug.Log($"bar: {totalBars:F2}, 4: {quarterBeat:F2}, 16: {sixteenthBeat:F2}, next: {nextSixteenthBeat}, delay: {delay:F2}");
        }

        public void RegisterInstrument(ARInstrument instrument)
        {
            _instruments.Add(instrument);
        }

    }
}
