using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawn : MonoBehaviour
{

    [SerializeField] private GameObject[] chunkTypes;
    [SerializeField] private int _gridSize;
    public int GridSize { get => this._gridSize; }
    private int numOfChunkTypes;
    public List<GameObject> Chunks { get; private set; }
    private List<GameObject> walls;
    // one scale unit in unity is equal to 10 on the transform.scale thingy
    private int unityOneBlockLength = 10;


    void Awake()
    {
        this.numOfChunkTypes = this.chunkTypes.Length;
        this.Chunks = new List<GameObject>();
        this.walls = new List<GameObject>();
        createGrid(this._gridSize);
    }

    void createGrid(int gridSize)
    {
        GameObject[] _currentObjects = new GameObject[3];


        // loop through the rows and columns of the grid
        for (int row = 0; row < gridSize; row++)
        {

            for(int column = 0; column < gridSize; column++)
            {
                GameObject newChunk;
                if(column == 0 && row == 0)
                {
                    //Sets first chunk to the basic grass type.
                    newChunk = this.chunkTypes[0];
                }
                else
                {
                    // select a random chunk and grab its width and length for later positioning
                    newChunk = randomizeChunk(this.chunkTypes, this.numOfChunkTypes);
                }
           

                // get a random 90 degree rotation
                int rot = 90 * Random.Range(0,3);
                Quaternion rotation = Quaternion.Euler(0, rot, 0);

                // get the current chunk's spawn position based off which row and column it is
                Vector3 spawnPos = new Vector3(row * this.unityOneBlockLength, 0f, column * this.unityOneBlockLength);

                // instantiate the chunk and add it to the chunks list for later potential use
                this.Chunks.Add(Instantiate(newChunk, spawnPos, rotation, this.transform));
                
            }
        }

        // chunk widths and lengths. used for positioning
        float chunkWidth = this.chunkTypes[0].transform.localScale.x;
        float chunkLength = this.chunkTypes[0].transform.localScale.z;

        // move all chunks such that the centre is at 0, 0, 0
        // this math looks weird, and is probably redundant in many places, but this was the best way to step-by-step visualise it
        //   get 1 chunk width and multiply it by the length of a unity scale (explained poorly above) to get how long one chunk is
        //   divide this by 2 because we want to centre the chunks and therefore need to work by 1/2 intervals
        //   multiply the grid size to push the chunks such that half is before 0, 0 and half is after
        //   make all this negative because of the direction chunk instantiation happened
        //   add back one half because ??????
        float offset = -(((chunkWidth * this.unityOneBlockLength) / 2) * this._gridSize) + ((chunkWidth * this.unityOneBlockLength) / 2);
        this.transform.position = new Vector3(offset, 0, offset);
        //Sets all Player/Enemy objects to offset spawn point.
        _currentObjects = GameObject.FindGameObjectsWithTag("Player");
        for (var i = 0; i < _currentObjects.Length; i++) {
            _currentObjects[i].transform.position = new Vector3(offset, 0.5f, offset);
        }


        // create walls
        createWalls(chunkWidth, chunkLength);

    }

    GameObject randomizeChunk(GameObject[] chunkList, int chunkNumber)
    {
        int randomizedChunk = Random.Range(0, chunkNumber);
        GameObject selectedChunk = chunkList[randomizedChunk];
        return selectedChunk;
    }

    void createWalls(float chunkWidth, float chunkLength)
    {

        // create four walls
        for (int i = 0; i < 4; i++)
        {

            // create individual wall
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // scale wall
            float length = chunkWidth * this._gridSize * this.unityOneBlockLength;
            wall.transform.localScale = new Vector3(length, 2, 0.5f);

            // rotate wall
            int rot = 90 * i;
            Quaternion rotation = Quaternion.Euler(0, rot, 0);
            wall.transform.rotation = rotation;

            // move wall forward
            wall.transform.position += wall.transform.forward * (this._gridSize * ((this.unityOneBlockLength / 2) * chunkWidth));
            // and up
            wall.transform.position = new Vector3(wall.transform.position.x, 1, wall.transform.position.z);

        }

    }

}
