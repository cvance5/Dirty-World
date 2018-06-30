using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleRemesher : MonoBehaviour
{
    public ParticleSystem ParticleSource;
    public ParticleSystemRenderer ParticleRenderer;

    public Transform ParticleRoot;

    public float TriggerTime;
    private float _time;

    private void Start()
    {
        _time = 0;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > TriggerTime)
        {
            Remesh();
            Destroy(this);
        }
    }

    private void Remesh()
    {
        var main = ParticleSource.main;

        var particleBuffer = new ParticleSystem.Particle[main.maxParticles];
        var numParticles = ParticleSource.GetParticles(particleBuffer);

        for (int i = 0; i < numParticles; i++)
        {
            var particle = particleBuffer[i];

            var meshObject = new GameObject($"Particle {i}");
            meshObject.transform.SetParent(ParticleRoot);
            meshObject.transform.LocalReset();

            var filter = meshObject.AddComponent<MeshFilter>();
            var renderer = meshObject.AddComponent<MeshRenderer>();

            filter.mesh = ParticleRenderer.mesh;
            renderer.material = ParticleRenderer.material;

            if (main.simulationSpace == ParticleSystemSimulationSpace.Local) meshObject.transform.localPosition = particle.position;
            else meshObject.transform.position = particle.position;

            meshObject.transform.localRotation = Quaternion.Euler(particle.rotation3D);
            meshObject.transform.localScale = particle.GetCurrentSize3D(ParticleSource);
        }

        Destroy(ParticleSource.gameObject);
    }
}
