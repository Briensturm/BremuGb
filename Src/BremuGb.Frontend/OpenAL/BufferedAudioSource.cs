﻿using System;
using System.Collections.Generic;

using OpenToolkit.Audio.OpenAL;

using BremuGb.Audio;

namespace BremuGb.Frontend.OpenAL
{
    internal class BufferedAudioSource
	{
		private List<byte> _sampleList;

		private List<int> _bufferList;

		private bool _doInit;
		private int _source;

		private int _bufferSize = 512;
		private int _bufferCount = 20;
	
		private int _sampleRate = 44100;

		private int _currentSample;
		private int _sampleCount;
		private int _sampleCountAverage = 24;

		private float[] _centerGainValues = new float[15] { 0.0f, 0.07f, 0.14f, 0.21f, 0.28f, 0.35f, 0.42f, 0.5f, 
																  0.57f, 0.64f, 0.71f, 0.78f, 0.85f, 0.92f, 1.0f };
		private float[] _singleSideGainValues = new float[8] { 0.0f, 0.07f, 0.14f, 0.21f, 0.28f, 0.35f, 0.42f, 0.5f };

		private int _volumeCodeLeft  = 0x7;
		private int _volumeCodeRight = 0x7;
		private SoundOutputTerminal _position = SoundOutputTerminal.Center;

		internal BufferedAudioSource()
		{
			_sampleList = new List<byte>();
			_bufferList = new List<int>();

			for (int i = 0; i<_bufferCount; i++)
            {
				var sampleBuffer = AL.GenBuffer();
				_bufferList.Add(sampleBuffer);
			}			

			_source = AL.GenSource();
			_doInit = true;
		}

		internal void Close()
		{
			AL.SourceStop(_source);
			AL.DeleteSource(_source);

			for(int i = 0; i<_bufferCount; i++)
				AL.DeleteBuffer(_bufferList[i]);

			ThrowIfOpenAlError();
		}

		internal void SetPosition(SoundOutputTerminal position)
		{
			_position = position;

			switch (position)
			{
				case SoundOutputTerminal.Center:
					AL.Source(_source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
					AL.Source(_source, ALSourcef.Gain, _centerGainValues[_volumeCodeLeft + _volumeCodeRight]);
					break;
				case SoundOutputTerminal.Left:
					AL.Source(_source, ALSource3f.Position, -1.0f, 0.0f, 0.0f);
					AL.Source(_source, ALSourcef.Gain, _singleSideGainValues[_volumeCodeLeft]);
					break;
				case SoundOutputTerminal.Right:
					AL.Source(_source, ALSource3f.Position, 1.0f, 0.0f, 0.0f);
					AL.Source(_source, ALSourcef.Gain, _singleSideGainValues[_volumeCodeRight]);
					break;
				case SoundOutputTerminal.None:
					AL.Source(_source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
					
					//mute
					AL.Source(_source, ALSourcef.Gain, 0.0f);
					break;
			}

			ThrowIfOpenAlError();
		}

		internal void SetVolume(int volumeCodeLeft, int volumeCodeRight)
        {
			_volumeCodeLeft = volumeCodeLeft;
			_volumeCodeRight = volumeCodeRight;

			switch (_position)
			{
				case SoundOutputTerminal.Center:
					AL.Source(_source, ALSourcef.Gain, _centerGainValues[_volumeCodeLeft + _volumeCodeRight]);
					break;
				case SoundOutputTerminal.Left:
					AL.Source(_source, ALSourcef.Gain, _singleSideGainValues[_volumeCodeLeft]);
					break;
				case SoundOutputTerminal.Right:
					AL.Source(_source, ALSourcef.Gain, _singleSideGainValues[_volumeCodeRight]);
					break;
			}

			ThrowIfOpenAlError();
		}

		internal void QueueSample(byte sample)
		{
			_currentSample += sample;
			_sampleCount++;

			if (_sampleCount < _sampleCountAverage)
				return;

			var averagedSample = (byte)(_currentSample / _sampleCountAverage);
			_sampleCount = 0;
			_currentSample = 0;

			if (_doInit)
			{
				_sampleList.Add(averagedSample);
				if (_sampleList.Count == _bufferSize * _bufferCount)
				{
					_doInit = false;

					for(int i = 0; i<_bufferList.Count; i++)
                    {
						AL.BufferData(_bufferList[i], ALFormat.Mono8, _sampleList.GetRange(i * _bufferSize, _bufferSize).ToArray(), _bufferSize, _sampleRate);
						AL.SourceQueueBuffer(_source, _bufferList[i]);
					}
					_sampleList.Clear();

					AL.SourcePlay(_source);

					return;
				}
				else
					return;
			}

			if (_sampleList.Count < _bufferSize)
				_sampleList.Add(averagedSample);

			QueueBufferIfFull();
		}

		internal void QueueBufferIfFull()
        {
			if (_sampleList.Count < _bufferSize)
				return;

			QueueBuffer();
		}

        internal void QueueBuffer()
        {
			AL.GetSource(_source, ALGetSourcei.BuffersProcessed, out var processedBufferCount);

			ThrowIfOpenAlError();

			if (processedBufferCount == 0)
				return;
			else
			{
				//refill processed buffers
				while (processedBufferCount > 0 && _sampleList.Count >= _bufferSize)
				{
					var buffer = AL.SourceUnqueueBuffer(_source);

					ThrowIfOpenAlError();

					AL.BufferData(buffer, ALFormat.Mono8, _sampleList.ToArray(), _sampleList.Count, _sampleRate);

					ThrowIfOpenAlError();

					AL.SourceQueueBuffer(_source, buffer);
					ThrowIfOpenAlError();

					_sampleList.RemoveRange(0, _bufferSize);
					processedBufferCount--;
				}
			}

			var sourceState = AL.GetSourceState(_source);
			if (sourceState.HasFlag(ALSourceState.Initial) || sourceState.HasFlag(ALSourceState.Stopped))
			{
				//buffer underflow, restarting source
				AL.SourcePlay(_source);
				ThrowIfOpenAlError();
			}
		}

		void ThrowIfOpenAlError()
		{
			var error = AL.GetError();
			if (error != ALError.NoError)
				throw new InvalidOperationException("AL Error: " + error.ToString());
		}
	}
}
