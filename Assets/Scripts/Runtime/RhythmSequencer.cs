namespace WorldInstrument
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Global rhythm machine.
    /// </summary>
    public sealed class RhythmSequencer : System.IDisposable
    {
        private double _quarterNoteDuration;

        private Transform _audioListenerTransform;

        private double _startDspTime;

        private readonly List<WorldInstrument> _instruments = new();

        public double DspTime => AudioSettings.dspTime - _startDspTime;
        public double SixteenthNoteDuration => _quarterNoteDuration / 4.0;


        // DSP time for shader
        private static readonly int _SequencerTimesID = Shader.PropertyToID("_SequencerTimes");

        public void Start(double bpm, AudioListener audioListener)
        {
            _audioListenerTransform = audioListener.transform;

            _startDspTime = AudioSettings.dspTime;
            _quarterNoteDuration = 60.0 / bpm;

            // Output debug info
            // Delay effect time should be matched with these values
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Start Rhythm Sequencer:");
            sb.AppendLine($"- BPM: {bpm}");
            sb.AppendLine($"- 1/4 Note Duration: {_quarterNoteDuration} sec, {_quarterNoteDuration * 1000} ms");
            sb.AppendLine($"- 1/16 Note Duration: {SixteenthNoteDuration} sec, {SixteenthNoteDuration * 1000} ms");
            Debug.Log(sb.ToString());
        }

        public void Dispose()
        {
            _instruments.Clear();
        }

        public void Tick()
        {
            double dspTime = DspTime;
            double sixteenthNoteDuration = SixteenthNoteDuration;
            double sixteenthBeat = dspTime / sixteenthNoteDuration % 16.0;

            float loudness = LoudnessMeter.GetLoudnessNormalized();

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

        public void RegisterReceiver(WorldInstrument instrument)
        {
            _instruments.Add(instrument);
        }
    }
}
