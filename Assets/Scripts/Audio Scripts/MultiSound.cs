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
    
    public override IEnumerator PlayOnSource(AudioSource mainSource, AudioSource schedulingSource)
    {
        _settings.ApplyValues(mainSource);
        IsInactive = false;

        yield return PlayIntro(mainSource, schedulingSource);
        yield return PlayLooping(schedulingSource);
        yield return PlayOutro(mainSource, schedulingSource);
    }

    private IEnumerator PlayIntro(AudioSource mainSource, AudioSource schedulingSource)
    {
        mainSource.clip = introClip;
        schedulingSource.clip = GetClip();
        double introDuration = (double) introClip.samples / introClip.frequency;
        
        mainSource.PlayScheduled(AudioSettings.dspTime + 0.1);
        schedulingSource.PlayScheduled(AudioSettings.dspTime + 0.1 + introDuration);

        yield return new WaitForSecondsRealtime((float) (0.1 + introDuration));
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

    private IEnumerator PlayOutro(AudioSource mainSource, AudioSource schedulingSource)
    {
        mainSource.clip = outroClip;
        double outroDuration = (double) outroClip.samples / outroClip.frequency;
        
        mainSource.PlayScheduled(AudioSettings.dspTime + 0.1);
        schedulingSource.SetScheduledEndTime(AudioSettings.dspTime + 0.1);

        yield return new WaitForSecondsRealtime((float) (AudioSettings.dspTime + 0.1 + outroDuration));
        
        mainSource.Stop();
    }
}