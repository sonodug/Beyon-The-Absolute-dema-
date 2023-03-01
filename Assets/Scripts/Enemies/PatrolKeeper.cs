using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolMovement), typeof(BoxCollider))]
public class PatrolKeeper : Enemy
{
    [SerializeField] private float _health;
    [SerializeField] private float _stunForce;
    [SerializeField] private float _timerToRotate;
    [SerializeField] private float _stunnedDuration;
    [SerializeField] private float _blockRotationDuration;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private List<Lamp> _lamps;
    
    private BoxCollider _collider;
    private Rigidbody _rigidbody;
    private Transform _prevState;

    private PatrolMovement _movement;
    private float _timeElapsed;
    private bool _isFacingLeft;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _movement = GetComponent<PatrolMovement>();
    }

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
        {
            foreach (var lamp in _lamps)
            {
                lamp.Disable();
            }
        }

        if (!IsStunned)
        {
            foreach (var lamp in _lamps)
            {
                lamp.Disable();
            }
        }
    }

    private void OnEnable()
    {
        _player.Stunned += OnStunned;
    }

    private void OnDisable()
    {
        _player.Stunned -= OnStunned;
    }
    
    protected override void OnDied() {}

    protected override void OnStunned()
    {
        IsStunned = true;
        _prevState = transform;
        _rigidbody.AddForce(Vector3.forward * (_stunForce * 100.0f));
        StartCoroutine(StunCoroutine());
    }

    protected override void InitBehaviours()
    {
        Movement = _movement;
    }
    
    private IEnumerator StunCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _stunnedDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(_prevState.position.x, _prevState.position.y, 0.0f);
        IsStunned = false;
        _rigidbody.isKinematic = true;

        StartCoroutine(BlockRotationCoroutine());
    }
    
    private IEnumerator BlockRotationCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _blockRotationDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        _rigidbody.isKinematic = false;
    }
    
    private IEnumerator RotateCoroutine()
    {
        float timeElapsed = 0;
        
        while (timeElapsed < _rotationDuration)
        {
            timeElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, _isFacingLeft ? Quaternion.Euler(transform.rotation.x, 90.0f, transform.rotation.z) : Quaternion.identity, Time.deltaTime * _rotationSpeed);
            yield return null;
        }

        _isFacingLeft = !_isFacingLeft;
    }
}
