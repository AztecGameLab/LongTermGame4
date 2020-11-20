using System.Collections;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

public class Channel
{
    [NotNull] public readonly AudioSource Source;
    [CanBeNull] private Sound _attachedSound;
    private bool _hasSound = false;

    public bool IsAvailable => !Source.isPlaying;
    public bool SoundEquals(Sound other) => _attachedSound != null && _attachedSound.id == other.id;
    
    public Channel(AudioSource source, Sound sound = null)
    {
        Source = source;
        
        if (sound != null) 
            SetSound(sound);
    }

    public IEnumerator Play(Sound sound)
    {
        SetSound(sound);
        yield return sound.PlayOnSource(Source);
    }
        
    public IEnumerator Stop()
    {
        if (_hasSound) _attachedSound.IsInactive = true;
        yield return new WaitForEndOfFrame();
    }

    private void SetSound([NotNull] Sound sound)
    {
        _attachedSound = sound;
        _hasSound = true;
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine(_hasSound ? _attachedSound.name : "None");
        result.AppendLine("\tIs active: " + !IsAvailable);
        return result.ToString();
    }
}