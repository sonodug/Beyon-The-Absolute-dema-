using DG.Tweening;
using UnityEngine;

public class PatrolMovement : MonoBehaviour, IMovable
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _duration;

    private Vector3[] _waypointsPosition;
    public Vector3[] WaypointsPosition => _waypointsPosition;

    private void Awake()
    {
        _waypointsPosition = ConvertToVectorArray(_waypoints);
        Move();
    }

    public void Move()
    {
        Tween tween = transform.DOPath(_waypointsPosition, _duration, PathType.Linear).SetOptions(true);
        tween.SetLoops(-1);
    }

    private Vector3[] ConvertToVectorArray(Transform[] array)
    {
        Vector3[] vectorArray = new Vector3[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            vectorArray[i] = array[i].position;
        }

        return vectorArray;
    }
}