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

		private BufferedAudioSource[] _bufferedAudioSources;

		private const int ChannelCount = 4;

		internal SoundPlayer()
        {
			_alDevice = ALC.OpenDevice(null);

			var contextAttributes = new ALContextAttributes();
			_alContext = ALC.CreateContext(_alDevice, contextAttributes);			

			ALC.MakeContextCurrent(_alContext);

			_bufferedAudioSources = new BufferedAudioSource[4];
			for(int i = 0; i<ChannelCount; i++)
				_bufferedAudioSources[i] = new BufferedAudioSource();			

			//TODO: Start/restart the sources synchronously
		}

		internal void Close()
		{
			foreach (var bufferedAudioSource in _bufferedAudioSources)
				bufferedAudioSource.Close();

			ALC.DestroyContext(_alContext);
			ALC.CloseDevice(_alDevice);
		}	
		
		public void QueueAudioSample(Channels soundChannel, byte sample)
		{
			_bufferedAudioSources[(int)soundChannel].QueueSample(sample);
		}

		public void SetChannelPosition(Channels soundChannel, SoundOutputTerminal position)
		{
			_bufferedAudioSources[(int)soundChannel].SetPosition(position);
		}

		public void QueueBuffersIfFull()
        {
			for (int i = 0; i < ChannelCount; i++)
				_bufferedAudioSources[i].QueueBufferIfFull();
		}
	}
}
