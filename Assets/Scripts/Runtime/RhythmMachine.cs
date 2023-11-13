namespace AugmentedInstrument
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Global rhythm machine.
    /// </summary>
    public sealed class RhythmMachine
    {
        public static RhythmMachine Instance { get; } = new RhythmMachine();

        private double _bpm;
        private double _quarterNoteDuration;

        private AudioListener _audioListener;
        private Transform _audioListenerTransform;

        private double _startDspTime;
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
            double totalBars = dspTime / BarDuration;
            double quarterBeat = dspTime / QuarterNoteDuration % 4.0;
            double sixteenthBeat = dspTime / SixteenthNoteDuration % 16.0;

            // Send to global shader value
            Shader.SetGlobalVector(_DspTimeID, new Vector4(
                (float)dspTime,
                (float)totalBars,
                (float)quarterBeat,
                (float)sixteenthBeat
            ));

            SixteenthBeat nextSixteenthBeat = (SixteenthBeat)(1 << ((int)sixteenthBeat + 1) % 16);
            double delay = SixteenthNoteDuration - (dspTime % SixteenthNoteDuration);

            Debug.Log($"bar: {totalBars:F2}, 4: {quarterBeat:F2}, 16: {sixteenthBeat:F2}, next: {nextSixteenthBeat}, delay: {delay:F2}");

            foreach (var instrument in _instruments)
            {
                if (instrument.BeatMask.HasFlag(nextSixteenthBeat))
                {
                    instrument.PlaySound(delay);
                }
            }
        }

        public void RegisterInstrument(ARInstrument instrument)
        {
            _instruments.Add(instrument);
        }

        public void PlayAll()
        {
            foreach (ARInstrument instrument in _instruments)
            {
                instrument.PlaySound();
            }
        }

    }
}
