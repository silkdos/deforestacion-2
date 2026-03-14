using UnityEngine;

namespace Deforestation.Dinosaurus
{
	public class Stegasaurus : Dinosaur
	{
		#region Fields
		[SerializeField] private float _radiusMovement = 100f;
		#endregion

		#region Properties
		#endregion

		#region Unity Callbacks	
		private void Start()
		{
			_health.OnHealthChanged += Damage;
		}

		void Update()
		{
			if (!_agent.pathPending)
			{ // Asegura que el agente haya calculado el camino
				if (_agent.remainingDistance <= _agent.stoppingDistance)
				{ // Comprueba si la distancia restante es menor que la distancia de parada
					if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
					{
						_anim.SetBool("Fleeing", false);

					}
				}
			}
		}

		#endregion

		#region Private Methods
		private void Damage(float health)
		{
			MoverAdestinoAleatorio();
		}
        void MoverAdestinoAleatorio()
        {
            if (_agent == null) return;
            if (!_agent.isActiveAndEnabled) return;
            if (!_agent.isOnNavMesh) return;

            Vector3 destinoAleatorio = Random.insideUnitSphere * _radiusMovement;
            destinoAleatorio += transform.position;

            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(destinoAleatorio, out hit, _radiusMovement, UnityEngine.AI.NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                _anim.SetBool("Fleeing", true);
            }
        }
        #endregion

        #region Public Methods
        #endregion

    }
}
