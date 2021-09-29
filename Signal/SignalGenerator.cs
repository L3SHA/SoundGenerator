using System;
using System.Collections.Generic;
using System.IO;
using SoundGenerator.Utils;

namespace SoundGenerator.Signal
{
    public class SignalGenerator
    {

        private static Random random = new Random();

        private delegate short GetModulationValueDelegate(double amplitude, double frequency, int step, double phase);

        private delegate short GetSignalValueDelegate(double amplitude, double anglevelocity, double phase);

        private static Dictionary<SignalType, GetSignalValueDelegate> mapSignalTypesToGetSignalValueDelegates = new Dictionary<SignalType, GetSignalValueDelegate>
        {
            { SignalType.Sinusoid, GetSinusoidValue },
            { SignalType.Borehole, GetBoreholeValue },
            { SignalType.Triangle, GetTriangleValue },
            { SignalType.Sawtooth, GetSawtoothValue },
            { SignalType.Noise, GetNoiseValue }
        };

        private static Dictionary<SignalType, GetModulationValueDelegate> mapSignalTypesToGetModulationValueDelegates = new Dictionary<SignalType, GetModulationValueDelegate>
        {
            { SignalType.Sinusoid, GetSinusoidModulationValue },
            { SignalType.Borehole, GetBoreholeModulationValue },
            { SignalType.Triangle, GetTriangleModulationValue },
            { SignalType.Sawtooth, GetSawtoothModulationValue },
            { SignalType.Noise, GetNoiseModulationValue }
        }; 

        public static MemoryStream GetSignalStream(MemoryStream waveFormatStream, List<SignalType> signalTypes, SignalType modulationType, int durationInSeconds, (bool amplitude, bool frequency) modulationSettings)
        {
            BinaryWriter waveWriter = new BinaryWriter(waveFormatStream);
            int samples = durationInSeconds * Constants.SamplesPerSecond;
            double angularVelocity = 0;
            double amplitude = 1000;
            //double angle = 0;
            double frequency = 0;
            for (int i = 0; i < samples; i++)
            {
                short value = 0;
                foreach (var signalType in signalTypes)
                {
                    if (modulationSettings.amplitude)
                    {
                        var modulationValueDelegate = mapSignalTypesToGetModulationValueDelegates[modulationType];
                        amplitude = modulationValueDelegate(1000, 4, i, 0);
                    }
                    if (modulationSettings.frequency)
                    {
                        var modulationValueDelegate = mapSignalTypesToGetModulationValueDelegates[modulationType];
                        frequency = modulationValueDelegate(1000, 4, i, 0);
                        angularVelocity += 2 * Math.PI * frequency / Constants.SamplesPerSecond;
                    }
                    else
                    {
                        angularVelocity = 2 * Math.PI * 220 * i / Constants.SamplesPerSecond;
                    }

                    var getSignalValueDelegate = mapSignalTypesToGetSignalValueDelegates[signalType];

                    //var getSignalValueDelegate = mapSignalTypesToGetModulationValueDelegates[signalType];
                    value = (short) (value + getSignalValueDelegate(amplitude, angularVelocity, 0));
                }
                /*double frequency = (Math.Sin(2 * Math.PI * i / Constants.SamplesPerSecond) + 0.5);
                angle += (short)(2 * Math.PI * frequency / Constants.SamplesPerSecond);
                //var getSignalValueDelegate = mapSignalTypesToGetSignalValueDelegates[signalType];
                short value = (short)(10000 * Math.Sin(angle + 0));//(short) (value + getSignalValueDelegate(1000, 260, i, GetValue(1000, 20, i, 0)));*/
                waveWriter.Write(value);
            }
            waveFormatStream.Seek(0, SeekOrigin.Begin);
            return waveFormatStream;
        }

        private static short GetSinusoidModulationValue(double amplitude, double frequency, int step, double phase)
        {
            return (short)(amplitude * Math.Sin(2 * Math.PI * frequency * step / Constants.SamplesPerSecond + phase));
        }

        private static short GetBoreholeModulationValue(double amplitude, double frequency, int step, double phase)
        {
            var t = frequency * step / Constants.SamplesPerSecond + phase;
            var period = 1 / frequency;
            return period / (t % period) < 4 ? (short)amplitude :(short)-amplitude;
        }

        private static short GetTriangleModulationValue(double amplitude, double frequency, int step, double phase)
        {
            var t = frequency * step / Constants.SamplesPerSecond + phase;
            return (short)(amplitude * (1f - 4f * Math.Abs(Math.Round(t - 0.25f) - (t - 0.25f))));
        }

        private static short GetSawtoothModulationValue(double amplitude, double frequency, int step, double phase)
        {
            var t = frequency * step / Constants.SamplesPerSecond + phase;
            return (short)(amplitude * 2 * (t - (float)Math.Floor(t + 0.5f)));
        }

        private static short GetNoiseModulationValue(double amplitude, double frequency, int step, double phase)
        {
            return (short)(amplitude * random.Next(2));
        }

        private static short GetSinusoidValue(double amplitude, double angleVelocity, double phase)
        {
            return (short)(amplitude * Math.Sin(angleVelocity + phase));
        }

        private static short GetBoreholeValue(double amplitude, double angleVelocity, double phase)
        {
            return (Math.Abs(Math.Sin(angleVelocity + phase)) < 1.0 - 10E-3) ? (short)amplitude : (short)-amplitude;
        }

        private static short GetTriangleValue(double amplitude, double angleVelocity, double phase)
        {
            return (short)(amplitude * (1f - 4f * Math.Abs(Math.Round(angleVelocity - 0.25f) - (angleVelocity - 0.25f))));
        }

        private static short GetSawtoothValue(double amplitude, double angleVelocity, double phase)
        {
            return (short)(amplitude * 2 * (angleVelocity - (float)Math.Floor(angleVelocity + 0.5f)));
        }

        private static short GetNoiseValue(double amplitude, double anglevelocity, double phase)
        {
            return (short)(amplitude * random.Next(2));
        }

    }
}
