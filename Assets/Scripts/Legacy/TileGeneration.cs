using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Terrain types... Water, Grass, Sand, Rock etc etc
[System.Serializable]
public class TerrainType 
{
    public string name;
    public float height;
    public Color color;
}



//Generates the noise map and applies it to the plain
public class TileGeneration : MonoBehaviour
{

    [SerializeField]
    NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;

    [SerializeField]
    private float mapScale;

    [SerializeField]
    private TerrainType[] terrainTypes;

    [SerializeField]
    private float heightMultiplier;
   

    //Use cure to control falttening to produce more realistic planes
    [SerializeField]
    private AnimationCurve heightCurve;

    [SerializeField]
    private Wave[] waves;

    void OnValidate(){
        //Do Something
        GenerateTile();
    }
   

    // Start is called before the first frame update
    void Start(){
        GenerateTile();
    }


    void GenerateTile(){
        //Calculate depth and width of height map based on mesh vertices
        Vector3[] meshVertices = this.meshFilter.sharedMesh.vertices;
        //this.meshFilter.sharedMesh.vertices;//

        int tileDepth= (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth= tileDepth;

        //Get the offset based on the tile positions
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        //calculate the offsets based on the tile position
        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth,tileWidth,this.mapScale,offsetX,offsetZ,waves);


        //Generate the heightmap using noise
        Texture2D tileTexture = BuildTexture(heightMap);
        //this.tileRenderer.sharedMaterial.mainTexture=tileTexture;
        this.tileRenderer.material.mainTexture=tileTexture;
        UpdateMeshVertices(heightMap);
    }


    private  Texture2D BuildTexture(float[,] heightMap){
        //Create a color array 
        //Choose shade from A -> B depending on the height value
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];

        for(int z = 0; z < tileDepth; z++){
            for(int x = 0; x < tileWidth; x++){
               
                //Get the corresponding 1d coloridx position from the current heightMap coordinate
                int colorIndex =  z * tileWidth + x;
                float heightValue = heightMap[z,x];
                //Pick a color between black and white
                //Color.Lerp(Color.black,Color.white,heightValue);
                colorMap[colorIndex] = chooseTerrainType(heightValue);
            }
        }

        //Create a new 2d texture from the colormap and return it 
        Texture2D tileTexture = new Texture2D(tileWidth,tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    private Color chooseTerrainType(float heightValue){
        foreach(TerrainType terrainObject in this.terrainTypes){
            if(heightValue <= terrainObject.height){
                return terrainObject.color;
            }
        }
        return terrainTypes[^1].color;
    }

    private void UpdateMeshVertices(float[,] heightMap){
        //Iterate over the meshVertices, update the heights to the height map
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        int vertexIndex = 0;
        for(int z = 0; z < tileDepth; z++){
            for(int x = 0; x < tileWidth; x++){
                float height = heightMap[z,x];
                Vector3 vertex = meshVertices[vertexIndex];
                meshVertices[vertexIndex] =  new Vector3(vertex.x,height*heightCurve.Evaluate(height) * heightMultiplier, vertex.z);
                vertexIndex++;
            }
        }

        //Apply the mesh vertices to the mesh, update the properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();

        //Update the collider 
        this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;

    }

 
}
