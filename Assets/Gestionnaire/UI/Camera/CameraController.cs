using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float normalSpeed;
    public float fastSpeed;
    public float mouvementSpeed;
    public float mouvementTime;
    public float rotationAmount;
    public float slideAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouvemntInput();
    }

    void HandleMouvemntInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            mouvementSpeed = fastSpeed;
        }
        else
        {
            mouvementSpeed = normalSpeed;
        }
        /*
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * mouvementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -mouvementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * mouvementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -mouvementSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        */
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime* slideAmount);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime* slideAmount);
        //transform.position = newPosition;
        //transform.rotation = newRotation;
    }
}
