using System;
using System.Collections.Generic;

using BremuGb.Memory;
using BremuGb.Common.Constants;
using BremuGb.Audio.SoundChannels;

namespace BremuGb.Audio
{
    public class AudioProcessingUnit : IMemoryAccessDelegate
    {
        public event EventHandler OutputTerminalChangedEvent;
        public event EventHandler MasterVolumeChangedEvent;

        private SquareWaveSweepChannel _squareWaveSweepChannel;
        private SquareWaveChannel _squareWaveChannel;       
        private WaveChannel _waveChannel;
        private NoiseChannel _noiseChannel;

        private ISoundChannel[] _soundChannels;

        private FrameSequencer _frameSequencer;

        private List<ushort> _unusedAudioAddresses;

        private int _lastOnOffValue = 1;
        private int _frameSequencerTimer = 0;
        private int _frameSequencerPeriod = 2048;

        private int _masterVolumeLeft;
        private int _masterVolumeRight;
        private bool _outputVinLeft;
        private bool _outputVinRight;

        private byte _channelOutputSelect;
        private byte _oldMasterVolume;

        private byte ChannelOutputSelect
        {
            get => _channelOutputSelect;

            set
            {
                if (_channelOutputSelect != value)
                {
                    _channelOutputSelect = value;
                    NotifyOutputTerminalChanged();
                }
            }
        }        

        private byte MasterVolume 
        { 
            get
            {
                return (byte)((_outputVinLeft ? 0x1 : 0x0) << 7 |
                              (_masterVolumeLeft << 4) |
                              (_outputVinRight ? 0x1 : 0x0) << 3 |
                               _masterVolumeRight);
            }

            set
            {
                if (value == _oldMasterVolume)
                    return;
                
                _oldMasterVolume = value;

                _outputVinLeft = (value & 0x80) == 0x80;
                _outputVinRight = (value & 0x08) == 0x08;
                _masterVolumeLeft = (value & 0x70) >> 4;
                _masterVolumeRight = value & 0x07;

                NotifyMasterVolumeChanged();
            }
        }

        private byte SoundOnOff
        {
            get => (byte)(_lastOnOffValue << 7 |
                          0x70 |
                         (_noiseChannel.IsEnabled() ? 0x08 : 0x00) |
                         (_waveChannel.IsEnabled() ? 0x04 : 0x00) |
                         (_squareWaveChannel.IsEnabled() ? 0x02 : 0x00) |
                         (_squareWaveSweepChannel.IsEnabled() ? 0x01 : 0x00));

            set
            {
                if ((value & 0x80) == 0x80)
                {
                    if (_lastOnOffValue == 0)
                        _frameSequencer.ResetAndEnable();

                    _lastOnOffValue = 1;
                }
                else
                {
                    _frameSequencer.Disable();

                    foreach (var soundChannel in _soundChannels)
                        soundChannel.Disable();

                    ChannelOutputSelect = 0;
                    MasterVolume = 0;

                    _lastOnOffValue = 0;
                }
            }
        }

        public AudioProcessingUnit()
        {
            _frameSequencer = new FrameSequencer();
            _frameSequencer.ResetAndEnable();

            _squareWaveSweepChannel = new SquareWaveSweepChannel();
            _squareWaveChannel = new SquareWaveChannel();
            _waveChannel = new WaveChannel();
            _noiseChannel = new NoiseChannel();

            _soundChannels = new ISoundChannel[4];
            _soundChannels[0] = _squareWaveSweepChannel;
            _soundChannels[1] = _squareWaveChannel;
            _soundChannels[2] = _waveChannel;
            _soundChannels[3] = _noiseChannel;

            //initial values after boot
            ChannelOutputSelect = 0xF3;
            SoundOnOff = 0xF1;
            MasterVolume = 0x77;
        }
        
        public byte GetCurrentSample(Channels soundChannel)
        {
            return _soundChannels[(int)soundChannel].GetSample();
        }

        public SoundOutputTerminal GetOutputTerminal(Channels soundChannel)
        {
            var outputTerminal = SoundOutputTerminal.None;

            var leftBitMask = 0x10 << (int)soundChannel;
            var rightBitMask = 0x01 << (int)soundChannel;

            if ((ChannelOutputSelect & leftBitMask) == leftBitMask)
                outputTerminal |= SoundOutputTerminal.Left;
            if ((ChannelOutputSelect & rightBitMask) == rightBitMask)
                outputTerminal |= SoundOutputTerminal.Right;

            return outputTerminal;
        }     
        
