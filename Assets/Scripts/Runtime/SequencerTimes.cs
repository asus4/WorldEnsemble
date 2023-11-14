namespace AugmentedInstrument
{
    using UnityEngine;

    public readonly struct SequencerTimes
    {
        public readonly double dspTime;
        public readonly double loudness;
        public readonly double sixteenthBeat;
        public readonly double durationUntilNextSixteenthBeat;


        public static SixteenthBeat NextSixteenthBeat(int currentSixteenthBeat)
        {
            int next = (currentSixteenthBeat + 1) % 16;
            return (SixteenthBeat)(1 << next);
        }

        public SequencerTimes(
            double dspTime,
            double loudness,
            double sixteenthBeat,
            double durationUntilNextSixteenthBeat
        )
        {
            this.dspTime = dspTime;
            this.loudness = loudness;
            this.sixteenthBeat = sixteenthBeat;
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
