using System;
using UnityEngine;

namespace Deforestation
{

    public class HealthSystem : MonoBehaviour
    {
        public event Action<float> OnHealthChanged;
        public event Action OnDeath;

        [SerializeField]
        private float _maxHealth = 100f;
        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float damage)
		{
			_currentHealth -= damage;
			OnHealthChanged?.Invoke(_currentHealth);

			if (_currentHealth <= 0)
			{
				Die();
			}
		}

		public void Heal(float amount)
		{
			_currentHealth += amount;
			_currentHealth = Mathf.Min(_currentHealth, _maxHealth);
			OnHealthChanged?.Invoke(_currentHealth);
		}

		public void SetHealth(float value)
		{
			_currentHealth = value;
			_currentHealth = Mathf.Min(_currentHealth, _maxHealth);
			OnHealthChanged?.Invoke(_currentHealth);
		}

		private void Die()
		{
            if (_currentHealth > 0)
                return;

            OnDeath?.Invoke();
        }
	}

}