using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WandProjectile : MonoBehaviour
{
    [SerializeField] int power;
    [SerializeField] float destroyTime;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * power);
        Destroy(gameObject, destroyTime);
    }
}
