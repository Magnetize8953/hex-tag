using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawn : MonoBehaviour
{

    [SerializeField] private GameObject[] chunkTypes;
    [SerializeField] private int gridSize;
    private int chunkNumber;
    private Vector3 startPos = Vector3.zero;


    GameObject randomizeChunk(GameObject[] cList, int cNum)
    {
        GameObject selectedChunk;
        int randomizedChunk = Random.Range(0, cNum);
        selectedChunk = cList[randomizedChunk];
        return selectedChunk;
    }

    void createGrid(int gSize)
    {
        float cW = chunkTypes[0].GetComponent<MeshRenderer>().bounds.size.x;
        float cH = chunkTypes[0].GetComponent<MeshRenderer>().bounds.size.z;
        for (int w = 0; w < gSize; w++)
        {
            for(int h = 0; h < gSize; h++)
            {
                
                GameObject newChunk;
                newChunk = randomizeChunk(chunkTypes,chunkNumber);
                float cWidth = newChunk.GetComponent<MeshRenderer>().bounds.size.x;
                float cHeight = newChunk.GetComponent<MeshRenderer>().bounds.size.z;
                int rot = 0 + (90 * Random.Range(0,3));
                Quaternion rotation = Quaternion.Euler(0,rot,0);
                Vector3 spawnPos = startPos + new Vector3(w * cWidth, 0f, h * cHeight);
                Instantiate(newChunk, spawnPos, rotation);
                /*if (w == 0 && h == 0)
                {
                    //Creates Topmost wall.
                    GameObject wallTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //Sets scale to match size of grid.
                    float cubeWidth = wallTop.GetComponent<MeshRenderer>().bounds.size.x;
                    float wallScaler = (cWidth / cubeWidth) * gridSize;
                   
                    Vector3 wallScaling = Vector3.one;
                    wallScaling.x = wallScaler + (cubeWidth/2);
                    wallScaling.y = 10;
                    //Sets position of Topmost wall.
                    Vector3 wallPos = startPos;
                    
                    wallPos.x = wallPos.x +cHeight;
                    wallPos.z = wallPos.z - (cWidth / 2) - (cubeWidth/2);
                    wallTop.transform.position = wallPos;
                    wallTop.transform.localScale = wallScaling;
                }*/
            }
            
        }
        createWalls(cW,cH,gridSize);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chunkNumber = chunkTypes.Length;
        createGrid(gridSize);
        Destroy(this);
    }

    void createWalls(float chunkWidth, float chunkHeight, int gridSize = 1)
    {
         List<GameObject> walls = new List<GameObject>();
        //Creates Wall.
        for (int i = 0; i < 4; i++) {
            walls.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        }
        //Sets scale to match size of grid.
        float cubeWidth = walls[0].GetComponent<MeshRenderer>().bounds.size.x;
        float wallScaler = (chunkWidth / cubeWidth) * gridSize;

        Vector3 wallScaling = Vector3.one;
        wallScaling.x = wallScaler + (cubeWidth / 2);
        wallScaling.y = 10;
        //Sets position of Topmost wall.
        Vector3 wallPos = startPos;

        Quaternion wallSidesRot = Quaternion.Euler(0, 90, 0);
        wallPos.x = wallPos.x + chunkHeight;
        wallPos.z = wallPos.z - (chunkWidth / 2) - (cubeWidth / 2);
        walls[0].transform.position = wallPos;

        //wallPos = startPos;
        wallPos.x = wallPos.x + (chunkWidth * gridSize) / 2 + (cubeWidth/2);
        wallPos.z = wallPos.z + (chunkHeight * gridSize) / 2 + (cubeWidth/2);
        walls[1].transform.rotation = wallSidesRot;
        walls[1].transform.position = wallPos;

        wallPos = startPos;
        wallPos.x = wallPos.x + chunkHeight;
        wallPos.z = wallPos.z + (chunkWidth * gridSize) - (chunkWidth/2) + (cubeWidth/2);
        walls[2].transform.position = wallPos;

        //This is the funky evil wall that keeps fucking me up :( -Mathew
        wallPos = startPos;
        wallPos.x = wallPos.x - (chunkWidth * gridSize) / 2 + (cubeWidth / 2);
        wallPos.z = wallPos.z + (chunkHeight * gridSize) / 2 + (cubeWidth / 2);
        walls[3].transform.rotation = wallSidesRot;
        walls[3].transform.position = wallPos;


        for (int j = 0; j < 4; j++)
        {
            walls[j].transform.localScale = wallScaling;
        }


    }

}
