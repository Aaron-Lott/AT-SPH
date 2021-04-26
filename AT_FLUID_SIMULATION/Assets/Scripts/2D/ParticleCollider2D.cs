using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollider2D : MonoBehaviour
{
    public Vector2 Position;
    public Vector2 Up;
    public Vector2 Right;
    public Vector2 Scale;

    private void Start()
    {
        Position = transform.position;
        Scale = transform.localScale;
        Right = transform.right;
        Up = transform.up;
    }
}