        public int GetMasterVolumeLeft()
        {
            return _masterVolumeLeft;
        }

        public int GetMasterVolumeRight()
        {
            return _masterVolumeRight;
        }

        public void AdvanceMachineCycle()
        {
            _frameSequencerTimer++;
            if(_frameSequencerTimer == _frameSequencerPeriod)
            {
                _frameSequencerTimer = 0;
                _frameSequencer.AdvanceClock(_soundChannels);
            }

            _squareWaveSweepChannel.AdvanceMachineCycle();
            _squareWaveChannel.AdvanceMachineCycle();
            _waveChannel.AdvanceMachineCycle();
            _noiseChannel.AdvanceMachineCycle();
        }

        public byte DelegateMemoryRead(ushort address)
        {
            if (address >= AudioRegisters.WaveRamFirstAddress && address <= AudioRegisters.WaveRamLastAddress)
                return _waveChannel.ReadWaveRam(address);

            if (_unusedAudioAddresses.Contains(address))
                return 0xFF;

            return address switch
            {                
                AudioRegisters.NR50 => MasterVolume,
                AudioRegisters.NR51 => ChannelOutputSelect,
                AudioRegisters.NR52 => SoundOnOff,

                AudioRegisters.Channel1.NR10 => _squareWaveSweepChannel.Sweep,
                AudioRegisters.Channel1.NR11 => _squareWaveSweepChannel.LengthLoad,
                AudioRegisters.Channel1.NR12 => _squareWaveSweepChannel.Envelope,
                AudioRegisters.Channel1.NR13 => _squareWaveSweepChannel.FrequencyLsb,
                AudioRegisters.Channel1.NR14 => _squareWaveSweepChannel.LengthEnable,

                AudioRegisters.Channel2.NR20 => 0xFF,
                AudioRegisters.Channel2.NR21 => _squareWaveChannel.LengthLoad,
                AudioRegisters.Channel2.NR22 => _squareWaveChannel.Envelope,
                AudioRegisters.Channel2.NR23 => _squareWaveChannel.FrequencyLsb,
                AudioRegisters.Channel2.NR24 => _squareWaveChannel.LengthEnable,                

                AudioRegisters.Channel3.NR30 => _waveChannel.DacPower,
                AudioRegisters.Channel3.NR31 => _waveChannel.LengthLoad,
                AudioRegisters.Channel3.NR32 => _waveChannel.VolumeCode,
                AudioRegisters.Channel3.NR33 => _waveChannel.FrequencyLsb,
                AudioRegisters.Channel3.NR34 => _waveChannel.LengthEnable,             

                AudioRegisters.Channel4.NR40 => 0xFF,
                AudioRegisters.Channel4.NR41 => _noiseChannel.LengthLoad,
                AudioRegisters.Channel4.NR42 => _noiseChannel.Envelope,
                AudioRegisters.Channel4.NR43 => _noiseChannel.Polynomial,
                AudioRegisters.Channel4.NR44 => _noiseChannel.LengthEnable,                
                
                _ => throw new InvalidOperationException($"0x{address:X2} is not a valid audio address"),
            };
        }

