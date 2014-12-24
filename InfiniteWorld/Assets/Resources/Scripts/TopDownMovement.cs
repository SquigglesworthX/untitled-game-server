using UnityEngine;

public class TopDownMovement : MonoBehaviour
{

    public Animator animator;    
    public int speed = 5;
    public int faceRightOffsetAngle;
   
    void Awake()
    {
    }	
	
	void Update () 
    {
        var movementVector = GetMovementVector();
        MovementAnimation(movementVector);
        FaceDirection(movementVector);
        transform.position += movementVector;
	}

    private Vector3 GetMovementVector()
    {
        Vector3 newDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        newDirection.Normalize();        
        return newDirection * Time.deltaTime * speed;
    }

    private void MovementAnimation(Vector3 movementVector)
    {
        bool isMoving = movementVector != Vector3.zero;
        animator.SetBool("isMoving", isMoving);
    }

    private void FaceDirection(Vector3 movementVector)
    {
        if (movementVector != Vector3.zero)
        {            
            float angle = Mathf.Atan2(movementVector.y, movementVector.x) * Mathf.Rad2Deg + faceRightOffsetAngle;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
