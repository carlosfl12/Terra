using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float tiltSpeed = 100f;
    public float acceleration = 3f;
    public float maxSpeed = 20f;

    private Vector3 velocity = Vector3.zero;

    public float scrollSpeed = 5f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 20f;

    private void Update()
    {
        // Movimiento de cámara
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Aplicar aceleración
        velocity += moveDirection * acceleration * Time.deltaTime;

        // Limitar velocidad máxima
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Mover la cámara con suavidad
        transform.position += velocity * Time.deltaTime;

        // Detener la cámara gradualmente cuando no se pulsan teclas
        if (moveDirection == Vector3.zero)
        {
            velocity *= Mathf.Pow(0.1f, Time.deltaTime * 10f);
        }

        // Rotación y inclinación de cámara con rueda central del ratón
        if (Input.GetMouseButton(2)) // Botón central del mouse
        {
            float rotationInput = Input.GetAxis("Mouse X");
            float tiltInput = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(rotationInput) > 0.1f)
            {
                transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
            }

            Vector3 newEulerAngles = transform.eulerAngles + new Vector3(-tiltInput * tiltSpeed * Time.deltaTime, 0f, 0f);
            newEulerAngles.x = Mathf.Clamp(newEulerAngles.x, 10f, 80f);
            transform.eulerAngles = newEulerAngles;
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        ZoomCamera(scrollInput);
    }

     void ZoomCamera(float scrollValue)
    {
        Vector3 newPosition = transform.position + transform.forward * scrollValue * scrollSpeed;

        // Limitar la distancia de zoom
        newPosition.y = Mathf.Clamp(newPosition.y, minZoomDistance, maxZoomDistance);

        transform.position = newPosition;
    }
}
