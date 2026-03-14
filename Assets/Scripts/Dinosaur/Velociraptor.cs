using System;
using UnityEngine;
using UnityEngine.AI;

namespace Deforestation.Dinosaurus
{
    public class Velociraptor : Dinosaur
    {
        #region Fields

        [SerializeField] private float _machineDetection = 120f;
        [SerializeField] private float _playerDetection = 50f;
        [SerializeField] private float _attackDistance = 3f;

        [SerializeField] private float _attackTime = 1.5f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _fleeTime = 5f;
        [SerializeField] private float _wanderRadius = 100f;
        [SerializeField] private float _wanderTimer = 5f;

        private float _wanderCooldown;

        private float _fleeTimer;

        private float _attackCooldown;

        private Transform _player => GameController.Instance.Player;
        private Vector3 _machinePosition => GameController.Instance.MachineController.transform.position;

        private bool _chase;
        private bool _attack;
        private bool _flee;

        #endregion

        #region Unity

        private float _lastHealth;

        private void Start()
        {
            _wanderCooldown = UnityEngine.Random.Range(1f, 10f);
            _lastHealth = _health.CurrentHealth;
            _health.OnHealthChanged += OnHealthChanged;
        }

        private void Update()
        {
            float machineDistance = Vector3.Distance(transform.position, _machinePosition);
            float playerDistance = Vector3.Distance(transform.position, _player.position);
            Vector3 dir = _agent.velocity;

            if (dir.sqrMagnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
            }

            //Huir de máquina
            if (machineDistance < _machineDetection)
            {
                Flee();
                return;
            }

            //Detectar player
            if (!_flee && !_chase && !_attack && playerDistance < _playerDetection)
            {
                ChasePlayer();
                return;
            }

            //CHASE PLAYER
            if (_chase)
            {
                _agent.SetDestination(_player.position);

                if (playerDistance < _attackDistance)
                {
                    Attack();
                    return;
                }

                if (playerDistance > _playerDetection)
                {
                    Idle();
                    return;
                }
            }

            //ATTACK
            if (_attack)
            {
                _attackCooldown -= Time.deltaTime;

                if (_attackCooldown <= 0)
                {
                    _attackCooldown = _attackTime;
                    GameController.Instance.PlayerHealth.TakeDamage(_attackDamage);
                }

                if (playerDistance > _attackDistance)
                {
                    ChasePlayer();
                    return;
                }
            }

            // FLEE
            if (_flee)
            {
                _fleeTimer -= Time.deltaTime;

                if (_fleeTimer <= 0)
                {
                    Idle();
                }

                return;
            }

            // WANDER
            if (!_chase && !_attack && !_flee)
            {
                _wanderCooldown -= Time.deltaTime;

                if (_wanderCooldown <= 0)
                {
                    Wander();
                    _wanderCooldown = _wanderTimer;
                }
            }

            if (!_chase && !_attack && !_flee && _agent.remainingDistance < 0.5f)
            {
                _anim.SetBool("Walk", false);
            }
        }

        private void OnHealthChanged(float currentHealth)
        {
            if (currentHealth < _lastHealth)
            {
                Flee();
            }

            _lastHealth = currentHealth;
        }

        #endregion

        #region States

        private void Idle()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", false);

            _chase = false;
            _attack = false;
            _flee = false;

            _agent.isStopped = true;

            if (!_chase && !_attack && !_flee)
            {
                _wanderCooldown -= Time.deltaTime;

                if (_wanderCooldown <= 0)
                {
                    Wander();
                    _wanderCooldown = _wanderTimer;
                }
            }
        }

        private void ChasePlayer()
        {
            _anim.SetBool("Run", true);
            _anim.SetBool("Attack", false);

            _agent.isStopped = false;
            _agent.SetDestination(_player.position);

            _chase = true;
            _attack = false;
            _flee = false;
        }

        private void Attack()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", true);

            _agent.isStopped = true;

            _chase = false;
            _attack = true;
            _flee = false;
        }
        private void Wander()
        {
            _agent.isStopped = false;

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, _wanderRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);

                _anim.SetBool("Walk", true);
                _anim.SetBool("Attack", false);
            }
        }
        private void Flee()
        {
            _fleeTimer = _fleeTime;
            // dirección opuesta a la máquina
            Vector3 dir = (transform.position - _machinePosition).normalized;

            // punto al que huir
            Vector3 fleeTarget = transform.position + dir * 20f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, 20f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }

            _anim.SetBool("Run", true);
            _anim.SetBool("Attack", false);

            _agent.isStopped = false;

            _chase = false;
            _attack = false;
            _flee = true;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _machineDetection);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _playerDetection);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }
    }
}