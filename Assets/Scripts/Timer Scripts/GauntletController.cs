using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A MonoBehavior "Gauntlet" that will listen to a list of EventTriggers and will throw a success event
/// if every EventTrigger is activated before the EndGauntlet() method is called.
/// <remarks>Works well in tandem with a Timer, starting and ending the gauntlet after some time has passed</remarks> 
/// </summary>
public class GauntletController : MonoBehaviour
{
    [Header("Gauntlet Settings")]
    [SerializeField] private UnityEvent onSuccess = default;
    [SerializeField] private UnityEvent onReset = default;
    [SerializeField] private List<EventTrigger> targets = default;
    
    private int MaxCharges => targets.Count;
    private bool Charged => _charges >= MaxCharges;
    private bool _acceptingCharge = false;
    private int _charges;
    private Timer _currentTimer;
    
    private void OnEnable()
    {
        foreach (var target in targets)
        {
            target.collisionStart.AddListener(OnTargetActivate);
        }
    }

    private void OnDisable()
    {
        foreach (var target in targets)
        {
            target.collisionStart.RemoveListener(OnTargetActivate);
        }
    }

    public void StartGauntlet(Timer timer)
    {
        if (Charged) onReset.Invoke();
        _charges = 0;
        _acceptingCharge = true;
        _currentTimer = timer;
    }

    public void EndGauntlet()
    {
        _acceptingCharge = false;
        if (!Charged)
        {
            onReset.Invoke();
        }
    }

    private void OnTargetActivate()
    {
        if (!_acceptingCharge || Charged) return;
        _charges++;
        if (Charged)
        {
            onSuccess.Invoke();
            _currentTimer.StopTimer(true);
        }
    }
}