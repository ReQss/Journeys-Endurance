using UnityEngine;

public class DestroyJoint : MonoBehaviour
{
    private HingeJoint hingeJointToDestroy;

    void Start()
    {
        hingeJointToDestroy = GetComponent<HingeJoint>();
    }

    void Update()
    {
        if (hingeJointToDestroy && hingeJointToDestroy.connectedBody)
        {
            Rigidbody connectedBody = hingeJointToDestroy.connectedBody;    // Access the connected body (Rigidbody)
            if (!connectedBody.TryGetComponent(out HingeJoint _))           // Try to get another HingeJoint component from the connected body
            {
                Destroy(hingeJointToDestroy);
            }
        }
    }
}
