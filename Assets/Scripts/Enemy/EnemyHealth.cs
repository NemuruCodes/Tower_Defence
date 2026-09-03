using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;

    public event Action OnDeath;

    [SerializeField] private int resourceReward = 50;

    private void Awake() => currentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            Die();
        }
        Debug.Log(currentHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        ResourceManager.Instance.Add(resourceReward);
        Destroy(gameObject);
    }

}
