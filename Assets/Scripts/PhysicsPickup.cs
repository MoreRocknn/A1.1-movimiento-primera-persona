using UnityEngine;

public class PhysicsPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private float holdDistance = 3f;
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float minHoldDistance = 1f;
    [SerializeField] private float maxHoldDistance = 6f;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float moveForce = 250f; // How "snappy" the holding feels
    [SerializeField] private float dragAmount = 10f; // Stabilizes the object while held

    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform holdArea; // Empty object where held item goes

    private Rigidbody heldObjRB;
    private GameObject heldObj;

    // Internal state
    private float initialDrag;
    private float initialAngularDrag;
    private bool initialUseGravity;

    void Update()
    {
        // Try to Pickup or Drop with 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                TryPickup();
            }
            else
            {
                DropObject();
            }
        }

        // Throw with Left Click
        if (Input.GetMouseButtonDown(0) && heldObj != null)
        {
            ThrowObject();
        }

        // Mouse wheel
        if (heldObj != null)
        {
            float scroll = Input.mouseScrollDelta.y;

            holdDistance += scroll * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);

            holdArea.localPosition = new Vector3(0, 0, holdDistance);
        }
    }

    void FixedUpdate()
    {
        if (heldObj != null)
        {
            MoveObject();
        }
    }

    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupRange, pickupLayer))
        {
            // Only pickup if it has a Rigidbody
            if (hit.rigidbody != null)
            {
                heldObj = hit.transform.gameObject;
                heldObjRB = hit.rigidbody;

                // Save original RB settings
                initialDrag = heldObjRB.linearDamping; // Note: In Unity 6 this is linearDamping, older versions use drag
                initialAngularDrag = heldObjRB.angularDamping; // older versions use angularDrag
                initialUseGravity = heldObjRB.useGravity;

                // Configure RB for holding
                heldObjRB.useGravity = false;
                heldObjRB.linearDamping = dragAmount; // High drag prevents oscillation
                heldObjRB.angularDamping = dragAmount; // Stop it from spinning wildly

                // Optional: Ignore collision with player to prevent pushing yourself
                // Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), playerCollider, true);
            }
        }
    }

    void MoveObject()
    {
        if (Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f)
        {
            // Calculate direction to hold point
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);

            // Apply velocity based force
            // We multiply by moveForce to make it snappy, but the Drag slows it down preventing overshoot
            heldObjRB.AddForce(moveDirection * moveForce);
        }
    }

    void DropObject()
    {
        // Restore original settings
        heldObjRB.useGravity = initialUseGravity;
        heldObjRB.linearDamping = initialDrag;
        heldObjRB.angularDamping = initialAngularDrag;

        heldObj = null;
        heldObjRB = null;
    }

    void ThrowObject()
    {
        // Cache the RB because DropObject clears it
        Rigidbody throwRB = heldObjRB;

        DropObject();

        // Apply impulse force forward
        throwRB.AddForce(playerCamera.forward * throwForce, ForceMode.Impulse);
    }
}
