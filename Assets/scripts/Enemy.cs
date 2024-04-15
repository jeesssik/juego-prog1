using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnCollisionEnter(Collision other)  //entro a la colision
    {
        Debug.Log("Colision inicio ");
    }

    private void OnCollisionStay(Collision other)  //colision en curso
    {
        Debug.Log("Colision stay " );
    }
    
    private void OnCollisionExit(Collision other)   //salida de la colision
    {
        Debug.Log("sali de la colision");
    }

  

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
