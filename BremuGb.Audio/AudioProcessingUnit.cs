﻿using System;
using System.Collections.Generic;

using BremuGb.Memory;
using BremuGb.Common.Constants;
using BremuGb.Audio.SoundChannels;

namespace BremuGb.Audio
{
    public class AudioProcessingUnit : IMemoryAccessDelegate
    {
        private SquareWaveSweepChannel _squareWaveSweepChannel;
        private SquareWaveChannel _squareWaveChannel;       
        private WaveChannel _waveChannel;
        private NoiseChannel _noiseChannel;

        private ISoundChannel[] _soundChannels;

        private FrameSequencer _frameSequencer;

        public AudioProcessingUnit()
        {
            _frameSequencer = new FrameSequencer();

            _squareWaveSweepChannel = new SquareWaveSweepChannel();
            _squareWaveChannel = new SquareWaveChannel();
            _waveChannel = new WaveChannel();
            _noiseChannel = new NoiseChannel();

            _soundChannels = new ISoundChannel[4];
            _soundChannels[0] = _squareWaveSweepChannel;
            _soundChannels[1] = _squareWaveChannel;
            _soundChannels[2] = _waveChannel;
            _soundChannels[3] = _noiseChannel;

            ChannelOutputSelect = 0xF3;
            SoundOnOff = 0x77;
        }

        private int _frameSequencerTimer = 0;

        private byte _channelOutputSelect;
        public byte ChannelOutputSelect 
        { 
            get => _channelOutputSelect;

            private set
            {
                if(_channelOutputSelect != value)
                {
                    _channelOutputSelect = value;
                    NotifyOutputTerminalChanged();
                }
            } 
        }
        
        public byte MasterVolume { get; private set; }
        public byte SoundOnOff 
        {
            get
            {
                return (byte)(_lastOnOffValue << 7 |
                                0x70 |
                                (_waveChannel.IsEnabled() ? 0x08 : 0x00) |
                                (_noiseChannel.IsEnabled() ? 0x04 : 0x00) |
                                (_squareWaveChannel.IsEnabled() ? 0x02 : 0x00) |
                                (_squareWaveSweepChannel.IsEnabled() ? 0x01 : 0x00));
            }

            private set
            {
                if ((value & 0x80) == 0x80)
                    _lastOnOffValue = 1;
                else
                {
                    _lastOnOffValue = 0;

                    foreach (var soundChannel in _soundChannels)
                        soundChannel.Disable();
                }                    
            }
        }

        private int _lastOnOffValue;

        public byte GetCurrentSample(Channels soundChannel)
        {
            switch(soundChannel)
            {
                case Channels.Channel1:
                    return _soundChannels[0].GetSample();
                case Channels.Channel2:
                    return _soundChannels[1].GetSample();
                case Channels.Channel3:
                    return _soundChannels[2].GetSample();
                case Channels.Channel4:
                    return _soundChannels[3].GetSample();
                default:
                    throw new InvalidOperationException("Invalid sound channel specified");
            }
        }

        public SoundOutputTerminal GetOutputTerminal(Channels soundChannel)
        {
            var output = SoundOutputTerminal.None;

            switch (soundChannel)
            {
                case Channels.Channel1:
                    if ((ChannelOutputSelect & 0x10) == 0x10)
                        output |= SoundOutputTerminal.Left;
                    if ((ChannelOutputSelect & 0x01) == 0x01)
                        output |= SoundOutputTerminal.Right;
                    break;

                case Channels.Channel2:
                    if ((ChannelOutputSelect & 0x20) == 0x20)
                        output |= SoundOutputTerminal.Left;
                    if ((ChannelOutputSelect & 0x02) == 0x02)
                        output |= SoundOutputTerminal.Right;
                    break;

                case Channels.Channel3:
                    if ((ChannelOutputSelect & 0x40) == 0x40)
                        output |= SoundOutputTerminal.Left;
                    if ((ChannelOutputSelect & 0x04) == 0x04)
                        output |= SoundOutputTerminal.Right;
                    break;

                case Channels.Channel4:
                    if ((ChannelOutputSelect & 0x80) == 0x80)
                        output |= SoundOutputTerminal.Left;
                    if ((ChannelOutputSelect & 0x08) == 0x08)
                        output |= SoundOutputTerminal.Right;
                    break;

                default:
                    throw new InvalidOperationException("Invalid sound channel specified");
            }

            return output;
        }

        public event EventHandler OutputTerminalChangedEvent;

        private void NotifyOutputTerminalChanged()
        {
            OutputTerminalChangedEvent?.Invoke(this, null);
        }

