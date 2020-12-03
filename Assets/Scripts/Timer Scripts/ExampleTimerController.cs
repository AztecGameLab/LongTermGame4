using System.Collections;
using UnityEngine;

/// <summary>
/// An example testing class to show what can be done with the timer system
/// </summary>
public class ExampleTimerController : MonoBehaviour
{
    [SerializeField] private Rigidbody platform = default;
    [SerializeField] private Transform endPosition = default;
    private Vector3 _startPos, _endPos;

    [SerializeField] private Transform spinningThing = default;
    private Vector3 _angle;

    [SerializeField] private GameObject bar = default;

    private void OnEnable()
    {
        bar.transform.localScale = Vector3.zero;
        _startPos = platform.position;
        _endPos = endPosition.position;
        _angle = spinningThing.eulerAngles;
    }

    public void UpdateTimerObjects(float completion)
    {
        UpdateSpinningThing();
        UpdatePlatform(completion);
        UpdateBar(completion);
    }
    
    public void ResetTimerObjects()
    {
        StartCoroutine(ResetTimerObjectsCoroutine());
    }
    
    private IEnumerator ResetTimerObjectsCoroutine()
    {
        var oldAngle = spinningThing.eulerAngles;
        var oldPos = platform.position;
        var oldScale = bar.transform.localScale;
        var elapsed = 0f;
        
        while (spinningThing.eulerAngles != _angle || platform.position != _startPos || bar.transform.localScale.y != 0)
        {
            elapsed += Time.deltaTime;
            
            spinningThing.eulerAngles = Vector3.Lerp(oldAngle, _angle, elapsed);
            platform.MovePosition(Vector3.Lerp(oldPos, _startPos, elapsed));
            
            var newScale= bar.transform.localScale;
            newScale.y = Vector3.Lerp(oldScale, Vector3.zero, elapsed).y;
            bar.transform.localScale = newScale;
            
            yield return new WaitForEndOfFrame();
        }
        
        bar.transform.localScale = Vector3.zero;
    }

    private void UpdateSpinningThing()
    {
        var oldAngle = spinningThing.eulerAngles;
        oldAngle.y += Time.deltaTime * 180;
        spinningThing.eulerAngles = oldAngle;
    }
    private void UpdatePlatform(float completion)
    {
        platform.MovePosition(Vector3.Lerp(_startPos, _endPos, completion));
    }
    private void UpdateBar(float completion)
    {
        bar.transform.localScale = new Vector3(1, completion, 1);
    }
}