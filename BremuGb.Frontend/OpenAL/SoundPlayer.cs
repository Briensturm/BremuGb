using System;
using OpenToolkit.Audio.OpenAL;

using BremuGb.Audio.SoundChannels;
using BremuGb.Frontend.OpenAL;
using BremuGb.Audio;

namespace BremuGb.Frontend
{
    internal class SoundPlayer
    {
		private ALDevice _alDevice;
		private ALContext _alContext;		

		private BufferedAudioSource _channel1Source;
		private BufferedAudioSource _channel2Source;
		private BufferedAudioSource _channel3Source;
		private BufferedAudioSource _channel4Source;

		internal SoundPlayer()
        {
			_alDevice = ALC.OpenDevice(null);

			var contextAttributes = new ALContextAttributes();
			_alContext = ALC.CreateContext(_alDevice, contextAttributes);			

			ALC.MakeContextCurrent(_alContext);

			_channel1Source = new BufferedAudioSource();
			_channel2Source = new BufferedAudioSource();
			_channel3Source = new BufferedAudioSource();
			_channel4Source = new BufferedAudioSource();

			//TODO: Start the sources synchronously
		}

		internal void Close()
		{
			_channel1Source.Close();
			_channel2Source.Close();
			_channel3Source.Close();
			_channel4Source.Close();

			ALC.DestroyContext(_alContext);
			ALC.CloseDevice(_alDevice);
		}	
		
		public void QueueAudioSample(Channels soundChannel, byte sample)
		{
			switch (soundChannel)
			{
				case Channels.Channel1:
					_channel1Source.QueueSample(sample);
					break;
				case Channels.Channel2:
					_channel2Source.QueueSample(sample);
					break;
				case Channels.Channel3:
					_channel3Source.QueueSample(sample);
					break;
				case Channels.Channel4:
					_channel4Source.QueueSample(sample);
					break;
				default:
					throw new InvalidOperationException("Invalid sound channel specified");
			}
		}

		public void SetChannelPosition(Channels soundChannel, SoundOutputTerminal position)
		{
			switch (soundChannel)
			{
				case Channels.Channel1:
					_channel1Source.SetPosition(position);
					break;
				case Channels.Channel2:
					_channel2Source.SetPosition(position);
					break;
				case Channels.Channel3:
					_channel3Source.SetPosition(position);
					break;
				case Channels.Channel4:
					_channel4Source.SetPosition(position);
					break;
				default:
					throw new InvalidOperationException("Invalid sound channel specified");
			}
		}
	}
}
