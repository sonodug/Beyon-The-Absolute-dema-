using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class StaticKeeper : Enemy
{
    [SerializeField] private float _health;
    [SerializeField] private float _stunForce;
    [SerializeField] private float _stunnedDuration;
    [SerializeField] private float _blockRotationDuration;

    private BoxCollider _collider;
    private Rigidbody _rigidbody;
    private Transform _prevState;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsStunned && transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
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
    
    protected override void OnDied() { }

    protected override void OnStunned()
    {
        
    }

    protected override void InitBehaviours()
    {
        
    }

    public override void ApplyStun()
    {
        IsStunned = true;
        _prevState = transform;
        _rigidbody.AddForce(Vector3.forward * (_stunForce * 100.0f));
        StartCoroutine(StunCoroutine());
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
}
