using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class EnemyUnitAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    private Transform _target;
    private float _lastAttackTime;
    private enum State { Idle, Moving, Attacking }
    private State _state = State.Idle;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        InvokeRepeating(nameof(FindTarget), 0f, 1f);
    }

    void Update()
    {
        switch (_state)
        {
            case State.Idle:
                if (_target != null)
                    _state = State.Moving;
                break;

            case State.Moving:
                if (_target == null) { _state = State.Idle; return; }
                Vector2 dir = (_target.position - transform.position).normalized;
                _rb.MovePosition(_rb.position + dir * moveSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, _target.position) <= attackRange)
                    _state = State.Attacking;
                break;

            case State.Attacking:
                if (_target == null) { _state = State.Idle; return; }
                if (Time.time - _lastAttackTime >= attackCooldown)
                {
                    var h = _target.GetComponent<Health>();
                    if (h != null) h.TakeDamage(attackDamage);
                    _lastAttackTime = Time.time;
                }
                if (Vector2.Distance(transform.position, _target.position) > attackRange)
                    _state = State.Moving;
                break;
        }
    }

    void FindTarget()
    {
        // 1) Grab all your player units via their component (fast, unsorted)
        var unitComponents = Object.FindObjectsByType<SelectableUnit>(
            FindObjectsSortMode.None
        );

        // 2) Grab all your player buildings via their component
        var buildingComponents = Object.FindObjectsByType<SelectableBuilding>(
            FindObjectsSortMode.None
        );

        // 3) Build a list of all potential targets
        List<Transform> candidates = new List<Transform>(unitComponents.Length + buildingComponents.Length);
        foreach (var su in unitComponents)
            candidates.Add(su.transform);
        foreach (var sb in buildingComponents)
            candidates.Add(sb.transform);

        // 4) Pick the closest
        float bestDist = float.MaxValue;
        Transform best    = null;
        Vector2 myPos     = transform.position;
        foreach (var t in candidates)
        {
            float d = Vector2.Distance(myPos, t.position);
            if (d < bestDist)
            {
                bestDist = d;
                best     = t;
            }
        }

        _target = best;
    }
}
