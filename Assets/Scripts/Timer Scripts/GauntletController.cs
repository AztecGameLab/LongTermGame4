using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GauntletController : MonoBehaviour
{
    [SerializeField] private List<EventTrigger> inactiveTargets = default;
    [SerializeField] private UnityEvent onSuccess = default;
    [SerializeField] private UnityEvent onFail = default;
    
    private int MaxCharges => inactiveTargets.Count;
    private List<EventTrigger> _activeTargets = new List<EventTrigger>();
    private bool _acceptingCharge = false;
    private bool Charged => charges >= MaxCharges;
    private int charges;
    
    private void OnEnable()
    {
        foreach (var target in inactiveTargets)
        {
            target.triggerEnter.AddListener(OnTargetActivate);
            target.triggerExit.AddListener(OnTargetDeactivate);
        }
    }

    private void OnDisable()
    {
        foreach (var target in inactiveTargets)
        {
            target.triggerEnter.RemoveListener(OnTargetActivate);
            target.triggerExit.RemoveListener(OnTargetDeactivate);
        }
    }

    public void StartGauntlet()
    {
        charges = 0;
        _acceptingCharge = true;
    }

    public void EndGauntlet()
    {
        _acceptingCharge = false;
        if (!Charged) onFail.Invoke();
    }

    private void OnTargetActivate()
    {
        if (!_acceptingCharge || Charged) return;
        charges++;
        if (Charged) onSuccess.Invoke();
    }
    
    private void OnTargetDeactivate()
    {
        if (!_acceptingCharge || charges <= 0) return;
        charges--;
    }
}