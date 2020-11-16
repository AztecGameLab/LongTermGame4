using System.Collections;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

public class Channel
{
    [NotNull] public readonly AudioSource MainSource;
    [NotNull] public readonly AudioSource SchedulingSource;
    
    [CanBeNull] private SoundInstance _attachedSound;
    private bool _hasSound = false;

    public bool IsAvailable => !MainSource.isPlaying && !SchedulingSource.isPlaying;
    
    public Channel(AudioSource mainSource, AudioSource schedulingSource, SoundInstance sound = null)
    {
        MainSource = mainSource;
        SchedulingSource = schedulingSource;
        
        if (sound != null) 
            SetSound(sound);
    }

    public bool HasSound(SoundInstance sound)
    {
        return _attachedSound == sound;
    }

    public IEnumerator Play(SoundInstance sound)
    {
        SetSound(sound);
        yield return sound.PlayOnSource(MainSource, SchedulingSource);
    }
        
    public IEnumerator Stop()
    {
        if (_hasSound) _attachedSound.IsInactive = true;
        yield return new WaitForEndOfFrame();
    }

    private void SetSound([NotNull] SoundInstance sound)
    {
        _attachedSound = sound;
        _hasSound = true;
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine(_hasSound ? _attachedSound.ToString() : "None");
        return result.ToString();
    }
}