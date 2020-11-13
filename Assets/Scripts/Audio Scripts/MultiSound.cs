using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New MultiSound", menuName = "Audio Custom/MultiSound", order = 2)]
public class MultiSound : Sound
{
    [Header("MultiSound Settings")] 
    
    [SerializeField, Tooltip("The clip that is played before the looping clips.")]
    private AudioClip introClip = default;

    [SerializeField, Tooltip("The clip that is played when the looping clips are ended.")]
    private AudioClip outroClip = default;

    [SerializeField, Tooltip("How long this MultiSound will take to crossfade")]
    private float crossfadeDuration = 1;

    private State _currentState = State.Inactive;
    
    public override IEnumerator PlayOnSource(AudioSource mainSource, AudioSource schedulingSource)
    {
        IsInactive = false;

        yield return PlayIntro(mainSource, schedulingSource);
        yield return PlayLooping(schedulingSource);
        yield return PlayOutro(mainSource, schedulingSource);
    }
    
    // This stage cannot be canceled programatically: it will always play to completion
    private IEnumerator PlayIntro(AudioSource intro, AudioSource looping)
    {
        _currentState = State.Intro;
    
        // Prepare both AudioSources for playback
        _settings.ApplyValues(intro);
        _settings.ApplyValues(looping);
        intro.clip = introClip;
        looping.clip = GetClip();
        double introDuration = (double) introClip.samples / introClip.frequency;
        
        // Play the intro, and crossfade the looping clip in right before the intro finishes
        intro.PlayScheduled(AudioSettings.dspTime + 0.1);
        yield return new WaitForSecondsRealtime((float) (0.1 + introDuration - crossfadeDuration));
        yield return Crossfade(looping, intro);
    }

    // This stage can be canceled programatically by setting IsInactive to true
    private IEnumerator PlayLooping(AudioSource looping)
    {
        _currentState = State.Looping;

        // Standard modulation code for the looping source, taken from base Sound class
        var startTime = Time.time;
        
        while (!IsInactive) {
            var elapsedTime = Time.time - startTime;
            
            // Iterate through the modulated values, and call their modulate method
            foreach (var val in modulatedValues)
            {
                SoundValue value = val.soundValue;
                float target = val.modulator.Modulate(val.value, elapsedTime);
                
                SetValue(value, target);
            }

            _settings.ApplyValues(looping);
            IsInactive |= !looping.isPlaying;
            yield return new WaitForEndOfFrame();
        }
    }

    // This stage cannot be canceled programatically: it will always play to completion
    private IEnumerator PlayOutro(AudioSource outro, AudioSource looping)
    {
        // Prepare both AudioSources for playback
        _settings.ApplyValues(outro);
        _settings.ApplyValues(looping); 
        outro.clip = outroClip;
        double outroDuration = (double) outroClip.samples / outroClip.frequency;
        
        yield return Crossfade(outro, looping);

        // Wait for the outro to finish playing
        _currentState = State.Outro;
        yield return new WaitForSecondsRealtime((float) (0.1 + outroDuration - crossfadeDuration));
        
        // Make sure the outro has finished and reset state
        _currentState = State.Inactive;
        outro.Stop();
        IsInactive = true;
    }

    private IEnumerator Crossfade(AudioSource fadeIn, AudioSource fadeOut)
    {
        _currentState = State.Crossfading;
        // Prepare the AudioSources for the crossfade after a short delay.
        // (The delay is added to ensure that everything is happening at the same time)
        _settings.ApplyValues(fadeIn);
        _settings.ApplyValues(fadeOut); 
        float startTime = Time.time;
        fadeIn.volume = 0;
        fadeIn.PlayScheduled(AudioSettings.dspTime + 0.1);
        fadeOut.SetScheduledEndTime(AudioSettings.dspTime + 0.1 + crossfadeDuration);
        yield return new WaitForSeconds(0.1f);
        
        // Over the crossfadeDuration, fadeIn gets louder as fadeOut gets quieter
        while (Time.time - startTime <= crossfadeDuration)
        {
            float percentComplete = (Time.time - startTime) / crossfadeDuration;
            fadeIn.volume = GetValue(SoundValue.Volume) * percentComplete;
            fadeOut.volume = GetValue(SoundValue.Volume) * (1 - percentComplete);
            yield return new WaitForEndOfFrame();
        }
    }

    public override string ToString()
    {
        return base.ToString() + "\nMultiSound State: " + _currentState;
    }

    private enum State
    {
        Intro,
        Looping,
        Outro,
        Crossfading,
        Inactive
    }
}