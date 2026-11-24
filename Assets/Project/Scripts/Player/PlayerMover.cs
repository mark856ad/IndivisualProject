using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private InputActionReference _moveAction;
    
    private Rigidbody _rigidbody;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        _rigidbody.linearVelocity = direction * _moveSpeed;
    }
}
