using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGameDevToolkit.Framework
{
    public enum AudioPersistType
    {
        Once,     //只播放一次，停止后自动移除
        Scene,    //切换场景时移除
        Persist   //不会在切换场景时移除，需要手动移除
    }

    public class AudioUnit
    {
        public int AudioId { get; private set; }
        public int Group { get; private set; }
        public float Volume { get; private set; }
        public AudioSource Source { get; private set; }

        public AudioPersistType PersistType;
        
        public AudioClip Clip
        {
            get { return Source.clip; }
            set { Source.clip = value; }
        }

        public bool Loop
        {
            get { return Source.loop; }
            set { Source.loop = value; }
        }

        public bool IsPlaying
        {
            get { return Source.isPlaying; }
        }

        public bool IsStopping { get; private set; }
        public bool IsPaused { get; private set; }

        public bool IsStopped
        {
            get { return !IsPlaying && !IsPaused; }
        }

        private static int _audioCounter;

        private float _endVolume;
        private float _duration;
        private float _fadeInterpolater;
        private float _startVolume;

        public AudioUnit(AudioClip clip, int group, float volume, bool loop, AudioPersistType persistType)
        {
            AudioId = _audioCounter;
            _audioCounter++;

            Group = group;
            Source = AudioManager.GetAudioSource(Group);
            Source.volume = Volume;
            Clip = clip;
            Volume = volume;
            Loop = loop;
            PersistType = persistType;
        }

        public void Play()
        {
            Source.Play();
        }

        /// <summary>
        /// 停止播放音频或在指定时间内将音量将为0
        /// </summary>
        public void Stop(float duration = -1)
        {
            FadeTo(0, duration);
            IsStopping = true;
        }

        public void Pause()
        {
            Source.Pause();
            IsPaused = true;
        }

        public void Resume()
        {
            Source.UnPause();
            IsPaused = false;
        }

        /// <summary>
        /// 渐变音频音量
        /// </summary>
        public void FadeTo(float endVolume, float duration)
        {
            FadeTo(Volume, endVolume, duration);
        }

        public void FadeTo(float startVolume, float endVolume, float duration)
        {
            _endVolume = Mathf.Clamp01(endVolume);
            _fadeInterpolater = 0;
            Volume = _startVolume = startVolume;
            _duration = duration;
        }


        public void Update()
        {
            if (Source == null) return;

            if (IsPlaying && _duration > float.Epsilon && Math.Abs(Volume - _endVolume) > float.Epsilon)
            {
                _fadeInterpolater += Time.unscaledDeltaTime;
                Volume = Mathf.Lerp(_startVolume, _endVolume, _fadeInterpolater / _duration);
            }
            Source.volume = Volume * AudioManager.GetGroupVolume(Group);

            if (Volume < float.Epsilon && IsStopping)
            {
                Source.Stop();
                IsStopping = false;
                IsPaused = false;
            }
        }

        public void Destroy()
        {
            AudioManager.DestroyAudio(this);
        }
    }
}