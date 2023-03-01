using System.Collections;
using UnityEngine;

public class StaticKeeperWithRotation : StaticKeeper
{
    [SerializeField] private float _timerToRotate;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Lamp _lamp;

    private float _timeElapsed;
    private bool _isFacingLeft;
    
    private void Update()
    {
        if (!IsStunned && transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        }
        
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= _timerToRotate)
        {
            StartCoroutine(RotateCoroutine());
            _timeElapsed = 0;
        }
        
        if (IsStunned)
            _lamp.Disable();

        if (!IsStunned)
            _lamp.Enable();
    }
    
    private IEnumerator RotateCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _rotationDuration)
        {
            timeElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, _isFacingLeft ? Quaternion.Euler(transform.rotation.x, 180.0f, transform.rotation.z) : Quaternion.identity, Time.deltaTime * _rotationSpeed);
            yield return null;
        }

        _isFacingLeft = !_isFacingLeft;
    }
}
