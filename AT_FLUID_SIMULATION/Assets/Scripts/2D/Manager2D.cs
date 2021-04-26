using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager2D : MonoBehaviour
{
    public GameObject prefab;
    public int amount;
    public int perRow;

    public Vector2 size = new Vector2(10, 10);

    private Particle2D[] particles;

    private static Vector2 gravity = new Vector2(0.0f, 2000 * -9.8f); 
    private static float restDensity = 1000.0f; 
    private static float gasConstant = 2000.0f; 
    private static float kernelRadius = 1.0f; 
    private static float kernelRadiusSquared = kernelRadius * kernelRadius;
    private static float mass = 65.0f; 
    private static float viscosity = 250.0f;
    private static float deltaTime = 0.0008f; 

    // smoothing kernels defined in Müller and their gradients
    private static float POLY6 = 315.0f / (65.0f * Mathf.PI * Mathf.Pow(kernelRadius, 9.0f));
    private static float SPIKY_GRAD = -45.0f / (Mathf.PI * Mathf.Pow(kernelRadius, 6.0f));
    private static float VISC_LAP = 45.0f / (Mathf.PI * Mathf.Pow(kernelRadius, 6.0f));

    // simulation parameters
    static float boundryEpsilon = kernelRadius;
    static float boundDamping = -0.5f;

    private void Start()
    {
        InitSPH();
    }

    private void Update()
    {
        ComputeDensityPressure();
        ComputeForces();
        Integrate();
    }

    private void InitSPH()
    {
        particles = new Particle2D[amount];

        for (int i = 0; i < amount; i++)
        {
            float x = (i % perRow) + Random.Range(-0.1f, 0.1f);
            float y = (2 * kernelRadius) + (i / perRow) * 1.1f;

            GameObject currentGO = Instantiate(prefab);
            Particle2D currentParticle = currentGO.AddComponent<Particle2D>();
            particles[i] = currentParticle;

            currentGO.transform.localScale = Vector2.one * kernelRadius;
            currentGO.transform.position = new Vector3(x, y, 0);

            currentParticle.go = currentGO;
            currentParticle.position = currentGO.transform.position;

        }
    }

    void ComputeDensityPressure()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].density = 0.0f;

            for (int j = 0; j < particles.Length; j++)
            {
                Vector2 rij = particles[j].position - particles[i].position;
                float r2 = rij.sqrMagnitude;

                if (r2 < kernelRadiusSquared)
                {
                    // this computation is symmetric
                    particles[i].density += mass * POLY6 * Mathf.Pow(kernelRadiusSquared - r2, 3.0f);
                }
            }
            particles[i].pressure = gasConstant * (particles[i].density - restDensity);
        }
    }

    void ComputeForces()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            Vector2 fpress = new Vector2(0.0f, 0.0f);
            Vector2 fvisc = new Vector2(0.0f, 0.0f);
            for (int j = 0; j < particles.Length; j++)
            {
                if (particles[i] == particles[j])
                    continue;

                Vector2 rij = particles[j].position - particles[i].position;
                float r = rij.magnitude;

                if (r < kernelRadius)
                {
                    // compute pressure force contribution
                    fpress += -rij.normalized * mass * (particles[i].pressure + particles[j].pressure) / (2.0f * particles[j].density) * SPIKY_GRAD * Mathf.Pow(kernelRadius - r, 2.0f);
                    // compute viscosity force contribution
                    fvisc += viscosity * mass * (particles[j].velocity - particles[i].velocity) / particles[j].density * VISC_LAP * (kernelRadius - r);
                }
            }
            Vector2 fgrav = gravity * particles[i].density;
            particles[i].force = fpress + fvisc + fgrav;
        }
    }

    void Integrate()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            // forward Euler integration
            particles[i].velocity += deltaTime * particles[i].force / particles[i].density;
            particles[i].position += deltaTime * particles[i].velocity;

            // enforce boundary conditions
            if (particles[i].position.x - boundryEpsilon < 2)
            {
                particles[i].velocity.x *= boundDamping;
                particles[i].position.x = 2 + boundryEpsilon;
            }
            if (particles[i].position.x + boundryEpsilon > size.x)
            {
                particles[i].velocity.x *= boundDamping;
                particles[i].position.x = size.x - boundryEpsilon;
            }
            if (particles[i].position.y - boundryEpsilon < 2)
            {
                particles[i].velocity.y *= boundDamping;
                particles[i].position.y = 2 + boundryEpsilon;
            }
            if (particles[i].position.y + boundryEpsilon > size.y)
            {
                particles[i].velocity.y *= boundDamping;
                particles[i].position.y = size.y - boundryEpsilon;
            }
        }
    }
}
