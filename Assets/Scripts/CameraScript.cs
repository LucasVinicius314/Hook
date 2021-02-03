using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
  [SerializeField]
  [Range(1f, 10f)]
  float sensitivity = 3f;
  float pitch = 0f;
  GameObject player;

  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    player = GameObject.Find("Player");
  }

  void Update()
  {
    player.transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensitivity);
    pitch -= Input.GetAxisRaw("Mouse Y") * sensitivity;

    pitch = Mathf.Clamp(pitch, -90f, +90f);

    transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
  }
}
