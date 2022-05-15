using System;
using System.Linq;
using Abstracts;
using Helper;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Fighter
{
    public class Fighter : DamageableObject
    {
        public Card myCard;
        public bool isPlayer;

        [SerializeField] protected NavMeshAgent agent;
        protected DamageableObject target;
        [SerializeField] protected Animator animator;

        public void InitializeFighter(Card card, bool player)
        {
            if (player)
            {
                GameManager.Instance.playerFighters.Add(this);
            }
            else
            {
                GameManager.Instance.opponentFighters.Add(this);
            }

            myCard = card;
            isPlayer = player;
            agent.stoppingDistance = card.attackRange;
            HealthBar = UIManager.Instance.InformationUI.CreateSlider(transform);
            HealthBar.GetComponent<TransformFollower>().target = transform;
            MaxHealth = myCard.health;
            Health = MaxHealth;
        }

        private DamageableObject FindTarget()
        {
            if (isPlayer)
            {
                var nearFighter = GameManager.Instance.opponentFighters.OrderByDescending(x =>
                    Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
                if (!nearFighter) return GameManager.Instance.opponentCastle;
                return nearFighter;
            }
            else
            {
                var nearFighter = GameManager.Instance.playerFighters.OrderByDescending(x =>
                    Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
                if (!nearFighter) return GameManager.Instance.playerCastle;
                return nearFighter;
            }
        }

        private void OnDestroy()
        {
            if (isPlayer)
            {
                GameManager.Instance.playerFighters.Remove(this);
            }
            else
            {
                
                GameManager.Instance.opponentFighters.Remove(this);
            }
        }

        private float _attackTime;

        public void Stop()
        {
            agent.SetDestination(transform.position);
            animator.SetBool("Run",false);
            target = null;
        }
        protected void Update()
        {
            if (GameManager.Instance.gameIsEnded) return;
            var isWalking = agent.stoppingDistance < agent.remainingDistance;
            animator.SetBool("Run",isWalking);
            if (!isWalking)
            {
                if (!target)
                {
                    target = FindTarget();
                    agent.SetDestination(target.transform.position);
                }
                else
                {
                    if (Vector3.Distance(target.transform.position, transform.position) <= myCard.attackRange)
                    {
                        if (_attackTime < 0)
                        {
                            Attack();
                            _attackTime = myCard.attackCoolDown;
                        }
                        else
                        {
                            _attackTime -= Time.deltaTime;
                        }
                        
                    }
                    else
                    {
                        agent.SetDestination(target.transform.position);
                    }
                }
            }
        }

        public void Attack()
        {
            var delta = target.transform.position - transform.position;
            var angle = Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
            transform.eulerAngles = transform.eulerAngles.SetY(angle);
            animator.SetTrigger("Attack");
        }

        public void AttackAnimationEvent()
        {
            if (target == null) return;
            target.GetDamage(myCard.damage);
        }

        public override void GetDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                OnDead();
            }
        }

        public override void OnDead()
        {
            GameObject.Destroy(gameObject);
        }
    }
}