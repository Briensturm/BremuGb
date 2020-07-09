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

			//noise and wave channel are panned left initially
			_bufferedAudioSources[2].SetPosition(SoundOutputTerminal.Left);
			_bufferedAudioSources[3].SetPosition(SoundOutputTerminal.Left);

			//TODO: Start/restart the sources synchronously
		}

		internal void Close()
		{
			foreach (var bufferedAudioSource in _bufferedAudioSources)
				bufferedAudioSource.Close();

			ALC.DestroyContext(_alContext);
			ALC.CloseDevice(_alDevice);
		}	
		
		internal void QueueAudioSample(Channels soundChannel, byte sample)
		{
			_bufferedAudioSources[(int)soundChannel].QueueSample(sample);
		}

		internal void SetChannelPosition(Channels soundChannel, SoundOutputTerminal position)
		{
			_bufferedAudioSources[(int)soundChannel].SetPosition(position);
		}

		internal void SetVolume(int volumeCodeLeft, int volumeCodeRight)
        {
			for (int i = 0; i < ChannelCount; i++)
				_bufferedAudioSources[i].SetVolume(volumeCodeLeft, volumeCodeRight);
		}

		//TODO: Check if calling this helps keep refilling buffers
		internal void QueueBuffersIfFull()
        {
			for (int i = 0; i < ChannelCount; i++)
				_bufferedAudioSources[i].QueueBufferIfFull();
		}
	}
}
