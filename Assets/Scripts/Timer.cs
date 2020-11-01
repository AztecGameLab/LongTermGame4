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
    
    [Header("Timer Events")]
    [SerializeField] private TimerTickEvent timerTick = default;
    [SerializeField] private UnityEvent timerStart = default;
    [SerializeField] private UnityEvent timerEnd = default;

    // How long this timer has been running
    private float _runTime;

    public void StartTimer()
    {
        _runTime = 0;
        StartCoroutine(OnTick());
    }

    public void StopTimer()
    {
        _runTime = 0;
        StopCoroutine(OnTick());
        timerEnd.Invoke();
    }

    private IEnumerator OnTick()
    {
        timerStart.Invoke();
        
        // Fire the timerTick event every frame that the timer is running, passing in the current completion
        while (_runTime < timerDuration)
        {
            timerTick.Invoke(_runTime / timerDuration);
            _runTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();            
        }

        timerEnd.Invoke();
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