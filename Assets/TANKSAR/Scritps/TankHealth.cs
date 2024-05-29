using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public Image healthCircle; // Asigna la imagen circular desde el Inspector

    public delegate void TankDestroyedAction(GameObject tank);
    public event TankDestroyedAction OnTankDestroyed;

    private bool SeguirJugando = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (!SeguirJugando)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            UpdateHealthUI();

            if (currentHealth <= 0f)
            {
                Die();
            }
        }
    }

    void UpdateHealthUI()
    {
        if (!SeguirJugando)
        {
            if (healthCircle != null)
            {
                healthCircle.fillAmount = currentHealth / maxHealth;
            }
        }
    }

    void Die()
    {
        if (!SeguirJugando)
        {
            SeguirJugando = true;
            // Lógica para cuando el tanque muere (desactivar el tanque, mostrar explosión, etc.)
            Debug.Log(gameObject.name + " has been destroyed!");
            OnTankDestroyed?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }
}
