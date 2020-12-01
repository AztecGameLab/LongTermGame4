using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A representation of a timer, providing UnityEvents for adding logic to different phases of the timer.
/// The TimerTick event passes a float which represents the completion of the timer, from 0 to 1.
/// </summary>
public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timerDuration = 10f;
    [SerializeField, Tooltip("Does the time expiring result in success? True if so.")] 
    private bool defaultTimeExpireResult = false;
    
    [Header("Timer Events")]
    [SerializeField] private TimerTickEvent timerTick = default;
    [SerializeField] private UnityEvent timerStart = default;
    [SerializeField] private UnityEvent timerEnd = default;

    [Header("Timer Sounds")] 
    [SerializeField] private MultiSound timerMultiSound = default;
    [SerializeField] private AudioClip timerFail = default;
    [SerializeField] private AudioClip timerSuccess = default;

    private AudioManager _audioManager;
    private MultiSoundInstance _timerMultiSound;
    
    // How long this timer has been running
    private float _runTime;
    private bool _canceled = false;
    
    private void Start()
    {
        _audioManager = AudioManager.Instance();
        _timerMultiSound = timerMultiSound.GenerateInstance();
    }

    public void StartTimer()
    {
        if (_runTime != 0) return;
        
        _runTime = 0;
        _audioManager.PlaySound(_timerMultiSound);
        _canceled = false;
        StartCoroutine(OnTick());
    }

    public void StopTimer(bool success)
    {
        _timerMultiSound.OutroClip = success ? timerSuccess : timerFail;
        _audioManager.StopSound(_timerMultiSound);
        
        _runTime = 0;
        _canceled = true;
        StopCoroutine(OnTick());
        timerEnd.Invoke();
    }

    private IEnumerator OnTick()
    {
        timerStart.Invoke();
        
        // Fire the timerTick event every frame that the timer is running, passing in the current completion
        while (_runTime < timerDuration && _canceled == false)
        {
            timerTick.Invoke(_runTime / timerDuration);
            _runTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();            
        }

        if (!_canceled) StopTimer(defaultTimeExpireResult);
    }
}

/// <summary>
/// A UnityEvent that is called every frame while a timer is running.
/// The float it passes represents the timer's current completion.
/// 0 = just started, 1 = finished
/// </summary>
[Serializable]
public class TimerTickEvent : UnityEvent<float>
{
}