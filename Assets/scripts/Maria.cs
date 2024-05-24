using UnityEngine;

public class Maria : MonoBehaviour
{
    private Animator animator; // Referencia al Animator Controller
    private Rigidbody rb;  // Referencia al Rigidbody del personaje
    private int attackIndex = 1; // marca la rotación de las diferentes animaciones de ataque
    private bool isAttacking = false; // marca si el personaje está atacando
    
    // Configuración de movimiento
    public float rotationSpeed = 80f; // Velocidad de rotación del personaje
    public float speed = 2f; // Velocidad de movimiento del personaje
    private bool isGrounded;
    public float jumpForce = 45f;  // Fuerza de salto del personaje
    public float gravityScale = 10f; // Gravedad del personaje
    
    void Start()
    {
        Debug.Log("Probando desde branch test");
        animator = GetComponent<Animator>(); // Obtener el Animator Controller del GameObject
        rb = GetComponent<Rigidbody>(); // Obtener el Rigidbody del GameObject
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
    
        // salto del personaje
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //Debug.Log("Maria Jump");
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // 5f es la fuerza con la que se va a impulsar el personaje al saltar
            isGrounded = false;
        } 
        
        // aplicar moviento al personaje si cuando salto tengo presionado alguna tecla de movimiento
          if (!isGrounded && (horizontalInput != 0 || verticalInput != 0))
          {
              Debug.Log("Salto con movimiento");
              Vector3 airMovement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
              rb.AddForce(airMovement, ForceMode.Acceleration);
          }
                 
         //aplico gravedad al personaje cuando esté en el aire
         if(!isGrounded)
         {
             rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
         }
         
        animator.SetBool("isGrounded", isGrounded);
        
        //Detección del boton izquierdo para atacar
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            Debug.Log("Ataque" + attackIndex);
            animator.SetInteger("AttackIndex", attackIndex);
            animator.SetTrigger("Attack");
            isAttacking = false;
            attackIndex++;
            
            if (attackIndex > 2)
            {
                attackIndex = 1;
            }
        }
        
    }
    
    // metodo para llamar al final de cada ataque
   public void ResetAttack()
    {
        isAttacking = false;
    }
    
    //funcion para detectar si el juegador está tocando el suelo
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
           // Debug.Log("Maria está en el suelo");
            isGrounded = true;
        }
    }

    // Función de daño al jugador
    public void TakeDamage(float damage)
    {
        Debug.Log("Player took " + damage + " damage");
    }
}
