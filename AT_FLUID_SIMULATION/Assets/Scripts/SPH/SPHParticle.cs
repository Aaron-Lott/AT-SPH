using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialHash;

public class SPHParticle: MonoBehaviour, ISpatialHashObject2D
{
	public Vector2 position = Vector2.zero;
	public Vector2 oldPosition = Vector2.zero;
	public Vector2 velocity = Vector2.zero;
	public Vector2 oldAcceleration = Vector2.zero;
	public Vector2 force = Vector2.zero;
	public float pressure = 0;
	public float density = 0;

	public List<Particle> neighbours = new List<Particle>();

	private void Update()
	{
		transform.position = position;
	}

	public Vector2 GetPosition()
	{
		return transform.position;
	}

	public float GetRadius()
	{
		return 0.5f;
	}
}