using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deforestation;

public class WaterDamage : MonoBehaviour
{
    public float damagePerSecond = 10f;

    private void OnTriggerStay(Collider other)
    {
        // Ignorar las rocas
        if (other.GetComponentInParent<Transform>().CompareTag("Rock"))
            return;

        // Buscar HealthSystem en el objeto o en su padre
        HealthSystem health = other.GetComponentInParent<HealthSystem>();

        if (health == null)
            return;

        health.TakeDamage(damagePerSecond * Time.deltaTime);
    }
}