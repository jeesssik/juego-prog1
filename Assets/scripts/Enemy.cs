/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float Edamage = 20;
    public Transform player;  // Referencia al transform del jugador
    public float detectionRange = 10f; // Rango de detección del enemigo
    public float maxChaseDistance = 15f; // Distancia máxima de persecución
    public float attackRange = 1f; // Rango de ataque del enemigo
    public LayerMask detectionLayer; // Capas a considerar en la detección
    
    private int attackIndex = 1;
    private float health = 80f;
    private Vector3 originalPosition; // Posición original del enemigo
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isReturning = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Obtener la referencia al NavMeshAgent
        animator = GetComponent<Animator>(); // Obtener la referencia al Animator
        originalPosition = transform.position; // Guardar la posición original del enemigo
    }

    private void Update()
    {
        if (!isChasing && !isReturning && !isAttacking)
        {
            DetectPlayer();
        }

        if (isChasing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > maxChaseDistance)
            {
                StopChasing();
            }
            else if (distanceToPlayer <= attackRange)
            {
                isChasing = false;
                Attack(); // Atacar si está dentro del rango de ataque
            }
            else
            {
                RotateTowards(player.position);
                MoveTowardsPlayer();
            }
        }

        // Verifica si el enemigo ha llegado a su posición original
        if (isReturning && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            isReturning = false;
            animator.SetBool("walking", false); // Detener animación de caminar
        }
    }

    private void DetectPlayer()
    {
        if (player.GetComponent<Maria>().isDead) StopChasing();
        
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, detectionLayer))
            {
                if (hit.collider.transform == player)
                {
                    if (distanceToPlayer <= attackRange && !isAttacking)
                    {
                        //Debug.Log("Debería estar atacando al jugador.");
                        isChasing = false;
                        Attack(); // Llama a la función de ataque si está dentro del rango de ataque
                        
                        //si el jugador está en rango de ataque, y ataca al enemigo, el enemigo recibe daño
                        if(Vector3.Distance(transform.position, player.position) <= attackRange)
                        {
                            player.GetComponent<Maria>().TakeDamage(Edamage);
                            //Debug.Log("recibo ataque de MARIA ");
                            
                        }
                    }
                    else if (distanceToPlayer > attackRange)
                    {
                        isChasing = true;
                        animator.SetTrigger("isThreatening");
                        StartCoroutine(WaitAndChase(1.5f)); // Esperar duración de la animación de amenaza antes de perseguir
                    }
                }
            }
        }

        if (isChasing && distanceToPlayer > maxChaseDistance)
        {
            Debug.Log("Dejando de perseguir");
            StopChasing();
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        isReturning = true;
        navMeshAgent.SetDestination(originalPosition);
        animator.SetBool("walking", true); // Iniciar animación de caminar al volver a la posición original
    }

    private void ReturnToOrigin()
    {
        navMeshAgent.SetDestination(originalPosition); // Regresar al origen
        Debug.Log("Regresando al origen");
    }
    
    // recibir daño
    public void TakeMariaDamage(int damage)
    {
        Debug.Log("Enemigo recibe daño");
        health -= damage;
        //PlayDamageSound();  // Reproducir sonido de daño
        Debug.Log("enemiga le queda " + health + " de vida");
        if (health <= 0)
        {
            //isDead = true;
            animator.SetTrigger("death");
            // deja de atacar y hacer daño al jugador
            isChasing = false;
            isAttacking = false;
            
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player != null && navMeshAgent != null)
        {
            navMeshAgent.SetDestination(player.position); // Moverse hacia el jugador
        }
    }

    // Rotar el enemigo hacia la posición del jugador
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Corrutina para esperar un tiempo antes de perseguir
    private IEnumerator WaitAndChase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (isChasing)
        {
            animator.SetBool("walking", true); // Iniciar animación de caminar
            MoveTowardsPlayer();
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            navMeshAgent.isStopped = true; 

            if (attackIndex == 1)
            {
                animator.SetTrigger("Attack1Trigger");
                animator.SetInteger("attackIndex", attackIndex);
                attackIndex = 2;
            }
            else
            {
                animator.SetTrigger("Attack2Trigger");
                animator.SetInteger("attackIndex", attackIndex);
                attackIndex = 1;
            }
            Invoke("MakeDamage", 0.5f);
            Invoke("ResetAttack", 1f); // Suponiendo que la duración del ataque es de 1 segundo
        }
    }
    
    //función que hace daño al jugador
    private void MakeDamage()
    {
        if(Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<Maria>().TakeDamage(Edamage);
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
        navMeshAgent.isStopped = false; // Reanudar el movimiento del enemigo
    }

    // Métodos de colisión
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión con el jugador detectada. Iniciando ataque...");
            Attack();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión continua con el jugador detectada. Iniciando ataque...");
            Attack();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Saliendo de la colisión con el jugador...");
        }
    }
}
*/

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float Edamage = 20;
    public bool isDead = false; 
    public Transform player;  // Referencia al transform del jugador
    public float detectionRange = 10f; // Rango de detección del enemigo
    public float maxChaseDistance = 15f; // Distancia máxima de persecución
    public float attackRange = 1f; // Rango de ataque del enemigo
    public LayerMask detectionLayer; // Capas a considerar en la detección
    
    private int attackIndex = 1;
    private float health = 80f;
    private Vector3 originalPosition; // Posición original del enemigo
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isReturning = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Obtener la referencia al NavMeshAgent
        animator = GetComponent<Animator>(); // Obtener la referencia al Animator
        originalPosition = transform.position; // Guardar la posición original del enemigo
    }

    private void Update()
    {
        if (!isChasing && !isReturning && !isAttacking)
        {
            DetectPlayer();
        }

        if (isChasing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > maxChaseDistance)
            {
                StopChasing();
            }
            else if (distanceToPlayer <= attackRange)
            {
                isChasing = false;
                Attack(); // Atacar si está dentro del rango de ataque
            }
            else
            {
                RotateTowards(player.position);
                MoveTowardsPlayer();
            }
        }

        // Verifica si el enemigo ha llegado a su posición original
        if (isReturning && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            isReturning = false;
            animator.SetBool("walking", false); // Detener animación de caminar
        }
    }

    private void DetectPlayer()
    {
        if (player.GetComponent<Maria>().isDead) StopChasing();
        
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, detectionLayer))
            {
                if (hit.collider.transform == player)
                {
                    if (distanceToPlayer <= attackRange && !isAttacking)
                    {
                        isChasing = false;
                        Attack(); // Llama a la función de ataque si está dentro del rango de ataque
                    }
                    else if (distanceToPlayer > attackRange)
                    {
                        isChasing = true;
                        animator.SetTrigger("isThreatening");
                        StartCoroutine(WaitAndChase(1.5f)); // Esperar duración de la animación de amenaza antes de perseguir
                    }
                }
            }
        }

        if (isChasing && distanceToPlayer > maxChaseDistance)
        {
            StopChasing();
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        isReturning = true;
        navMeshAgent.SetDestination(originalPosition);
        animator.SetBool("walking", true); // Iniciar animación de caminar al volver a la posición original
    }
    
    // Función que recibe daño
    public void TakeMariaDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            Debug.Log("Enemigo recibe " + damage + " de daño. Vida restante: " + health);
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("death");
        isChasing = false;
        isAttacking = false;
        isDead = true;
        navMeshAgent.isStopped = true;
        // Aquí podrías agregar lógica adicional al morir el enemigo
    }

    private void MoveTowardsPlayer()
    {
        if (player != null && navMeshAgent != null)
        {
            navMeshAgent.SetDestination(player.position); // Moverse hacia el jugador
        }
    }

    // Rotar el enemigo hacia la posición del jugador
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Corrutina para esperar un tiempo antes de perseguir
    private IEnumerator WaitAndChase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (isChasing)
        {
            animator.SetBool("walking", true); // Iniciar animación de caminar
            MoveTowardsPlayer();
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            navMeshAgent.isStopped = true;

            if (attackIndex == 1)
            {
                animator.SetTrigger("Attack1Trigger");
                animator.SetInteger("attackIndex", attackIndex);
                attackIndex = 2;
            }
            else
            {
                animator.SetTrigger("Attack2Trigger");
                animator.SetInteger("attackIndex", attackIndex);
                attackIndex = 1;
            }

            Invoke("MakeDamage", 0.5f); // Llamar a MakeDamage después de 0.5 segundos
            Invoke("ResetAttack", 1f); // Reiniciar el ataque después de 1 segundo
        }
    }

    // Función que hace el daño al jugador
    private void MakeDamage()
    {
        if (health > 0 && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<Maria>().TakeDamage(Edamage);
            Debug.Log("Hizo daño a Maria");
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
        navMeshAgent.isStopped = false; // Reanudar el movimiento del enemigo
    }

    // Métodos de colisión (si decides mantenerlos para otro propósito)
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión con el jugador detectada. Iniciando ataque...");
            Attack();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión continua con el jugador detectada. Iniciando ataque...");
            Attack();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Saliendo de la colisión con el jugador...");
        }
    }
}
