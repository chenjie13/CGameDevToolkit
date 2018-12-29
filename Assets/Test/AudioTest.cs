
using CGameDevToolkit.Framework;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip MusicClip;
    public AudioClip SfxClip;

    private AudioUnit _audioUnit;

    private void Start()
    {
        _audioUnit = AudioManager.Prepare(MusicClip, loop: true, persistType: AudioPersistType.Persist);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _audioUnit.Play();
            Debug.Assert(_audioUnit.IsPlaying, "IsPlaying");
            Debug.Log("start with " + _audioUnit.Volume);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_audioUnit.IsPaused)
            {
                _audioUnit.Resume();
                Debug.Assert(!_audioUnit.IsPaused, "!IsPaused");
            }
            else
            {
                _audioUnit.Pause();
                Debug.Assert(_audioUnit.IsPaused, "IsPaused");
                Debug.Log("in pause isplaying " + _audioUnit.IsPlaying);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _audioUnit.FadeTo(0, 1, 3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _audioUnit.Stop(1);
            Debug.Assert(_audioUnit.IsStopping, "IsStopping");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.PlayOnce(SfxClip, 1);
        }
    }
}
