using System;
using UnityEngine;

public interface IDamagalbe
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamagalbe
{
    public UICondition uiCondition;
    private float lastYPosition;
    public float fallHeightThreshold = 5f;
    public int fallDamageAmount = 10;

    Condition health { get { return uiCondition.health; } }

    public event Action onTakeDamage;

    private void Start()
    {
        lastYPosition = transform.position.y;
    }

    private void Update()
    {
        if (health.curValue == 0f)
        {
            Die();
        }

        if (transform.position.y > lastYPosition)
        {
            lastYPosition = transform.position.y;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float fallDistance = lastYPosition - transform.position.y;

        lastYPosition = transform.position.y;

        if (fallDistance > fallHeightThreshold)
        {
            int damage = fallDamageAmount + Mathf.FloorToInt(fallDistance / 5f) * 5;
            TakePhysicalDamage(damage);
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    private void Die()
    {
        health.Add(100f);
        transform.position = new Vector3(70, 3, 65);
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }
}
