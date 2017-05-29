using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour {

    private float xDir = 1.0f;
    private float zDir = 1.0f;
    private Transform tr;

    public float moveSpeed = 1.0f;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveDir = (Vector3.right * xDir) + (Vector3.forward * zDir);
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        if (tr.localPosition.x > 2)
            xDir = -1.0f;
        else if (tr.localPosition.x < -2)
            xDir = 1.0f;

        if (tr.localPosition.z > 2)
            zDir = -1.0f;
        else if (tr.localPosition.z < -2)
            zDir = 1.0f;
    }
}
