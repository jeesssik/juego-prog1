using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maria : MonoBehaviour
{
    private Animator animator; // Referencia al Animator Controller
    [SerializeField] private Transform camera;

    // Configuración de movimiento
    public float rotationSpeed = 10f; // Velocidad de rotación del personaje
    public float speed = 1f; // Velocidad de movimiento del personaje
    [SerializeField] private float turningSpeed = 2f;

    void Start()
    {
        Debug.Log("Probando desde branch test");
        animator = GetComponent<Animator>(); // Obtener el Animator Controller del GameObject
    }

    void Update()
    {
        // Obtener la entrada del teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Rotar el personaje sobre su eje
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

        // Calcular el movimiento del personaje hacia adelante y hacia atrás
        Vector3 movement = transform.forward * verticalInput * speed * Time.deltaTime;

        // Aplicar el movimiento al personaje
        transform.Translate(movement, Space.World);

        // Controlar las animaciones según la dirección del movimiento
        if (verticalInput > 0f) // Hacia adelante
        {
            animator.SetBool("walk", true);
            animator.SetBool("backward", false);
        }
        else if (verticalInput < 0f) // Hacia atrás
        {
            animator.SetBool("walk", false);
            animator.SetBool("backward", true);
        }
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("backward", false);
        }

        // Controlar las animaciones de giro
        if (horizontalInput < 0f) // Hacia la izquierda
        {
            animator.SetBool("turnL", true);
            animator.SetBool("turnR", false);
        }
        else if (horizontalInput > 0f) // Hacia la derecha
        {
            animator.SetBool("turnL", false);
            animator.SetBool("turnR", true);
        }
        else
        {
            animator.SetBool("turnL", false);
            animator.SetBool("turnR", false);
        }
        
        // Rotar el personaje
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    // Función de daño al jugador
    public void TakeDamage(float damage)
    {
        Debug.Log("Player took " + damage + " damage");
    }
}

         