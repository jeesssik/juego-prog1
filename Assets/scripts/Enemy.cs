
using System;
using System.Collections;
using System.Collections.Generic;
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

    private bool isThreatening = false;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Obtener la referencia al NavMeshAgent
        animator =GetComponent<Animator>();  // hago referencia al animator para que funcionen las animaciones
    }

    private void Update()
    {
        if (!isChasing)
        {
            DetectPlayer();
        }
        
    }

    private void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, detectionLayer))
            {
                if (hit.collider.transform == player)
                {
                    isChasing = true;
                    isThreatening = true;
                    animator.SetBool("isThreatening", isThreatening);
                    MoveTowardsPlayer(distanceToPlayer);
                    
                }
            }
        }

        if (isChasing && distanceToPlayer > maxChaseDistance)
        {
            
                isChasing = false; // Deja de perseguir si el jugador está fuera del alcance
                navMeshAgent.ResetPath(); // Detener el movimiento del enemigo
                isThreatening = false;
                animator.SetBool("isThreatening", isThreatening);
            
        }

        
    }

    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            navMeshAgent.SetDestination(player.position); // Moverse hacia el jugador
        }
        else
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        // Implementa el comportamiento de ataque
        Debug.Log("Atacando al jugador");
    }

    // Métodos de colisión
    private void OnCollisionEnter(Collision other) //entro a la colision
    {
//        Debug.Log("Colision inicio ");
    }

    private void OnCollisionStay(Collision other) //colision en curso
    {
        //Debug.Log("Colision stay " );
        Maria player = other.gameObject.GetComponent<Maria>();
        if (player != null)
        {
            // player.TakeDamage(damage * Time.fixedDeltaTime); // fixed delta time es el tiempo que tarda en ejecutarse un frame
        }
        else
        {
            // Debug.Log("no estoy tocando al jugador"); // funciona este msj
        }
    }

    private void OnCollisionExit(Collision other) //salida de la colision
    {
        Debug.Log("sali de la colision");
    }
}
