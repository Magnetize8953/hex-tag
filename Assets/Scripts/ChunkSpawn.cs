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
        for(int w = 0; w < gSize; w++)
        {
            for(int h = 0; h < gSize; h++)
            {
                GameObject newChunk;
                newChunk = randomizeChunk(chunkTypes,chunkNumber);
                float cWidth = newChunk.GetComponent<MeshRenderer>().bounds.size.x;
                float cHeight = newChunk.GetComponent<MeshRenderer>().bounds.size.y;
                float rot = 0f + 90f * Random.Range(0f,3f);
                Quaternion rotation = Quaternion.identity;
                rotation.Set(rot, 0f, 0f,Quaternion.identity.w);
                Vector3 spawnPos = startPos + new Vector3(w * cWidth / 2, h * cHeight / 2, 0);
                Instantiate(newChunk, spawnPos, rotation);
            }
            
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chunkNumber = chunkTypes.Length;
        createGrid(gridSize);
        Destroy(this);
    }

}
