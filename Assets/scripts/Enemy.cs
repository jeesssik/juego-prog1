
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float Edamage = 5;
    public bool isDead = false;
    public Transform player;
    public float detectionRange = 10f; 
    public float maxChaseDistance = 15f; 
    public float attackRange = 1f; 
    public LayerMask detectionLayer;
    private float attackSoudSpeed = 0.5f;
    private float walkSoundSpeed = 0.7f;
    private AudioSource sfx;
    public AudioClip stepSound;
    public AudioClip punchSound;
    
    private int attackIndex = 1;
    private float health = 100f;
    private Vector3 originalPosition; // Posición original del enemigo
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isReturning = false;
    
    public ProgressBar healthBar; // Referencia al script de la barra de salud
    
    private Coroutine stepSoundCoroutine;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Obtener la referencia al NavMeshAgent
        animator = GetComponent<Animator>(); // Obtener la referencia al Animator
        originalPosition = transform.position; // Guardar la posición original del enemigo
        healthBar.gameObject.SetActive(false); // Ocultar la barra de salud al inicio
        sfx = GetComponent<AudioSource>();
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
            StopStepSound();
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
        StopStepSound();
    }
    
    // Función que recibe daño
    public void TakeMariaDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
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
        StopStepSound();
    }

    private void MoveTowardsPlayer()
    {
        if (player != null && navMeshAgent != null)
        {
            //animacion de caminar hacia el jugador
            if (stepSoundCoroutine == null)
            {
                stepSoundCoroutine = StartCoroutine(PlayStepSound());
            }
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
            navMeshAgent.velocity = Vector3.zero;
            
            float attackDuration = GetAttackAnimationDuration();
            float damageTime = attackDuration * 0.5f;
            StartCoroutine(PlaySound(attackSoudSpeed));
            Invoke("MakeDamage", attackDuration / 2f);

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

            Invoke("ResetAttack", 1f); // Reiniciar el ataque después de 1 segundo
        }
    }
    
    private IEnumerator PlaySound(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        sfx.PlayOneShot(punchSound,0.3f);
    }

    private IEnumerator PlayStepSound()
    {
        while (isChasing && !isDead && navMeshAgent.velocity.magnitude > 0.1f)
        {
            sfx.PlayOneShot(stepSound, 0.3f);
            yield return new WaitForSeconds(walkSoundSpeed);
        }
        stepSoundCoroutine = null;
    }
    
    private void StopStepSound()
    {
        if (stepSoundCoroutine != null)
        {
            StopCoroutine(stepSoundCoroutine);
            stepSoundCoroutine = null;
        }
    }
    
    // Obtener la duración de la animación de ataque actual
    private float GetAttackAnimationDuration()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.length;
        }
        return 1f; 
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
        navMeshAgent.isStopped = false; 
    }
}