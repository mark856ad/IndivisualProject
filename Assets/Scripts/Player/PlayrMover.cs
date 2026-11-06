using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayrMover : MonoBehaviour
{
   [SerializeField] private float _moveSpeed = 5f;
   
   
   private Vector2 _moveInput;

   private void Update()
   {
      Move();
   }

   public void OnMove(InputValue value)
   {
      _moveInput = value.Get<Vector2>();
   }

   private void Move()
   {
      Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
      transform.Translate(direction * _moveSpeed * Time.deltaTime);
   }
   
}