        public void DelegateMemoryWrite(ushort address, byte data)
        {
            if (address >= AudioRegisters.WaveRamFirstAddress && address <= AudioRegisters.WaveRamLastAddress)
            {
                _waveChannel.WriteWaveRam(address, data);
                return;
            }

            if (address == AudioRegisters.NR52)
            {
                SoundOnOff = data;
                return;
            }

            if (_lastOnOffValue == 0)
            {
                //on DMG, length counters can still be written when APU is off
                switch(address)
                {
                    case AudioRegisters.Channel1.NR11:
                        _squareWaveSweepChannel.SetLengthCounter(data & 0x3F);
                        break;
                    case AudioRegisters.Channel2.NR21:
                        _squareWaveChannel.SetLengthCounter(data & 0x3F);
                        break;
                    case AudioRegisters.Channel3.NR31:
                        _waveChannel.SetLengthCounter(data);
                        break;
                    case AudioRegisters.Channel4.NR41:
                        _noiseChannel.SetLengthCounter(data & 0x3F);
                        break;
                }

                return;
            }

            switch (address)
            {                
                case AudioRegisters.NR50:
                    MasterVolume = data;
                    break;
                case AudioRegisters.NR51:
                    ChannelOutputSelect = data;
                    break;

                case AudioRegisters.Channel1.NR10:
                    _squareWaveSweepChannel.Sweep = data;
                    break;
                case AudioRegisters.Channel1.NR11:
                    _squareWaveSweepChannel.LengthLoad = data;
                    break;
                case AudioRegisters.Channel1.NR12:
                    _squareWaveSweepChannel.Envelope = data;
                    break;
                case AudioRegisters.Channel1.NR13:
                    _squareWaveSweepChannel.FrequencyLsb = data;
                    break;
                case AudioRegisters.Channel1.NR14:
                    _squareWaveSweepChannel.LengthEnable = data;
                    break;

                case AudioRegisters.Channel2.NR21:
                    _squareWaveChannel.LengthLoad = data;
                    break;
                case AudioRegisters.Channel2.NR22:
                    _squareWaveChannel.Envelope = data;
                    break;
                case AudioRegisters.Channel2.NR23:
                    _squareWaveChannel.FrequencyLsb = data;
                    break;
                case AudioRegisters.Channel2.NR24:
                    _squareWaveChannel.LengthEnable = data;
                    break;
                
                case AudioRegisters.Channel3.NR30:
                    _waveChannel.DacPower = data;
                    break;
                case AudioRegisters.Channel3.NR31:
                    _waveChannel.LengthLoad = data;
                    break;
                case AudioRegisters.Channel3.NR32:
                    _waveChannel.VolumeCode = data;
                    break;
                case AudioRegisters.Channel3.NR33:
                    _waveChannel.FrequencyLsb = data;
                    break;
                case AudioRegisters.Channel3.NR34:
                    _waveChannel.LengthEnable = data;
                    break;

                case AudioRegisters.Channel4.NR41:
                    _noiseChannel.LengthLoad = data;
                    break;
                case AudioRegisters.Channel4.NR42:
                    _noiseChannel.Envelope = data;
                    break;
                case AudioRegisters.Channel4.NR43:
                    _noiseChannel.Polynomial = data;
                    break;
                case AudioRegisters.Channel4.NR44:
                    _noiseChannel.LengthEnable = data;
                    break;                
            }            
        }

        public IEnumerable<ushort> GetDelegatedAddresses()
        {
            var addressList = new List<ushort>();

            var audioRegisterAddresses = new ushort[] { AudioRegisters.NR51,
                                                        AudioRegisters.NR50,
                                                        AudioRegisters.NR52,

                                                        AudioRegisters.Channel1.NR10,
                                                        AudioRegisters.Channel1.NR11,
                                                        AudioRegisters.Channel1.NR12,
                                                        AudioRegisters.Channel1.NR13,
                                                        AudioRegisters.Channel1.NR14,

                                                        AudioRegisters.Channel2.NR20,
                                                        AudioRegisters.Channel2.NR21,
                                                        AudioRegisters.Channel2.NR22,
                                                        AudioRegisters.Channel2.NR23,
                                                        AudioRegisters.Channel2.NR24,

                                                        AudioRegisters.Channel3.NR30,
                                                        AudioRegisters.Channel3.NR31,
                                                        AudioRegisters.Channel3.NR32,
                                                        AudioRegisters.Channel3.NR33,
                                                        AudioRegisters.Channel3.NR34,

                                                        AudioRegisters.Channel4.NR40,
                                                        AudioRegisters.Channel4.NR41,
                                                        AudioRegisters.Channel4.NR42,
                                                        AudioRegisters.Channel4.NR43,
                                                        AudioRegisters.Channel4.NR44 };            

            var waveRamAddresses = new ushort[AudioRegisters.WaveRamLastAddress - AudioRegisters.WaveRamFirstAddress + 1];
            for (int i = 0; i < waveRamAddresses.Length; i++)
                waveRamAddresses[i] = (ushort)(i + AudioRegisters.WaveRamFirstAddress);

            //these unused audio addresses always return 0xFF on read and ignore writes
            _unusedAudioAddresses = new List<ushort>();
            for (ushort i = AudioRegisters.UnusedAudioAddressRangeStart; i <= AudioRegisters.UnusedAudioAddressRangeEnd; i++)
                _unusedAudioAddresses.Add(i);
            
            addressList.AddRange(audioRegisterAddresses);
            addressList.AddRange(waveRamAddresses);
            addressList.AddRange(_unusedAudioAddresses);

            return addressList;
        }

        private void NotifyOutputTerminalChanged()
        {
            OutputTerminalChangedEvent?.Invoke(this, null);
        }

        private void NotifyMasterVolumeChanged()
        {
            MasterVolumeChangedEvent?.Invoke(this, null);
        }
    }
}
