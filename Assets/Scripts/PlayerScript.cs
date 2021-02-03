using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
  [SerializeField]
  [Range(1f, 20f)]
  float movementSpeed = 10f;
  [SerializeField]
  [Range(20f, 100f)]
  float grappleRange = 30f;
  [SerializeField]
  [Range(0f, 1f)]
  float hookBounciness = 0.6f;
  [SerializeField]
  LayerMask ground;
  [SerializeField]
  LayerMask notPlayer;
  CharacterController controller;
  Rigidbody rb;
  Camera mainCamera;
  ConfigurableJoint hook;
  bool grounded = false;
  bool hooking = false;
  GameObject hand;
  UIScript uIScript;
  LineRenderer lineRenderer;
  Vector3 grapplePoint;
  Vector3 grappledAt;
  float distanceIndex = 10f;
  float distanceAtGrapple = 0f;

  void Start()
  {
    controller = GetComponent<CharacterController>();
    mainCamera = Camera.main;
    rb = GetComponent<Rigidbody>();
    hand = GameObject.Find("Hand");
    lineRenderer = GetComponent<LineRenderer>();
    uIScript = GameObject.Find("Canvas").GetComponent<UIScript>();
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
      distanceIndex += scroll * 3f;
      distanceIndex = Mathf.Clamp(distanceIndex, 1f, 10f);
      float newDistanceIndex = distanceIndex / 10f;
      hook.linearLimit = new SoftJointLimit()
      {
        limit = distanceAtGrapple * newDistanceIndex,
        bounciness = hookBounciness
      };
    }
    #endregion

    if (hooking)
    {
      lineRenderer.SetPosition(0, hand.transform.position);
      lineRenderer.SetPosition(1, grapplePoint);
    }

    uIScript.SetSpeed(rb.velocity.magnitude);
  }

  void FixedUpdate()
  {
    RaycastHit hit;
    Debug.DrawRay(transform.position, Vector3.down * 1.1f);
    if ((!hooking) && Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, ground))
    {
      grounded = true;
      controller.enabled = true;
      rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
      rb.isKinematic = true;
    }
    else
    {
      grounded = false;
      controller.enabled = false;
      rb.isKinematic = false;
      rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    if (hooking)
    {
      if (Vector3.Distance(hand.transform.position, grapplePoint) > hook.linearLimit.limit)
      {
        Vector3 direction = grapplePoint - hand.transform.position;
        Debug.DrawRay(hand.transform.position, direction);
        hook.linearLimitSpring = new SoftJointLimitSpring()
        {
          spring = 20f,
          damper = 20f
        };
      }
      else
      {
        hook.linearLimitSpring = new SoftJointLimitSpring()
        {
          spring = 0f,
          damper = 0f
        };
      }
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
    distanceIndex = 10f;
    grapplePoint = point;
    grappledAt = mainCamera.transform.position;
    distanceAtGrapple = Vector3.Distance(grappledAt, grapplePoint);
    hook = gameObject.AddComponent<ConfigurableJoint>();
    hook.autoConfigureConnectedAnchor = false;
    hook.anchor = hand.transform.localPosition;
    hook.connectedAnchor = grapplePoint;
    hook.xMotion = ConfigurableJointMotion.Limited;
    hook.yMotion = ConfigurableJointMotion.Limited;
    hook.zMotion = ConfigurableJointMotion.Limited;
    float newDistanceIndex = distanceIndex / 10f;
    hook.linearLimit = new SoftJointLimit()
    {
      limit = distanceAtGrapple * newDistanceIndex,
      bounciness = hookBounciness
    };
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
