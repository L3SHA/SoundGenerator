namespace SoundGenerator.Utils
{
    public enum SignalType
    {
        Sinusoid,
        Borehole,
        Triangle,
        Sawtooth,
        Noise,
        Polygarmonic
    }
    public static class Constants
    {
        public const int SamplesPerSecond = 44100;
    }
}
