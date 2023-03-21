using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : Consumer
{
    private GameController _gameController;
    private Transform _projectileParent;

    private enum Target
    {
        First,
        Last,
        Closest,
        Farthest,
        Strongest,
        Weakest,
    }
    [SerializeField] private Target targeting = Target.First; 
    [SerializeField] private Transform towerHead;
    [SerializeField] private GameObject projectile;
    
    private Animator _animator;
    private bool _isAttacking = false;
    
    private List<Transform> _targets;
    private Transform _currentTarget;
    private float _range;

    private void Awake()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        _projectileParent = GameObject.FindWithTag("EntityController").transform.Find("Projectiles");
        _animator = GetComponent<Animator>();
        _targets = new List<Transform>();
        _range = GetComponent<CircleCollider2D>().radius;
    }
    protected void Update()
    {
        if (!isOn || _isAttacking) return;
        Aim();
        if (IsCoolingDown) return;
        Attack();
    }

    private void Attack()
    {
        if (!isOn || IsCoolingDown || _isAttacking || ReferenceEquals(_currentTarget, null)) return;
        
        _animator.SetTrigger("Attack");
    }
    protected override void Idle()
    {
        _isAttacking = false;
        base.Idle();
    }
    private void Aim()
    {
        _currentTarget = GetTarget();
        if (ReferenceEquals(_currentTarget, null)) return;
        towerHead.up = (_currentTarget.position - towerHead.position).normalized;
    }
    private void Shoot()
    {
        if (_currentTarget == null) return;
        _isAttacking = true;
        var towerHeadPos = towerHead.position;
        var instance = Instantiate(
            projectile, towerHeadPos, Quaternion.identity, _projectileParent);
        var behaviour = instance.GetComponent<Projectile>();
        behaviour.targetPosition = 
            towerHeadPos + _range * (_currentTarget.position - towerHeadPos).normalized;
    }

    private Transform GetTarget()
    {
        return targeting switch
        {
            Target.First => GetFirst(),
            Target.Last => GetLast(),
            Target.Closest => GetClosest(),
            Target.Farthest => GetFarthest(),
            _ => GetFirst()
        };
    }
    private Transform GetFirst()
    {
        return _targets.FirstOrDefault();
    }
    private Transform GetLast()
    {
        return _targets.LastOrDefault();
    }
    private Transform GetClosest()
    {
        return _targets.OrderBy(x => Vector3.Distance(towerHead.position, x.position)).FirstOrDefault();
    }
    private Transform GetFarthest()
    {
        return _targets.OrderBy(x => Vector3.Distance(towerHead.position, x.position)).LastOrDefault();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        _targets.Add(col.transform);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        _targets.Remove(col.transform);
    }
}
