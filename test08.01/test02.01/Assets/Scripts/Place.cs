using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Place: MonoBehaviour
{
    float speed;
    float maxSpeed;
    float direction = 0;
    bool right = false;
    bool left = false;
   
    void Update()
    {
        Vector3 position = transform.position;

        maxSpeed = 0f;

        if (Input.GetKey(KeyCode.Alpha1))
            maxSpeed = 10f;
        if (Input.GetKey(KeyCode.Alpha2))
            maxSpeed = 40f;
        if (Input.GetKey(KeyCode.Alpha3))
            maxSpeed = 70f;
        if (Input.GetKey(KeyCode.Alpha4))
            maxSpeed = 100f;

        

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            left = true;
            right = false;
           // direction = 1;
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            right = true;
            left = false;
            //direction = -1;
        }
        if (left)
        {
            if(speed < maxSpeed)
            {
                speed += 20* Time.deltaTime;
                print(speed);
            }
            if(speed > maxSpeed)
            {
                speed -= 20 * Time.deltaTime;
                print(speed);
            }
        }
        if(right)
        {
            if (speed > -maxSpeed)
            {
                speed -= 20 * Time.deltaTime;
                print(speed);
            }
            if (speed < -maxSpeed)
            {
                speed += 20 * Time.deltaTime;
                print(speed);
            }
        }
        if (maxSpeed == 0f)
        {
            if ((-1f <= speed && speed <= 1f))
            {
                speed = 0;
            }
        }
        position.z += 0.1f * speed * Time.deltaTime;
        transform.position = position;
      
    }
}
