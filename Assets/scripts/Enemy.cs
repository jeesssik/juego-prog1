using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float Edamage = 5;
    public bool isDead = false;
    public Transform player; // Referencia al transform del jugador
    public float detectionRange = 10f; // Rango de detección del enemigo
    public float maxChaseDistance = 15f; // Distancia máxima de persecución
    public float attackRange = 1f; // Rango de ataque del enemigo
    public LayerMask detectionLayer; // Capas a considerar en la detección
    
    private int attackIndex = 1;
    private float health = 100f;
    private Vector3 originalPosition; // Posición original del enemigo
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isReturning = false;
    
    public ProgressBar healthBar; // Referencia al script de la barra de salud

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Obtener la referencia al NavMeshAgent
        animator = GetComponent<Animator>(); // Obtener la referencia al Animator
        originalPosition = transform.position; // Guardar la posición original del enemigo
        healthBar.gameObject.SetActive(false); // Ocultar la barra de salud al inicio
    }

    private void Update()
    {
        healthBar.BarValue = health;

        if (!isChasing && !isReturning && !isAttacking && !isDead)
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
            healthBar.gameObject.SetActive(false); // Ocultar la barra de salud al regresar
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
                    healthBar.gameObject.SetActive(true); // Mostrar la barra de salud cuando el jugador está en rango
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
        healthBar.gameObject.SetActive(false); // Ocultar la barra de salud al dejar de perseguir
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
        healthBar.gameObject.SetActive(false); // Ocultar la barra de salud al morir
    }

    private void MoveTowardsPlayer()
    {
        if (player != null && navMeshAgent != null)
        {
            //animacion de caminar hacia el jugador
            animator.SetBool("walking", true);
            navMeshAgent.SetDestination(player.position); // Moverse hacia el jugador
        }
    }

    // Rotar el enemigo hacia la posición del jugador
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
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
