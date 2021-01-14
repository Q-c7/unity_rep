using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;



public class Strella : MonoBehaviour
{// Update is called once per frame

	[SerializeField]
	GameObject bulk, start, temp, partStrela;

	Vector3 axis = new Vector3(0f, 0f, 1f);



	void Update()
	{

		axis.x = -Mathf.Cos(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
		axis.y = 0;
		axis.z = Mathf.Sin(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);

		//Vector3 angles = transform.eulerAngles;
		if (Input.GetKey(KeyCode.Z))
		{
			if (partStrela.transform.eulerAngles.z <= 10 || partStrela.transform.eulerAngles.z >= 350)
			{
				transform.RotateAround(bulk.transform.position, axis, -2f * Time.deltaTime);
				
				Vector3 position = transform.position;
				//print("bulk" + bulk.transform.position);
				//print("begin" + temp.transform.position);
				float length = (bulk.transform.position - temp.transform.position).magnitude;
				float lY = (bulk.transform.position.y - temp.transform.position.y);
				float lX = (bulk.transform.position.x - temp.transform.position.x);
				float lZ = (bulk.transform.position.z - temp.transform.position.z);
				float XZ = Mathf.Sqrt(lX * lX + lZ * lZ);
				
				if (partStrela.transform.eulerAngles.z <= 10 && partStrela.transform.eulerAngles.z >= 1)
				{
					position.y -= 0.014f * XZ * Time.deltaTime;
					position.x -= 0.007f * lY * Time.deltaTime * Mathf.Sin(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
					position.z -= 0.007f * lY * Time.deltaTime * Mathf.Cos(-start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
				}
				else
				{
					position.y -= 0.019f * XZ * Time.deltaTime;
					position.x -= 0.009f * lY * Time.deltaTime * Mathf.Sin(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
					position.z -= 0.009f * lY * Time.deltaTime * Mathf.Cos(-start.transform.eulerAngles.y * 2 * Mathf.PI / 360);

				}
				/*if (partStrela.transform.eulerAngles.z < 350)
				{
					if (partStrela.transform.eulerAngles.z > 1.5)
						position.y -= 0.1f * XZ * Time.deltaTime;
					else
						position.y -= 0.15f * XZ * Time.deltaTime;
				}
				if (partStrela.transform.eulerAngles.z > 350)
					position.y -= 0.1f * XZ * Time.deltaTime;*/
				transform.position = position;
			}


		}

		if (Input.GetKey(KeyCode.C))
		{
			if (partStrela.transform.eulerAngles.z <= 15 || partStrela.transform.eulerAngles.z >= 355)
			{
				transform.RotateAround(bulk.transform.position, axis, 2f * Time.deltaTime);
				Vector3 position = transform.position;
				float length = (bulk.transform.position - temp.transform.position).magnitude;
				float lY = (bulk.transform.position.y - temp.transform.position.y);
				float lX = (bulk.transform.position.x - temp.transform.position.x);
				float lZ = (bulk.transform.position.z - temp.transform.position.z);
				float XZ = Mathf.Sqrt(lX * lX + lZ * lZ);
				if (partStrela.transform.eulerAngles.z <= 10 && partStrela.transform.eulerAngles.z >= 1)
				{
					position.y += 0.014f * XZ * Time.deltaTime;
					position.x += 0.007f * lY * Time.deltaTime * Mathf.Sin(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
					position.z += 0.007f * lY * Time.deltaTime * Mathf.Cos(-start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
				}
				else
				{
					position.y += 0.019f * XZ * Time.deltaTime;
					position.x += 0.009f * lY * Time.deltaTime * Mathf.Sin(start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
					position.z += 0.009f * lY * Time.deltaTime * Mathf.Cos(-start.transform.eulerAngles.y * 2 * Mathf.PI / 360);
				}
					/*if (partStrela.transform.eulerAngles.z < 350)
					{
						if (partStrela.transform.eulerAngles.z > 1.5)
							position.y += 0.03f * XZ * Time.deltaTime;
						else
							position.y += 0.05f * XZ * Time.deltaTime;
					}
					if (partStrela.transform.eulerAngles.z > 350)
						position.y += 0.04f * XZ * Time.deltaTime;*/
					transform.position = position;
			}
		}



		//ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
		//joint.anchor = bulk.transform.position;
	}
}