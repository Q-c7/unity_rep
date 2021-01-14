using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{

    // Start is called before the first frame update

    bool rotationA = false;
    bool rotationD = false;
    float speed = 0f;
	float maxSpeed;
    // Crane displacement part
    void Update()
    {
		maxSpeed = 0;

		if(Input.GetKey(KeyCode.Alpha7))
			maxSpeed = 1f;
		if (Input.GetKey(KeyCode.Alpha8))
			maxSpeed = 4f;
		if (Input.GetKey(KeyCode.Alpha9))
			maxSpeed = 7f;
		if (Input.GetKey(KeyCode.Alpha0))
			maxSpeed = 10f;

		if (Input.GetKey(KeyCode.A))
		{
	
			rotationD = false;
			rotationA = false;
			if (!rotationA)
			{
				if (speed > -maxSpeed)
				{
					speed -= 1f * Time.deltaTime;

				}
				else
				{
					speed += 1f * Time.deltaTime;
				}
				transform.Rotate(0, Time.deltaTime * speed, 0);
			}

			rotationA = true;

		}
		if (!Input.GetKey(KeyCode.A))
		{
			if (rotationA)
			{
				if (speed < 0)
				{
					speed += 1f * Time.deltaTime;
					transform.Rotate(0, 0.005f * speed, 0);

				}
				else
				{
					rotationA = false;
					speed = 0;
				}
			}

		}
		if (Input.GetKey(KeyCode.D))
		{
			
			rotationA = false;
			rotationD = false;
			if (!rotationD)
			{
				if (speed < maxSpeed)
				{
					speed += 1f * Time.deltaTime;

				}
				else
                {
					speed -= 1f * Time.deltaTime;
				}
				transform.Rotate(0, Time.deltaTime * speed, 0);
			}
			rotationD = true;
		
		}
		if (!Input.GetKey(KeyCode.D))
		{
			if (rotationD)
			{
				if (speed > 0)
				{
					speed -= 1f * Time.deltaTime;
					transform.Rotate(0, 0.005f * speed, 0);
				}
				else
				{
					rotationD = false;
					speed = 0;
				}
			}
		}


	}
}
