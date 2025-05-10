using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Health), typeof(Rigidbody2D))]
public class PlayerUnitCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public int   attackDamage = 10;
    public float attackCooldown = 1f;

    Health _health;
    Rigidbody2D _rb;
    Transform _target;
    float _lastAttackTime;

    void Awake()
    {
        _health = GetComponent<Health>();
        _rb     = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        InvokeRepeating(nameof(FindTarget), 0f, 1f);
    }

    void Update()
    {
        if (_health.currentHealth <= 0) return;

        if (_target != null)
        {
            float dist = Vector2.Distance(transform.position, _target.position);
            if (dist > attackRange)
            {
                // move toward enemy
                Vector2 dir = (_target.position - transform.position).normalized;
                _rb.MovePosition(_rb.position + dir * moveSpeed * Time.deltaTime);
            }
            else if (Time.time - _lastAttackTime >= attackCooldown)
            {
                // attack
                var h = _target.GetComponent<Health>();
                if (h != null) h.TakeDamage(attackDamage);
                _lastAttackTime = Time.time;
            }
        }
    }

    void FindTarget()
    {
        // find all EnemyUnitAI instances
        var enemies = Object.FindObjectsByType<EnemyUnitAI>(
            FindObjectsSortMode.None
        );

        float bestDist = float.MaxValue;
        Transform best = null;
        Vector2 myPos = transform.position;

        foreach (var e in enemies)
        {
            float d = Vector2.Distance(myPos, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = e.transform;
            }
        }
        _target = best;
    }
}
