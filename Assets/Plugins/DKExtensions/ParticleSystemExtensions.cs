using UnityEngine;

public static class ParticleSystemExtensions
{

    public static void EmissionEnable(this ParticleSystem ps, bool enabled = true)
    {
        var em = ps.emission;
        em.enabled = enabled;
    }

    public static void EmissionDisable(this ParticleSystem ps)
    {
        ps.EmissionEnable(false);
    }
}
