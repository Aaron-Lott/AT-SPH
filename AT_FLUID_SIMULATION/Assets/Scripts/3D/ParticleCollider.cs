using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ParticleCollider : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Up;
    public Vector3 Right;
    public Vector3 Forward;
    public Vector2 Scale;

    private void Start()
    {
        Position = transform.position;
        Scale = transform.localScale;
        Right = transform.right;
        Up = transform.up;
        Forward = transform.forward;
    }
}
