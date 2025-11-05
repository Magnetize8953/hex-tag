using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawn : MonoBehaviour
{

    [SerializeField] private GameObject[] chunkTypes;
    [SerializeField] private int gridSize;
    private int numOfChunkTypes;
    private List<GameObject> chunks;
    private List<GameObject> walls;
    // one scale unit in unity is equal to 10 on the transform.scale thingy
    private int unityOneBlockLength = 10;

    void Start()
    {
        this.numOfChunkTypes = this.chunkTypes.Length;
        this.chunks = new List<GameObject>();
        this.walls = new List<GameObject>();
        createGrid(this.gridSize);
        Destroy(this);
    }

    void createGrid(int gridSize)
    {

        // loop through the rows and columns of the grid
        for (int row = 0; row < gridSize; row++)
        {

            for(int column = 0; column < gridSize; column++)
            {

                // select a random chunk and grab its width and length for later positioning
                GameObject newChunk = randomizeChunk(this.chunkTypes, this.numOfChunkTypes);
                float newChunkWidth = newChunk.transform.localScale.x;
                float newChunkLength = newChunk.transform.localScale.z;

                // get a random 90 degree rotation
                int rot = 90 * Random.Range(0,3);
                Quaternion rotation = Quaternion.Euler(0, rot, 0);

                // get the current chunk's spawn position based off which row and column it is
                Vector3 spawnPos = new Vector3(row * newChunkWidth, 0f, column * newChunkLength);

                // instantiate the chunk and add it to the chunks list for later potential use
                this.chunks.Add(Instantiate(newChunk, spawnPos, rotation, this.transform));

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
        float offset = -(((chunkWidth * this.unityOneBlockLength) / 2) * this.gridSize) + ((chunkWidth * this.unityOneBlockLength) / 2);
        this.transform.position = new Vector3(offset, 0, offset);

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
            float length = chunkWidth * this.gridSize * this.unityOneBlockLength;
            wall.transform.localScale = new Vector3(length, 2, 0.5f);

            // rotate wall
            int rot = 90 * i;
            Quaternion rotation = Quaternion.Euler(0, rot, 0);
            wall.transform.rotation = rotation;

            // move wall forward
            wall.transform.position += wall.transform.forward * (this.gridSize * ((this.unityOneBlockLength / 2) * chunkWidth));
            // and up
            wall.transform.position = new Vector3(wall.transform.position.x, 1, wall.transform.position.z);

        }

    }

}
