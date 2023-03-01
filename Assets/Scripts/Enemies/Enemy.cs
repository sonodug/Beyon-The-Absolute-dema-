using System.Threading;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected Player _player;
    
    protected IDamageable Health;
    protected IMovable Movement;
    protected IWeaponable Weapon;
    public bool IsStunned;

    protected abstract void OnDied();
    protected abstract void OnStunned();
    public abstract void ApplyStun();
    protected abstract void InitBehaviours();
}
