﻿using SoundGenerator.Wave;
using System;
using System.Windows.Forms;
using SoundGenerator.Utils;
using System.IO;
using System.Media;
using SoundGenerator.Signal;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace SoundGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            string[] options = { "Sinusoid", "Borehole", "Triangle", "Sawtooth", "Noise" };
            var yPosition = 55;
            var xPosition = 400;
            var yDelta = 25;
            foreach (var option in options)
            {
                var signal = new CheckBox();
                signal.Location = new Point(xPosition, yPosition);
                signal.Size = new Size(80, 25);
                signal.Name = option;
                signal.Text = option;
                this.Controls.Add(signal);
                yPosition += yDelta;
            }
            txtDuration.Text = "5";
        }

        private void Play_Click(object sender, EventArgs e)
        {
            var mapStringsToSignalTypes = new Dictionary<string, SignalType>
            {
                {"Sinusoid", SignalType.Sinusoid},
                {"Borehole", SignalType.Borehole},
                {"Triangle", SignalType.Triangle},
                {"Sawtooth", SignalType.Sawtooth},
                {"Noise", SignalType.Noise}
            };
            var checkboxControls = this.Controls.OfType<CheckBox>().Where(c => c.Checked).ToList();
            List<SignalType> checkedSignalTypes = new List<SignalType>();
            foreach (var control in checkboxControls)
            {
                var signalType = mapStringsToSignalTypes[control.Name];
                checkedSignalTypes.Add(signalType);
            }
            
            var duration = int.Parse(txtDuration.Text);
            MemoryStream waveHeaderStream = WaveGenerator.GenerateWaveStream(duration);
            MemoryStream wavefile = SignalGenerator.GetSignalStream(waveHeaderStream, checkedSignalTypes, duration);
            SoundPlayer soundPlayer = new SoundPlayer(wavefile);
            soundPlayer.Play();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = this.CreateGraphics();
            Pen p = new Pen(Color.Red, 1);
            g.DrawRectangle(p, 100, 100, 1, 1);
        }
    }
}
