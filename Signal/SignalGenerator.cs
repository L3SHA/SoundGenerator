using System;
using System.Collections.Generic;
using System.IO;
using SoundGenerator.Utils;

namespace SoundGenerator.Signal
{
    public class SignalGenerator
    {

        private static Random random = new Random();

        private delegate short GetSignalValueDelegate(double amplitude, double frequency, int step, double phase);

        private static Dictionary<SignalType, GetSignalValueDelegate> mapSignalTypesToGetSignalValueDelegates = new Dictionary<SignalType, GetSignalValueDelegate>
        {
            { SignalType.Sinusoid, GetSinusoidValue },
            { SignalType.Borehole, GetBoreholeValue },
            { SignalType.Triangle, GetTriangleValue },
            { SignalType.Sawtooth, GetSawtoothValue },
            { SignalType.Noise, GetNoiseValue }
        }; 

        public static MemoryStream GetSignalStream(MemoryStream waveFormatStream, List<SignalType> signalTypes, int durationInSeconds)
        {
            BinaryWriter waveWriter = new BinaryWriter(waveFormatStream);
            int samples = durationInSeconds * Constants.SamplesPerSecond;
            for (int i = 0; i < samples; i++)
            {
                short value = 0;
                foreach (var signalType in signalTypes)
                {
                    var getSignalValueDelegate = mapSignalTypesToGetSignalValueDelegates[signalType];
                    value = (short) (value + getSignalValueDelegate(1000, 500, i, 0));
                }
                waveWriter.Write(value);
            }
            waveFormatStream.Seek(0, SeekOrigin.Begin);
            return waveFormatStream;
        }

        private static short GetSinusoidValue(double amplitude, double frequency, int step, double phase)
        {
            return (short)(amplitude * Math.Sin(2 * Math.PI * frequency * step / Constants.SamplesPerSecond + phase));
        }

        private static short GetBoreholeValue(double amplitude, double frequency, int step, double phase)
        {
            var t = frequency * step / Constants.SamplesPerSecond + phase;
            var period = 1 / frequency;
            return period / (t % period) < 4 ? (short)amplitude :(short)-amplitude;
        }

        private static short GetTriangleValue(double amplitude, double frequency, int step, double phase)
        {
            var t = frequency * step / Constants.SamplesPerSecond + phase;
            return (short)(amplitude * (1f - 4f * Math.Abs(Math.Round(t - 0.25f) - (t - 0.25f))));
        }

        private static short GetSawtoothValue(double amplitude, double frequency, int step, double phase)
        {
            var t = frequency * step / Constants.SamplesPerSecond + phase;
            return (short)(amplitude * 2 * (t - (float)Math.Floor(t + 0.5f)));
        }

        private static short GetNoiseValue(double amplitude, double frequency, int step, double phase)
        {
            return (short)(amplitude * random.Next(2));
        }

    }
}
