using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using BremuGb.Serial;

using NUnit.Framework;

namespace BremuGb.IntegrationTests
{
    internal class TestRomRunner
    {
        private GameBoy _gameBoy;

        private List<byte> _serialData;

        internal TestRomRunner(string romPath)
        {
            _gameBoy = new GameBoy(romPath);
            _gameBoy.SerialDataTransferedEvent += new EventHandler<SerialEventArgs>(OnSerialDataTransfered);

            _serialData = new List<byte>();
        }

        internal void Run(int cycleCount)
        {
            var completedCycles = 0;
            while (completedCycles++ < cycleCount)
                _gameBoy.AdvanceMachineCycle(Input.JoypadState.None);
        }

        internal void RunUntilSerialResponse(int timeoutCycles)
        {
            var completedCycles = 0;
            var serialDataReceived = false;

            while (completedCycles++ < timeoutCycles)
            {
                _gameBoy.AdvanceMachineCycle(Input.JoypadState.None);

                //once serial data is received, run for a few more cycles to receive all data
                if (_serialData.Count > 0 && !serialDataReceived)
                {
                    serialDataReceived = true;

                    completedCycles = 0;
                    timeoutCycles = 20000;
                }
            }
        }

        internal string GetSentSerialDataAsString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var serialByte in _serialData)
                stringBuilder.Append(Convert.ToChar(serialByte));

            return stringBuilder.ToString();
        }

        internal string GetBlarggTestResultFromMemory()
        {
            var stringBuilder = new StringBuilder();

            ushort address = 0xA004;
            bool endOfStringReached = false;

            while (!endOfStringReached)
            {
                var nextByte = _gameBoy.MemoryRead(address++);
                if (nextByte == 0x00)
                    endOfStringReached = true;
                else
                    stringBuilder.Append(Convert.ToChar(nextByte));
            }

            return stringBuilder.ToString();
        }

        internal void AssertGekkioTestResult()
        {
            //gekkio's tests send a few magic numbers via serial in case of success
            Assert.AreEqual(6, _serialData.Count);
            Assert.AreEqual(3, _serialData[0]);
            Assert.AreEqual(5, _serialData[1]);
            Assert.AreEqual(8, _serialData[2]);
            Assert.AreEqual(13, _serialData[3]);
            Assert.AreEqual(21, _serialData[4]);
            Assert.AreEqual(34, _serialData[5]);
        }

        internal void AssertScreen(string pathToExpectedScreen)
        { 
            var expectedImage = Image.Load<Rgb24>(pathToExpectedScreen);

            Assert.True(expectedImage.TryGetSinglePixelSpan(out var span));
            var array = MemoryMarshal.AsBytes(span).ToArray();

            var actualScreen = _gameBoy.GetScreen();

            for (int i = 0; i < array.Length; i++)
                Assert.AreEqual(array[i], actualScreen[i]);
        }

        private void OnSerialDataTransfered(object sender, SerialEventArgs e)
        {
            _serialData.Add(e.SerialData);
        }
    }
}
