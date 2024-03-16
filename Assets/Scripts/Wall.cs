using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Wall : MonoBehaviour
{
    [SerializeField] GameObject normalWallPrefab, socketWallPrefab;
    [SerializeField] XRSocketInteractor wallSocket;
    [SerializeField] GameObject[] wallCubes;
    [SerializeField] float cubeSpacing = 0.005f;
    private Vector3 cubeSize, spawnPosition;

    void Start()
    {
        if(normalWallPrefab != null)
        {
            cubeSize = normalWallPrefab.GetComponent<Renderer>().bounds.size;
        }
        spawnPosition = transform.position;
        BuildWall();
    }

    private void BuildWall()
    {
        wallCubes = new GameObject[2];
        if(normalWallPrefab != null)
        {
            wallCubes[0] = Instantiate(normalWallPrefab, spawnPosition, transform.rotation);
        }
        spawnPosition.y += cubeSize.y + cubeSpacing;
        if (socketWallPrefab != null )
        {
            wallCubes[1] = Instantiate(socketWallPrefab, spawnPosition, transform.rotation);
            wallSocket = wallCubes[1].GetComponentInChildren<XRSocketInteractor>();
            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(OnSocketEnter);
                wallSocket.selectExited.AddListener(OnSocketExit);
            }
        }
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                wallCubes[i].transform.SetParent(transform);
            }
        }
    }

    private void OnSocketExit(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            Rigidbody rigidbody = wallCubes[i].GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }
    }

    private void OnSocketEnter(SelectEnterEventArgs arg0)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            Rigidbody rigidbody = wallCubes[i].GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
