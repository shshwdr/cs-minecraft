﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MechController : MonoBehaviour
{
    public float speed = 10;
    Rigidbody rigidbody;
    [SerializeField] private MouseLook m_MouseLook;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        m_MouseLook.Init(transform, Camera.main.transform);
    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        rigidbody.velocity += (vert * transform.forward + horz * transform.right) * Time.deltaTime * speed;

        animator.SetFloat("Forward", vert);

        //this move should happen after camera rotate in MouseLook, otherwise it might get jitter
        Vector3 idealPos = transform.position + Vector3.up * 6 - Camera.main.transform.forward * 10;
        Camera.main.transform.position = idealPos;
        Vector3 idealForward = Camera.main.transform.forward;
        idealForward.y = 0;
        idealForward.Normalize();
        transform.rotation = Quaternion.LookRotation(idealForward);
    }
    private void FixedUpdate()
    {
        m_MouseLook.UpdateCursorLock();
    }
    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, Camera.main.transform);
    }
}
