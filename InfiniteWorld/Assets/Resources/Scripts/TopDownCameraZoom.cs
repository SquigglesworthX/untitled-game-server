using UnityEngine;
using System.Collections;

public class TopDownCameraZoom : MonoBehaviour {

    public float zoomSpeed = 8f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

	void Start () 
    {	
	}
	
	
	void Update () 
    {
        var wheelAmount = Input.GetAxisRaw("Mouse ScrollWheel");
        var newZoom = camera.orthographicSize + wheelAmount * zoomSpeed;
        camera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
	}
}
