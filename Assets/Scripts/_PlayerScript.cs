using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerScript : MonoBehaviour
{
  [SerializeField]
  [Range(1f, 20f)]
  float movementSpeed = 10f;
  [SerializeField]
  [Range(20f, 100f)]
  float grappleRange = 30f;
  [SerializeField]
  LayerMask ground;
  [SerializeField]
  LayerMask notPlayer;
  CharacterController controller;
  Rigidbody rb;
  Camera mainCamera;
  SpringJoint hook;
  bool grounded = false;
  bool hooking = false;
  GameObject hand;
  LineRenderer lineRenderer;
  Vector3 grapplePoint;
  Vector3 grappledAt;
  float distance = 9f;

  void Start()
  {
    controller = GetComponent<CharacterController>();
    mainCamera = Camera.main;
    rb = GetComponent<Rigidbody>();
    hand = GameObject.Find("Hand");
    lineRenderer = GetComponent<LineRenderer>();
  }

  void Update()
  {
    #region Keyboard Input
    if (Input.GetKeyDown(KeyCode.R))
    {
      rb.velocity = Vector3.zero;
      transform.position = Vector3.up * 3f;
    }
    if (Input.GetKeyDown(KeyCode.F))
    {
      RaycastHit hit;
      if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, grappleRange, notPlayer))
      {
        StartCoroutine(HookOn(hit.point));
      }
    }
    if (Input.GetKeyUp(KeyCode.F))
    {
      StartCoroutine(HookOff());
    }
    #endregion

    #region Scroll
    float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
    if (scroll != 0 && hook != null)
    {
      distance += scroll * 3f;
      distance = Mathf.Clamp(distance, 1f, 10f);
      hook.maxDistance = (Vector3.Distance(grappledAt, grapplePoint) / 10f) * distance;
      hook.minDistance = 0f;
    }
    #endregion

    if (hooking)
    {
      lineRenderer.SetPosition(0, hand.transform.position);
      lineRenderer.SetPosition(1, grapplePoint);
    }
  }

  void FixedUpdate()
  {
    if (hook != null)
    {
      Debug.Log(hook.maxDistance);
    }

    RaycastHit hit;
    Debug.DrawRay(transform.position, Vector3.down * 1.1f);
    if ((!hooking) && Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, ground))
    {
      // grounded = true;
      // controller.enabled = true;
      // rb.isKinematic = true;
    }
    else
    {
      // grounded = false;
      // controller.enabled = false;
      // rb.isKinematic = false;
    }

    if (controller != null && controller.enabled)
    {
      Vector3 vertical = Input.GetAxisRaw("Vertical") * transform.forward;
      Vector3 horizontal = Input.GetAxisRaw("Horizontal") * transform.right;

      Vector3 direction = vertical + horizontal;
      direction.y = 0f;
      direction = Vector3.ClampMagnitude(direction, 1f) * movementSpeed;

      Vector3 move = new Vector3(direction.x, Physics.gravity.y, direction.z) * Time.deltaTime;
      controller.Move(move);
    }
  }

  IEnumerator HookOn(Vector3 point)
  {
    yield return new WaitForFixedUpdate();
    hooking = true;
    distance = 9f;
    hook = gameObject.AddComponent<SpringJoint>();
    grapplePoint = point;
    grappledAt = mainCamera.transform.position;
    hook.autoConfigureConnectedAnchor = false;
    hook.connectedAnchor = grapplePoint;
    hook.maxDistance = (Vector3.Distance(grappledAt, grapplePoint) / 10f) * distance;
    hook.minDistance = 0.01f;
    hook.spring = 1000f;
    hook.damper = 1000f;
    hook.massScale = 1f;
    lineRenderer.enabled = true;
  }

  IEnumerator HookOff()
  {
    yield return new WaitForFixedUpdate();
    hooking = false;
    Destroy(hook);
    lineRenderer.enabled = false;
  }
}
