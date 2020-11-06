using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GauntletController : MonoBehaviour
{
    [SerializeField] private List<EventTrigger> inactiveTargets = default;
    [SerializeField] private UnityEvent onSuccess = default;
    [SerializeField] private UnityEvent onReset = default;
    
    private int MaxCharges => inactiveTargets.Count;
    private List<EventTrigger> _activeTargets = new List<EventTrigger>();
    private bool _acceptingCharge = false;
    private bool Charged => charges >= MaxCharges;
    private int charges;
    
    private void OnEnable()
    {
        foreach (var target in inactiveTargets)
        {
            target.collisionStart.AddListener(OnTargetActivate);
        }
    }

    private void OnDisable()
    {
        foreach (var target in inactiveTargets)
        {
            target.collisionStart.RemoveListener(OnTargetActivate);
        }
    }

    public void StartGauntlet()
    {
        if (Charged) onReset.Invoke();
        charges = 0;
        _acceptingCharge = true;
    }

    public void EndGauntlet()
    {
        _acceptingCharge = false;
        if (!Charged) onReset.Invoke();
    }

    private void OnTargetActivate()
    {
        if (!_acceptingCharge || Charged) return;
        charges++;
        if (Charged) onSuccess.Invoke();
    }
}