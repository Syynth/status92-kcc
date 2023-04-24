using System;
using UnityEngine;

namespace Status92.KCC
{
    
    
    public class Controller2D : MonoBehaviour
    {
        
        private BoxCollider2D Collider;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
        }
        
        
    }
}