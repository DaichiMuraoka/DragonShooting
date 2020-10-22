using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour
{
    ParticleSystem particle;
    private void Start()
    {
        particle = this.GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (particle.isStopped)
        {
            Destroy(this.gameObject);
        }
    }
}
