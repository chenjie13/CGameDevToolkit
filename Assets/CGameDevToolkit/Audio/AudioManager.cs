using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace CGameDevToolkit.Framework
{
    public class AudioGroupSetting
    {
        public float Volume = 1;
        public bool IgnoreDuplicate;
        public Transform SourceTrans;
    }

    [MonoSingletonPath("[Manager]/AudioManager")]
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private static Dictionary<int, AudioUnit> _audioDic = new Dictionary<int, AudioUnit>();
        private static Dictionary<int, AudioGroupSetting> _groupSettingDic = new Dictionary<int, AudioGroupSetting>();

        public override void OnSingletonInit()
        {
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// 加载场景时移除非持久类型(AudioPersistType.Persist)的AudioUnit
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RemoveNonPersistAudio(_audioDic);
        }

        private void Update()
        {
            UpdateAllAudio(_audioDic);
        }

        private static IEnumerable<int> GetGroupAudioIds(int group)
        {
            foreach (var audioUnit in _audioDic)
            {
                if (audioUnit.Value.Group == group)
                {
                    yield return audioUnit.Key;
                }
            }
        }

        internal static AudioSource GetAudioSource(int group)
        {
            return GetGroupSourceTrans(group).gameObject.AddComponent<AudioSource>();
        }

        private static void UpdateAllAudio(Dictionary<int, AudioUnit> audioDict)
        {
            var keys = new List<int>(audioDict.Keys);
            foreach (var key in keys)
            {
                AudioUnit audioUnit = audioDict[key];
                audioUnit.Update();

                if (audioUnit.PersistType == AudioPersistType.Once && audioUnit.IsStopped)
                {
                    DestroyAudio(audioUnit);
                }
            }
        }

        private static void RemoveNonPersistAudio(Dictionary<int, AudioUnit> audioDict)
        {
            var keys = new List<int>(audioDict.Keys);
            foreach (var key in keys)
            {
                AudioUnit audioUnit = audioDict[key];
                if (audioUnit.PersistType != AudioPersistType.Persist)
                {
                    DestroyAudio(audioUnit);
                }
            }
        }

        private static AudioGroupSetting NewOrGetGroupSetting(int group)
        {
            if (!_groupSettingDic.ContainsKey(group))
                _groupSettingDic.Add(group, new AudioGroupSetting());
            return _groupSettingDic[group];
        }

        #region Setting Functions

        public static bool GetGroupIgnoreDuplicate(int group)
        {
            return _groupSettingDic.ContainsKey(group) && _groupSettingDic[group].IgnoreDuplicate;
        }

        public static void SetGroupIgnoreDuplicate(int group, bool ignore)
        {
            NewOrGetGroupSetting(group).IgnoreDuplicate = ignore;
        }

        public static float GetGroupVolume(int group)
        {
            return _groupSettingDic.ContainsKey(group) ? _groupSettingDic[group].Volume : 0;
        }

        public static void SetGlobalVolume(int group, float volume)
        {
            NewOrGetGroupSetting(group).Volume = volume;
        }

        public static Transform GetGroupSourceTrans(int group)
        {
            var setting = NewOrGetGroupSetting(group);
            if (setting.SourceTrans == null)
            {
                var go = new GameObject("Group " + group);
                go.transform.parent = Instance.transform;
                setting.SourceTrans = go.transform;
            }

            return setting.SourceTrans;
        }

        public static void SetGroupSourceTrans(int group, Transform trans)
        {
            NewOrGetGroupSetting(group).SourceTrans = trans;
        }

        #endregion

        #region Audio Functions

        public static AudioUnit GetAudio(int audioID)
        {
            if (_audioDic.ContainsKey(audioID))
            {
                return _audioDic[audioID];
            }

            return null;
        }

        public static AudioUnit GetAudio(AudioClip audioClip)
        {
            var keys = new List<int>(_audioDic.Keys);
            foreach (var key in keys)
            {
                AudioUnit audioUnit = _audioDic[key];
                if (audioUnit.Clip == audioClip)
                {
                    return audioUnit;
                }
            }

            return null;
        }

        /// <summary>
        /// 准备播放音频
        /// 该方法在需要主动对音频操作，长期持有AudioUnit变量，设置复杂播放参数时使用
        /// </summary>
        //NOTE: prepare（persistType=once）要立即play,不然创建出来的audioUnit会在下一帧destroy
        //      只播放一次推荐使用 PlayOnce                 
        public static AudioUnit Prepare(AudioClip clip, int group = 0, float volume = 1, bool loop = false,
            AudioPersistType persistType = AudioPersistType.Scene)
        {
            if (clip == null)
            {
                Debug.LogError("[AudioManager] Audio clip is null", clip);
            }

            if (GetGroupIgnoreDuplicate(group))
            {
                AudioUnit duplicateAudioUnit = GetAudio(clip);
                if (duplicateAudioUnit != null)
                {
                    return duplicateAudioUnit;
                }
            }

            var audioUnit = new AudioUnit(clip, group, volume, loop, persistType);
            _audioDic.Add(audioUnit.AudioId, audioUnit);
            return audioUnit;
        }

        /// <summary>
        /// 播放一次音频，结束自动destroy
        /// </summary>
        public static void PlayOnce(AudioClip clip, int group = 0, float volume = 1)
        {
            Prepare(clip, group, volume, false, AudioPersistType.Once).Play();
        }

        public static void StopAll(float fadeOutSeconds = -1)
        {
            foreach (var key in _audioDic.Keys)
            {
                _audioDic[key].Stop(fadeOutSeconds);
            }
        }

        private static void StopGroup(int group, float fadeOutSeconds = -1)
        {
            foreach (var key in GetGroupAudioIds(group))
            {
                _audioDic[key].Stop(fadeOutSeconds);
            }
        }

        public static void PauseAll()
        {
            foreach (var key in _audioDic.Keys)
            {
                _audioDic[key].Pause();
            }
        }

        public static void PauseGroup(int group)
        {
            foreach (var key in GetGroupAudioIds(group))
            {
                _audioDic[key].Pause();
            }
        }

        public static void ResumeAll()
        {
            foreach (var key in _audioDic.Keys)
            {
                _audioDic[key].Resume();
            }
        }


        public static void ResumeGroup(int group)
        {
            foreach (int key in GetGroupAudioIds(group))
            {
                _audioDic[key].Resume();
            }
        }

        public static void DestroyAudio(int audioId)
        {
            DestroyAudio(GetAudio(audioId));
        }

        public static void DestroyAudio(AudioUnit audioUnit)
        {
            if (audioUnit == null) return;
            if (audioUnit.Source != null)
            {
                Destroy(audioUnit.Source);
            }

            _audioDic.Remove(audioUnit.AudioId);
        }

        public static void DestroyGroup(int group)
        {
            foreach (var id in GetGroupAudioIds(group))
            {
                DestroyAudio(id);
            }
        }

        public static void DestroyAll()
        {
            foreach (var audioUnit in _audioDic.Values)
            {
                DestroyAudio(audioUnit);
            }
        }

        #endregion
    }
}