using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kernels
{
	static public float Poly6(float distSqr, float h)
	{
		float coef = 315f / (64f * Mathf.PI * Mathf.Pow(h, 9));
		float hSqr = h * h;

		if (hSqr < distSqr)
			return 0f;

		return coef * Mathf.Pow(hSqr - distSqr, 3);
	}

	static public Vector2 PressureSpiky(Vector2 r, float h)
	{
		float coef = 15f / (Mathf.PI * Mathf.Pow(h, 6));
		float dist = r.magnitude;

		if(dist > h)
			return Vector3.zero;

		return -coef * r.normalized * Mathf.Pow(h - dist, 3);
	}

	static public float ViscosityLaplacian(float r, float h)
	{
		if (h < r)
			return 0f;

		float coef = 15f / (2 * Mathf.PI * Mathf.Pow(h, 3));
		return coef * (h - r);
	}

	/*static public Vector2 GradientSpiky(Vector2 r, float h)
	{
		float coef = 15f / (Mathf.PI * Mathf.Pow(h, 6));
		float dist = r.magnitude;

		if (h < dist)
			return Vector3.zero;

		return -coef * r.normalized * Mathf.Pow(h - dist, 2);
	}

	static public float ViscosityLaplacian(float r, float h)
	{
		if (h < r)
			return 0f;

		float coef = 45f / (Mathf.PI * Mathf.Pow(h, 6));
		return coef * (h - r);
	}*/
}