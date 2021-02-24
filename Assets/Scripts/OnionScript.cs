using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionScript : MonoBehaviour
{
  [SerializeField]
  [Range(1f, 360f)]
  float angularSpeed = 10f;
  Camera mainCamera;

  void Start()
  {
    mainCamera = Camera.main;
  }

  void FixedUpdate()
  {
    Vector3 position = transform.position;
    Vector3 direction = mainCamera.transform.position - position;
    Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, angularSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
    transform.LookAt(position + newDirection);
  }
}
