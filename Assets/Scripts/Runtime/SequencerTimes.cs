namespace AugmentedInstrument
{
    using UnityEngine;

    public readonly struct SequencerTimes
    {
        public readonly double dspTime;
        public readonly double loudness;
        public readonly double sixteenthBeat;
        public readonly SixteenthBeat nextSixteenthBeat;
        public readonly double durationUntilNextSixteenthBeat;

        public SequencerTimes(
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
}
