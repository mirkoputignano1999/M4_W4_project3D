using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundDistance = 0.2f;
    [SerializeField] private LayerMask Ground;
    [SerializeField] private Vector3 Drag;
    [SerializeField] private Transform _groundChecker;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
