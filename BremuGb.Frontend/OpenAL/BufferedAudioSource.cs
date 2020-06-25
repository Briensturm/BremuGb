using System;
using System.Collections.Generic;

using OpenToolkit.Audio.OpenAL;

using BremuGb.Audio;

namespace BremuGb.Frontend.OpenAL
{
    internal class BufferedAudioSource
	{ 
		private List<byte> _sampleList;
		private List<byte> _initialList;
		private bool _doInit = true;

		private int _sampleBuffer0;
		private int _sampleBuffer1;

		private int _source;

		private int _bufferSize = 4410*2;
		private int _sampleRate = 41943;

		internal BufferedAudioSource()
		{
			_sampleList = new List<byte>();
			_initialList = new List<byte>();

			_sampleBuffer0 = AL.GenBuffer();
			_sampleBuffer1 = AL.GenBuffer();

			_source = AL.GenSource();
		}

		internal void Close()
		{
			AL.SourceStop(_source);
			AL.DeleteSource(_source);
			AL.DeleteBuffer(_sampleBuffer0);
			AL.DeleteBuffer(_sampleBuffer1);
		}

		internal void SetPosition(SoundOutputTerminal position)
		{
			switch(position)
			{
				case SoundOutputTerminal.Center:
					AL.Source(_source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
					AL.Source(_source, ALSourcef.Gain, 1.0f);
					break;
				case SoundOutputTerminal.Left:
					AL.Source(_source, ALSource3f.Position, -1.0f, 0.0f, 0.0f);
					AL.Source(_source, ALSourcef.Gain, 1.0f);
					break;
				case SoundOutputTerminal.Right:
					AL.Source(_source, ALSource3f.Position, 1.0f, 0.0f, 0.0f);
					AL.Source(_source, ALSourcef.Gain, 1.0f);
					break;
				case SoundOutputTerminal.None:
					AL.Source(_source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
					
					//mute
					AL.Source(_source, ALSourcef.Gain, 0.0f);
					break;
			}
		}

		internal void QueueSample(byte sample)
		{
			if (_doInit)
			{
				_initialList.Add(sample);
				if (_initialList.Count == _bufferSize)
					_doInit = false;

				return;
			}

			if(_sampleList.Count < _bufferSize)
				_sampleList.Add(sample);

			if (_sampleList.Count >= _bufferSize)
			{
				if (_initialList.Count > 0)
				{
					Console.WriteLine("Start streaming audio...");
					AL.BufferData(_sampleBuffer0, ALFormat.Mono8, _initialList.ToArray(), _initialList.Count, _sampleRate);
					AL.BufferData(_sampleBuffer1, ALFormat.Mono8, _sampleList.ToArray(), _sampleList.Count, _sampleRate);
					AL.SourceQueueBuffer(_source, _sampleBuffer0);
					AL.SourceQueueBuffer(_source, _sampleBuffer1);

					AL.SourcePlay(_source);

					_initialList.Clear();
					_sampleList.Clear();
				}
				else
					QueueBuffer(_sampleList);
			}

			void QueueBuffer(List<byte> bufferList)
			{
				AL.GetSource(_source, ALGetSourcei.BuffersProcessed, out var processedBufferCount);

				ThrowIfOpenAlError();

				if (processedBufferCount == 0)
					return;

				var buffer = AL.SourceUnqueueBuffer(_source);

				ThrowIfOpenAlError();

				AL.BufferData(buffer, ALFormat.Mono8, _sampleList.ToArray(), _sampleList.Count, _sampleRate);

				ThrowIfOpenAlError();

				AL.SourceQueueBuffer(_source, buffer);
				ThrowIfOpenAlError();

				var sourceState = AL.GetSourceState(_source);
				if (sourceState.HasFlag(ALSourceState.Initial) || sourceState.HasFlag(ALSourceState.Stopped))
				{
					Console.WriteLine("Audio buffer underflow occured, resuming play...");
					AL.SourcePlay(_source);
					ThrowIfOpenAlError();
				}

				_sampleList.Clear();
			}

			void ThrowIfOpenAlError()
			{
				var error = AL.GetError();
				if (error != ALError.NoError)
					throw new InvalidOperationException("AL Error: " + error.ToString());
			}
		}
	}
}
