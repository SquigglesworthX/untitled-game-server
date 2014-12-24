using UnityEngine;
using System.Collections;

public class TopDownCameraFollow : MonoBehaviour
{

    public Transform target;
    public float followSpeed = 3;

	void Start () 
    {	
	}	
	
	void Update () {
        var newPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * followSpeed);
	}
}
