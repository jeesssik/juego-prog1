using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float damage = 1;
    public Transform player; // Referencia al transform del jugador
    public float detectionRange = 10f; // Rango de detección del enemigo
    public float maxChaseDistance = 15f; // Distancia máxima de persecución
    public float attackRange = 2f; // Rango de ataque del enemigo
    public LayerMask detectionLayer; // Capas a considerar en la detección
    private int attackIndex= 1;
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
        if (!isChasing && !isReturning)
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
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            Debug.Log("Jugador dentro del rango de detección del enemigo.");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, detectionLayer))
            {
                if (hit.collider.transform == player)
                {
                    if (distanceToPlayer <= attackRange && !isAttacking)
                    {
                        Attack(); // Llama a la función de ataque si está dentro del rango de ataque
                    }
                    else if (distanceToPlayer > attackRange)
                    {
                        isChasing = true;
                        animator.SetTrigger("isThreatening");
                        StartCoroutine(WaitAndChase(1.5f)); // Esperar duración de la animación de amenaza antes de perseguir
                    }
                }
            }else
            {
                Debug.Log("Jugador fuera del rango de detección del enemigo.");
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
        
       ///////////////////////// Debug.Log("Atacando al jugador");
        if (!isAttacking)
        {
            isAttacking = true;

            if (attackIndex == 1)
            {
                animator.SetTrigger("Attack1Trigger");
                attackIndex = 2;
            }
            else
            {
                animator.SetTrigger("Attack2Trigger");
                attackIndex = 1;
            }

            Invoke("ResetAttack", 1f); // Suponiendo que la duración del ataque es de 1 segundo
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
    
    // Métodos de colisión
    private void OnCollisionEnter(Collision other) //entro a la colision
    {
        //////////////////////Debug.Log("Colisión con el jugador detectada. Iniciando ataque...");
       
        Attack();
    }

    private void OnCollisionStay(Collision other) //colision en curso
    {
        Maria player = other.gameObject.GetComponent<Maria>();
        if (player != null)
        {
           ///////////// Debug.Log("Colisión continua con el jugador detectada. Iniciando ataque...");
            
            Attack();
            // player.TakeDamage(damage * Time.fixedDeltaTime); // fixed delta time es el tiempo que tarda en ejecutarse un frame
        }
    }

    //** Acá tiene que buscar si el jugador está dentro de la distancia aceptable para volver a perseguir
    private void OnCollisionExit(Collision other) //salida de la colision
    {
//        Debug.Log("sali de la colision");
    }
}
