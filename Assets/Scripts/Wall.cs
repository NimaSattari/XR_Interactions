using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteAlways]
public class Wall : MonoBehaviour
{
    [SerializeField] int columns, rows;
    [SerializeField] GameObject normalWallPrefab, socketWallPrefab;
    [SerializeField] int socketPos;
    private XRSocketInteractor wallSocket;
    public XRSocketInteractor GetWallSocket => wallSocket;
    [SerializeField] ExplosiveDevice explosiveDevice;
    [SerializeField] List<GeneratedColumn> generatedColumns;
    private GameObject[] wallCubes;
    [SerializeField] float cubeSpacing = 0.005f;
    private Vector3 cubeSize, spawnPosition;
    [SerializeField] bool buildWall, deleteWall, destroyWall;
    [SerializeField] int maxPower = 100;
    [SerializeField] AudioClip destroyWallClip;
    public AudioClip GetDestroyClip => destroyWallClip;
    [SerializeField] AudioClip socketClip;
    public AudioClip GetSocketClip => socketClip;


    public UnityEvent OnDestroy;

    private void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnSocketEnter);
            wallSocket.selectExited.AddListener(OnSocketExit);
        }
        if(explosiveDevice != null)
        {
            explosiveDevice.OnDetonate.AddListener(OnDestroyWall);
        }
    }

    private void BuildWall()
    {
        if (normalWallPrefab != null)
        {
            cubeSize = normalWallPrefab.GetComponent<Renderer>().bounds.size;
        }
        spawnPosition = transform.position;
        int socketedColumn = UnityEngine.Random.Range(0, columns);
        for (int i = 0; i < columns; i++)
        {
            if (i == socketedColumn)
            {
                GenerateColumn(i, rows, true);
            }
            else
            {
                GenerateColumn(i, rows, false);
            }
            spawnPosition.x += cubeSize.x + cubeSpacing;
        }
    }

    private void GenerateColumn(int index, int height, bool socketed)
    {
        GeneratedColumn tempColumn = new GeneratedColumn();
        tempColumn.InitializeColumn(transform, index, height, socketed);
        spawnPosition.y = transform.position.y;
        wallCubes = new GameObject[height];
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (normalWallPrefab != null)
            {
                wallCubes[i] = Instantiate(normalWallPrefab, spawnPosition, transform.rotation);
                tempColumn.SetCube(wallCubes[i]);
            }
            spawnPosition.y += cubeSize.y + cubeSpacing;
        }
        if (socketed && socketWallPrefab != null)
        {
            if(socketPos < 0 || socketPos >= height)
            {
                socketPos = 0;
            }
            AddSocketWall(tempColumn);
        }
        generatedColumns.Add(tempColumn);
    }

    private void AddSocketWall(GeneratedColumn socketedColumn)
    {
        if (wallCubes[socketPos] != null)
        {
            Vector3 pos = wallCubes[socketPos].transform.position;
            DestroyImmediate(wallCubes[socketPos]);
            wallCubes[socketPos] = Instantiate(socketWallPrefab, pos, transform.rotation);
            socketedColumn.SetCube(wallCubes[socketPos]);
            if (socketPos == 0)
            {
                wallCubes[socketPos].transform.SetParent(transform);
            }
            else
            {
                wallCubes[socketPos].transform.SetParent(wallCubes[0].transform);
            }
            wallSocket = wallCubes[socketPos].GetComponentInChildren<XRSocketInteractor>();
            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(OnSocketEnter);
                wallSocket.selectExited.AddListener(OnSocketExit);
            }
        }
    }

    private void OnSocketExit(SelectExitEventArgs arg0)
    {
        if (generatedColumns.Count >= 1)
        {
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].ResetColumn();
            }
        }
    }

    private void OnSocketEnter(SelectEnterEventArgs arg0)
    {
        if(generatedColumns.Count >= 1)
        {
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].ActivateColumn();
            }
        }
    }

    private void OnDestroyWall()
    {
        if (generatedColumns.Count >= 1)
        {
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                int power = UnityEngine.Random.Range(maxPower / 2, maxPower);
                generatedColumns[i].DestroyColumn(power);
            }
        }
        OnDestroy?.Invoke();
    }

    private void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            BuildWall();
        }
        if (deleteWall)
        {
            deleteWall = false;
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].DeleteColumn();
            }
            if(generatedColumns.Count >= 1)
            {
                generatedColumns.Clear();
            }
        }
        if (destroyWall)
        {
            destroyWall = false;
            for (int i = 0; i < generatedColumns.Count; i++)
            {
                generatedColumns[i].DestroyColumn(maxPower);
            }
        }
    }
}

[System.Serializable]
public class GeneratedColumn
{
    [SerializeField] GameObject[] wallCubes;
    [SerializeField] bool isSocketed;
    [SerializeField] int columnIndex;

    private bool isParented;
    private Transform parentObject;
    private Transform columnObject;
    private const string Column_Name = "column";
    private const string SocketedColumn_Name = "socketedcolumn";

    public void InitializeColumn(Transform parent, int index, int rows, bool socketed)
    {
        columnIndex = index;
        parentObject = parent;
        wallCubes = new GameObject[rows];
        isSocketed = socketed;
    }

    private void SetColumnName(GameObject cube, int index)
    {
        if (isSocketed)
        {
            cube.name = SocketedColumn_Name;
        }
        else
        {
            cube.name = Column_Name;
        }
        cube.name += index.ToString();
    }

    public void SetCube(GameObject cube)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if(!isParented)
            {
                isParented = true;
                SetColumnName(cube, columnIndex);
                cube.transform.SetParent(parentObject);
                columnObject = cube.transform;
            }
            else
            {
                cube.transform.SetParent(columnObject);
            }
            if (wallCubes[i] == null)
            {
                wallCubes[i] = cube;
                break;
            }
        }
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                UnityEngine.Object.DestroyImmediate(wallCubes[i]);
            }
        }
        wallCubes = new GameObject[0];
    }

    public void DestroyColumn(int power)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rigidbody = wallCubes[i].GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.constraints = RigidbodyConstraints.None;
                wallCubes[i].transform.SetParent(parentObject);
                rigidbody.AddRelativeForce(UnityEngine.Random.onUnitSphere * power);
            }
        }
    }

    public void ActivateColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rigidbody = wallCubes[i].GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
            }
        }
    }

    public void ResetColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rigidbody = wallCubes[i].GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
            }
        }
    }
}
