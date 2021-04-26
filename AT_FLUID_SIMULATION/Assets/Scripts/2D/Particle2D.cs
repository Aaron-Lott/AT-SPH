using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public GameObject go;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 force;
    public float density;
    public float pressure;

    private void Update()
    {
        transform.position = position;
    }
}
