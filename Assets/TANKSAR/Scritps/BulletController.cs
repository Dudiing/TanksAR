using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 10f;
    public float explosionRadius = 5f; // Radio del OverlapSphere
    public AudioClip collisionSound; // Asigna el clip de audio desde el Inspector
    private AudioSource audioSource;

    void Start()
    {
        // Obtener el AudioSource adjunto al objeto
        audioSource = GetComponent<AudioSource>();

        // Destruir la bala después de 10 segundos
        Destroy(gameObject, 10f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Reproducir el sonido de colisión
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }

        // Crear el OverlapSphere para buscar otros objetos en el radio de la explosión
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            //Debug.Log("Impactado con: " + hitCollider.gameObject.name);
            TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
            if (tankHealth != null)
            {
                //Debug.Log("Impactado 2 con: " + hitCollider.gameObject.name);
                // Calcular el daño basado en la distancia al punto de impacto
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float damageAmount = damage * (1 - (distance / explosionRadius));
                tankHealth.TakeDamage(damageAmount);
            }
        }

        // Destruir la bala después de que el sonido haya terminado
        Destroy(gameObject, 0.5f);
    }
}
