using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Maria : MonoBehaviour
{
    private Animator animator; 
    private Rigidbody rb; 
    private int attackIndex = 1; 
    private bool isAttacking = false;
   
    private AudioSource sfx;
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public AudioClip uhhSound;
    public AudioClip ahhSound;
    public AudioClip deathSound;
    
    private float stepInterval = 0.7f;
    private float runStepInterval = 0.38f;
    private float stepTimer;
    
    // Configuración de movimiento
    public float health = 100f; 
    public float rotationSpeed = 80f; 
    public float speed = 2f; 
    private bool isGrounded;
    public float jumpForce = 45f; 
    public float gravityScale = 10f; 
    public float runSpeed = 4f; 
    public bool isDead = false;
    
    void Start()
    {
        Debug.Log("Probando desde branch test");
        animator = GetComponent<Animator>(); // Obtener el Animator Controller del GameObject
        rb = GetComponent<Rigidbody>(); // Obtener el Rigidbody del GameObject
        sfx = GetComponent<AudioSource>();
        
        stepTimer = stepInterval;
    }

    void Update()
    {
        Move();
        Attack();
    }

    // Función de ataque del personaje
    void Attack()
    {
        //Detección del boton izquierdo para atacar
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            Debug.Log("Ataque" +  isAttacking);
            animator.SetInteger("AttackIndex", attackIndex);
            animator.SetTrigger("Attack");
            sfx.PlayOneShot(attackSound);
            isAttacking = false;
            
            // acá tengo que llamar o hacer algo con el daño que causa el ataque
            
            attackIndex++;

            if (attackIndex > 2)
            {
                attackIndex = 1;
            }
            
            Invoke("ApplyDamage", 0.5f);
            Invoke("ResetAttack", 1f);
        }
    }

    // Función de movimiento del personaje
    void Move()
    {
        // Obtener la entrada del teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.RightShift); // Detectar si se está presionando la tecla Shift para correr

        // Ajustar la velocidad según si el personaje está corriendo o caminando
        float currentSpeed = isRunning ? runSpeed : speed;
        // Ajuste de intervalo de pasos según si corre o camina
        float currentStepInterval = isRunning ? runStepInterval : stepInterval; // Ajustar el intervalo de pasos

        // Rotar el personaje sobre su eje
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

        // Calcular el movimiento del personaje hacia adelante y hacia atrás
        Vector3 movement = transform.forward * verticalInput * currentSpeed * Time.deltaTime;

        // Aplicar el movimiento al personaje
        transform.Translate(movement, Space.World);

        // Controlar las animaciones según la dirección del movimiento y si está corriendo
        if (verticalInput > 0f) // Hacia adelante
        {
            PlayFootstep(currentStepInterval, 0.2f);
            animator.SetBool("walk", !isRunning);
            animator.SetBool("Run", isRunning);
            animator.SetBool("backward", false);
        }
        else if (verticalInput < 0f) // Hacia atrás
        {
            PlayFootstep(currentStepInterval, 0.5f);
            animator.SetBool("walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("backward", true);
        }
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("Run", false);
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
        if (horizontalInput != 0)
        {
            transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        // Salto del personaje
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 5f es la fuerza con la que se va a impulsar el personaje al saltar
            sfx.PlayOneShot(jumpSound);
            isGrounded = false;
        }

        // Aplicar movimiento al personaje si cuando salto tengo presionado alguna tecla de movimiento
        if (!isGrounded && (horizontalInput != 0 || verticalInput != 0))
        {
            Debug.Log("Salto con movimiento");
            Vector3 airMovement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
            rb.AddForce(airMovement, ForceMode.Acceleration);
        }

        // Aplicar gravedad al personaje cuando esté en el aire
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        }

        animator.SetBool("isGrounded", isGrounded);
    }
    
    // Función de reproducción de sonido al caminar
    void PlayFootstep(float interval, float volume)
    {
        if (stepTimer <= 0f && isGrounded && !isDead)
        {
            sfx.volume = 0.2f;
            sfx.PlayOneShot(walkSound, volume);
            stepTimer = interval;
        }
        stepTimer -= Time.deltaTime;
    }

    
    // Función para detectar si el jugador está tocando el suelo
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
        if (isDead) return;
        health -= damage;
        PlayDamageSound();  // Reproducir sonido de daño
        Debug.Log("Maria le queda " + health + " de vida");
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    
    // Función para reproducir sonidos de daño
    void PlayDamageSound()
    {
        AudioClip damageSound = Random.value > 0.5f ? uhhSound : ahhSound;
        sfx.PlayOneShot(damageSound);
    }

    // Función de muerte con espera
    IEnumerator Die()
    {
        Debug.Log("Player has died.");
        isDead = true;
        animator.SetTrigger("death");
        sfx.PlayOneShot(deathSound);

        // Esperar a que termine la animación de muerte
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Cargar la escena de pantalla de muerte y manejar la descarga de la escena actual
        SceneManager.LoadScene("DeathScreen", LoadSceneMode.Additive);
        StartCoroutine(UnloadLevel1Scene());
    }

    // Coroutine para descargar la escena actual
    private IEnumerator UnloadLevel1Scene()
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("Level1");
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
}