        public void AdvanceMachineCycle()
        {
            _frameSequencerTimer++;
            if(_frameSequencerTimer == 2048)
            {
                _frameSequencerTimer = 0;
                _frameSequencer.AdvanceClock(_soundChannels);
            }

            foreach(var soundChannel in _soundChannels)
            {
                soundChannel.AdvanceMachineCycle();
            }
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if (address >= 0xFF30 && address <= 0xFF3F)
                return _waveChannel.WaveTable[address - 0xFF30];

            return address switch
            {
                AudioRegisters.ChannelOutputSelect => ChannelOutputSelect,
                AudioRegisters.MasterVolume => MasterVolume,
                AudioRegisters.SoundOnOff => SoundOnOff,
                AudioRegisters.Channel1.DutyLength => _squareWaveSweepChannel.DutyLength,
                AudioRegisters.Channel1.Envelope => _squareWaveSweepChannel.Envelope,
                AudioRegisters.Channel1.FrequencyHi => _squareWaveSweepChannel.FrequencyHi,
                AudioRegisters.Channel1.FrequencyLo => _squareWaveSweepChannel.FrequencyLo,
                AudioRegisters.Channel1.Sweep => _squareWaveSweepChannel.Sweep,
                AudioRegisters.Channel2.DutyLength => _squareWaveChannel.DutyLength,
                AudioRegisters.Channel2.Envelope => _squareWaveChannel.Envelope,
                AudioRegisters.Channel2.FrequencyHi => _squareWaveChannel.FrequencyHi,
                AudioRegisters.Channel2.FrequencyLo => _squareWaveChannel.FrequencyLo,
                AudioRegisters.Channel3.OnOff => _waveChannel.OnOff,
                AudioRegisters.Channel3.OutputLevel => _waveChannel.OutputLevel,
                AudioRegisters.Channel3.FrequencyHi => _waveChannel.FrequencyHi,
                AudioRegisters.Channel3.FrequencyLo => _waveChannel.FrequencyLo,
                AudioRegisters.Channel3.SoundLength => _waveChannel.SoundLength,
                AudioRegisters.Channel4.Envelope => _noiseChannel.Envelope,
                AudioRegisters.Channel4.InitConsecutive => _noiseChannel.InitConsecutive,
                AudioRegisters.Channel4.PolynomialCounter => _noiseChannel.PolynomialCounter,
                AudioRegisters.Channel4.SoundLength => _noiseChannel.SoundLength,
                _ => throw new InvalidOperationException($"0x{address:X2} is not a valid sound address"),
            };
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            switch(address)
            {
                case AudioRegisters.ChannelOutputSelect:
                    ChannelOutputSelect = data;
                    break;
                case AudioRegisters.MasterVolume:
                    MasterVolume = data;
                    break;
                case AudioRegisters.SoundOnOff:
                    SoundOnOff = data;
                    break;
                case AudioRegisters.Channel1.DutyLength:
                    _squareWaveSweepChannel.DutyLength = data;
                    break;
                case AudioRegisters.Channel1.Envelope:
                    _squareWaveSweepChannel.Envelope = data;
                    break;
                case AudioRegisters.Channel1.FrequencyHi:
                    _squareWaveSweepChannel.FrequencyHi = data;
                    break;
                case AudioRegisters.Channel1.FrequencyLo:
                    _squareWaveSweepChannel.FrequencyLo = data;
                    break;
                case AudioRegisters.Channel1.Sweep:
                    _squareWaveSweepChannel.Sweep = data;
                    break;
                case AudioRegisters.Channel2.DutyLength:
                    _squareWaveChannel.DutyLength = data;
                    break;
                case AudioRegisters.Channel2.Envelope:
                    _squareWaveChannel.Envelope = data;
                    break;
                case AudioRegisters.Channel2.FrequencyHi:
                    _squareWaveChannel.FrequencyHi = data;
                    break;
                case AudioRegisters.Channel2.FrequencyLo:
                    _squareWaveChannel.FrequencyLo = data;
                    break;
                case AudioRegisters.Channel3.OnOff:
                    _waveChannel.OnOff = data;
                    break;
                case AudioRegisters.Channel3.OutputLevel:
                    _waveChannel.OutputLevel = data;
                    break;
                case AudioRegisters.Channel3.FrequencyHi:
                    _waveChannel.FrequencyHi = data;
                    break;
                case AudioRegisters.Channel3.FrequencyLo:
                    _waveChannel.FrequencyLo = data;
                    break;
                case AudioRegisters.Channel3.SoundLength:
                    _waveChannel.SoundLength = data;
                    break;
                case AudioRegisters.Channel4.Envelope:
                    _noiseChannel.Envelope = data;
                    break;
                case AudioRegisters.Channel4.InitConsecutive:
                    _noiseChannel.InitConsecutive = data;
                    break;
                case AudioRegisters.Channel4.PolynomialCounter:
                    _noiseChannel.PolynomialCounter = data;
                    break;
                case AudioRegisters.Channel4.SoundLength:
                    _noiseChannel.SoundLength = data;
                    break;
            }

            if (address >= 0xFF30 && address <= 0xFF3F)
                _waveChannel.WaveTable[address - 0xFF30] = data;
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            var addressList = new List<ushort>();          

            var audioRegisterAddresses = new ushort[] { AudioRegisters.ChannelOutputSelect,
                                                        AudioRegisters.MasterVolume,
                                                        AudioRegisters.SoundOnOff,
                                                        AudioRegisters.Channel1.DutyLength,
                                                        AudioRegisters.Channel1.Envelope,
                                                        AudioRegisters.Channel1.FrequencyHi,
                                                        AudioRegisters.Channel1.FrequencyLo,
                                                        AudioRegisters.Channel1.Sweep,
                                                        AudioRegisters.Channel2.DutyLength,
                                                        AudioRegisters.Channel2.Envelope,
                                                        AudioRegisters.Channel2.FrequencyHi,
                                                        AudioRegisters.Channel2.FrequencyLo,
                                                        AudioRegisters.Channel3.OnOff,
                                                        AudioRegisters.Channel3.OutputLevel,
                                                        AudioRegisters.Channel3.FrequencyHi,
                                                        AudioRegisters.Channel3.FrequencyLo,
                                                        AudioRegisters.Channel3.SoundLength,
                                                        AudioRegisters.Channel4.Envelope,
                                                        AudioRegisters.Channel4.InitConsecutive,
                                                        AudioRegisters.Channel4.PolynomialCounter,
                                                        AudioRegisters.Channel4.SoundLength};            

            var waveRamAddresses = new ushort[0x10];
            for (int i = 0; i < waveRamAddresses.Length; i++)
                waveRamAddresses[i] = (ushort)(i + 0xFF30);
            
            addressList.AddRange(audioRegisterAddresses);
            addressList.AddRange(waveRamAddresses);

            return addressList;
        }
    }
}
