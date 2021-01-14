using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public Vector3 vector = new Vector3(1, 0, 0);

public class Rotate : MonoBehaviour
{
	[SerializeField]
	GameObject start;

	
	Vector3 axis = new Vector3(0, 0, 0);

	// Update is called once per frame
	void Update()
	{
		Vector3 vector = start.transform.position;
		axis.x = -Mathf.Cos(start.transform.eulerAngles.y * 2 * Mathf.PI / 360 );
		axis.y = 0;
		axis.z = Mathf.Sin(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);

		float horizontalInput = Input.GetAxis("Horizontal");


		if (Input.GetAxis("Horizontal") != null)
        {
            vector.z += 0.4f*horizontalInput * Time.deltaTime;
            
        }
		if (Input.GetKey(KeyCode.Z))
		{
			if (transform.eulerAngles.z < 10 || transform.eulerAngles.z > 350)
			{
				transform.RotateAround(vector, axis, 1f * Time.deltaTime);
				//print("Position" + vector);
				print("Angle" + transform.eulerAngles);
			}
		}

		if (Input.GetKey(KeyCode.C))
		{
			if (transform.eulerAngles.z <= 15 || transform.eulerAngles.z > 355)
			{
				transform.RotateAround(vector, axis, -1f * Time.deltaTime);
				//print("Position" + vector);
				print("Angle" + transform.eulerAngles);
			}
		}
	}
}
