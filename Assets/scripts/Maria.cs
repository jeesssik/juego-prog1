using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maria : MonoBehaviour
{
    public float speed = 5f; // Velocidad de movimiento del personaje
    private Animator animator; // Referencia al Animator Controller
    public float rotationSpeed = 30f; // Velocidad de rotación del personaje

    
    void Start()
    {
        animator = GetComponent<Animator>(); // Obtener el Animator Controller del GameObject
    }
    
    
    void Update()
    {
        // Obtener la entrada del teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calcular el movimiento del personaje
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed * Time.deltaTime;

        // Aplicar el movimiento al personaje
        transform.Translate(movement);

        if (verticalInput > 0f) // Hacia adelante
        {
            animator.SetBool("walk", true);
        }
        else if (verticalInput< 0f) // Hacia atrás
        {
            animator.SetBool("walk", false);
            animator.SetBool("backward", true);
        }
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("backward", false);
            
        }
        
        if(horizontalInput < 0f) // Hacia la izquierda
        {
            animator.SetBool("turnL", true);
            animator.SetBool("turnR", false);
        }
        else if(horizontalInput > 0f) // Hacia la derecha
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
    // funcion daño al jugador
    public void TakeDamage(float damage)
    {
        Debug.Log("Player took " + damage + " damage");
    }
}
