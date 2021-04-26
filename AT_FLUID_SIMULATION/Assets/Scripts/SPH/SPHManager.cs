using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SPHManager : MonoBehaviour
{
    public GameObject prefab;

    public int amount = 200;
    [Range(20f, 100f)]
    public float gassConstant = 50f;

    public float mass = 20.0f;
    public float radius = 0.1f;
    public float smoothingRadius = 0.7f;
    public float viscosity = 0.6f;

	[Range(10f, 200f)]
	public float restDensity = 82.0f;

	[Range(0f, 1f)]
	public float velocityDamping = 0.5f;


	[Range(0f, 5f)]
	public float forceDamping = 0f;

	public float gravityModifier = 1;

    public Vector2 size = new Vector2(10, 10);
    private Vector2 offset = new Vector2(2, 2);

    List<SPHParticle> particles = new List<SPHParticle>();
    Vector2 gravity = Physics.gravity;

    private bool usePressureForce = true;
    private bool useViscosityForce = true;
    private bool useGravityForce = true;


    private void Start()
    {
		RestartSimulation();
    }

    private void Update()
    {
    }

    public void RestartSimulation()
    {
        if (particles.Count != 0)
        {
            particles.Clear();
        }

        int side = (int)Mathf.Sqrt(amount);

        float dx = smoothingRadius * 0.75f;
        for (int i = 0; i < side; i++)
        {
            for (int j = 0; j < side; j++)
            {
                Vector2 pos = new Vector2(i * dx, j * dx) + offset;
                Vector2 vel = Vector2.zero;

                GameObject currentGO = Instantiate(prefab);
                SPHParticle currentParticle = currentGO.AddComponent<SPHParticle>();
                currentParticle.position = pos;
                currentParticle.velocity = vel;
                particles.Add(currentParticle);
            }
        }

    }

	void FixedUpdate()
	{
		Simulate(Time.fixedDeltaTime);
	}

	void Simulate(float dt)
	{
		foreach (SPHParticle p0 in particles)
		{

			p0.density = 0.0f;

			foreach (SPHParticle p1 in particles)
			{

				if (p0 == p1)
					continue;

				float distSqr = (p0.position - p1.position).sqrMagnitude;

				if (distSqr <= smoothingRadius * smoothingRadius)
				{
					p0.density += mass * Kernels.Poly6(distSqr, smoothingRadius);
				}
			}

			p0.density = Mathf.Max(p0.density, restDensity);

			ComputeParticlePressure(p0);
		}

		foreach (SPHParticle p0 in particles)
		{
			p0.force = Vector2.zero;
			Vector2 pressureGradient = Vector2.zero;
			Vector2 viscosityGradient = Vector2.zero;

			foreach (SPHParticle p1 in particles)
			{
				if (p0 == p1)
					continue;

				if (usePressureForce == true)
					pressureGradient += PressureForce(p0, p1);

				if (useViscosityForce == true)
					viscosityGradient += ViscosityForce(p0, p1);
			}

			p0.force += pressureGradient + viscosityGradient;
			ExternalForces(p0);

		}


		foreach (SPHParticle particle in particles)
		{
			ForceBounds(particle);

			Vector2 acceleration = particle.force;
			particle.velocity += 0.5f * (particle.oldAcceleration + acceleration) * dt;

			Vector2 deltaPos = particle.velocity * dt + 0.5f * acceleration * dt * dt;

			particle.position += deltaPos;
			particle.oldAcceleration = acceleration;
		}
	}

	void ComputeParticlePressure(SPHParticle particle)
	{
		particle.pressure = gassConstant * (particle.density - restDensity);
	}

	Vector2 PressureForce(SPHParticle p0, SPHParticle p1)
	{
		float dividend = p0.pressure + p1.pressure;
		float divisor = 2 * p0.density * p1.density;

		Vector2 r = p0.position - p1.position;
		return -mass * (dividend / divisor) * Kernels.PressureSpiky(r, smoothingRadius);
	}

	Vector2 ViscosityForce(SPHParticle p0, SPHParticle p1)
	{
		Vector2 r = p0.position - p1.position;
		Vector2 v = p1.velocity - p0.velocity;
		return viscosity * v * (mass / p0.density) * Kernels.ViscosityLaplacian(r.magnitude, smoothingRadius);
	}

	void ExternalForces(SPHParticle particle)
	{
		if (useGravityForce)
			particle.force += gravity * gravityModifier;
	}

	public List<SPHParticle> GetParticles()
	{
		return particles;
	}


	void ForceBounds(SPHParticle particle)
	{
		Vector2 pos = particle.position;

		if (pos.x < offset.x)
		{
			particle.position.x = offset.x;
			particle.velocity.x = -particle.velocity.x * velocityDamping;
			particle.force.x = -particle.force.x * forceDamping;
		}
		else if (pos.x > size.x + offset.x)
		{
			particle.position.x = size.x + offset.x;
			particle.velocity.x = -particle.velocity.x * velocityDamping;
			particle.force.x = -particle.force.x * forceDamping;
		}

		if (pos.y < offset.y)
		{
			particle.position.y = offset.y;
			particle.velocity.y = -particle.velocity.y * velocityDamping;
			particle.force.y = -particle.force.y * forceDamping;
		}
		else if (pos.y > size.y + offset.y)
		{
			particle.position.y = size.y + offset.y;
			particle.velocity.y = -particle.velocity.y * velocityDamping;
			particle.force.y = -particle.force.y * forceDamping;
		}

	}
}
