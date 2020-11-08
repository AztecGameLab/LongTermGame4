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

    [SerializeField]
    private float crossfadeDuration = 1;
    
    public override IEnumerator PlayOnSource(AudioSource mainSource, AudioSource schedulingSource)
    {
        _settings.ApplyValues(mainSource);
        _settings.ApplyValues(schedulingSource);
        IsInactive = false;

        yield return PlayIntro(mainSource, schedulingSource);
        yield return PlayLooping(schedulingSource);
        yield return PlayOutro(mainSource, schedulingSource);
    }

    private IEnumerator PlayIntro(AudioSource intro, AudioSource looping)
    {
        intro.clip = introClip;
        looping.clip = GetClip();
        double introDuration = (double) introClip.samples / introClip.frequency;
        
        intro.PlayScheduled(AudioSettings.dspTime + 0.1);
        yield return new WaitForSecondsRealtime((float) (0.1 + introDuration - crossfadeDuration));

        
        yield return CrossFade(looping, intro);
    }

    private IEnumerator PlayLooping(AudioSource schedulingSource)
    {
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

            _settings.ApplyValues(schedulingSource);
            IsInactive |= !schedulingSource.isPlaying;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator PlayOutro(AudioSource outro, AudioSource looping)
    {
        outro.clip = outroClip;
        double outroDuration = (double) outroClip.samples / outroClip.frequency;
        outro.PlayScheduled(AudioSettings.dspTime + 0.1);
        
        yield return CrossFade(outro, looping);

        yield return new WaitForSecondsRealtime((float) (0.1 + outroDuration - crossfadeDuration));
        outro.Stop();
    }

    private IEnumerator CrossFade(AudioSource fadeIn, AudioSource fadeOut)
    {
        var startTime = Time.time;

        fadeIn.PlayScheduled(AudioSettings.dspTime + 0.1);
        fadeIn.volume = 0;
        fadeOut.SetScheduledEndTime(AudioSettings.dspTime + 0.1 + crossfadeDuration);
        yield return new WaitForSeconds(0.1f);
        
        while (Time.time - startTime <= crossfadeDuration)
        {
            var percentComplete = (Time.time - startTime) / crossfadeDuration;
            fadeIn.volume = GetValue(SoundValue.Volume) * percentComplete;
            fadeOut.volume = GetValue(SoundValue.Volume) * (1 - percentComplete);
            yield return new WaitForEndOfFrame();
        }
    }
}