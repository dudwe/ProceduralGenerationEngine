using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    //Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    Vector2[] uvs;
    // Start is called before the first frame update
    
    [SerializeField]
    public int xSize=5;
    [SerializeField]
    public int zSize=4;

    [SerializeField]
    NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    private float mapScale;

    [SerializeField]
    private float heightMultiplier;
   
    [SerializeField]
    private MeshCollider meshCollider;

    //Use cure to control falttening to produce more realistic planes
    [SerializeField]
    private AnimationCurve heightCurve;



    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private TerrainType[] terrainTypes;
    [SerializeField]
    private MeshRenderer meshRenderer;

    float[,] fallOffMap;

    [SerializeField]
    bool useFalloff;

    [SerializeField]
    float falloff_a =3;
    [SerializeField]
    float falloff_b = 2.2f;
 
  

    void OnValidate(){
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh=GetComponent<MeshFilter>().sharedMesh;
        //GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
        //mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh=mesh;
        if(useFalloff){
            fallOffMap = FallOfGenerator.GenerateFallofMap (xSize+1,falloff_a,falloff_b);
        }
        float[,] heightMap = CreateShape();
        
        ApplyTexture(heightMap);
        UpdateMesh();
      
    }
    void Start()
    {   
        //mesh.Clear();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh=GetComponent<MeshFilter>().sharedMesh;
        if(useFalloff){
            fallOffMap = FallOfGenerator.GenerateFallofMap (xSize+1,falloff_a,falloff_b);
        }
        //GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
        //mesh = new Mesh();
      
        //GetComponent<MeshFilter>().mesh=mesh;

        float[,] heightMap = CreateShape();
        
        ApplyTexture(heightMap);

        UpdateMesh();
    }

    [SerializeField]
    private Wave[] waves;
 

    void ApplyTexture(float[,] heightMap){
        Texture2D tileTexture = BuildTexture(heightMap);
        this.meshRenderer.sharedMaterial.mainTexture=tileTexture;

        //mesh.uv=uvs;
        meshFilter.sharedMesh.SetUVs(0,uvs);
    }

    float[,] CreateShape(){
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //Get offset
        float offsetX = this.gameObject.transform.position.x;
        float offsetZ = this.gameObject.transform.position.z;

        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(zSize+1,xSize+1,this.mapScale,offsetX,offsetZ,waves);
     

        int tileDepth = zSize;
        int tileWidth = xSize;

        for(int i = 0, z = 0; z <= tileDepth;z++){
            for(int x = 0; x <= tileWidth; x++){
                if(useFalloff){
                    heightMap[z,x] = Mathf.Clamp01((heightMap[z,x] - fallOffMap[z,x]));
                }
                float height = heightMap[z,x]; //height*heightCurve.Evaluate(height) * heightMultiplier
                vertices[i] = new Vector3(x,height*heightCurve.Evaluate(height) * heightMultiplier,z);
                
                i++;
            }
        }


        triangles = new int[zSize * xSize * 6];

        int tris = 0;
        int vert = 0;
        for(int z =0; z < zSize; z++){
            int zStart = z * xSize;
            int zEnd = zStart + xSize;
            for(int x =zStart ;x<zEnd;x++){
                triangles[tris]=vert;
                triangles[tris+1]=vert + xSize + 1;
                triangles[tris+2]=vert + 1;
                triangles[tris+3]=vert + 1;
                triangles[tris+4]=vert + xSize + 1;
                triangles[tris+5]=vert + xSize + 2; 

        
                vert ++;
                tris+=6;   
            }
            vert ++;                                                                                                                                                                                    
            
        }

        
        meshFilter.sharedMesh.Clear();
        meshFilter.sharedMesh.vertices=vertices;
        
        meshFilter.sharedMesh.triangles=triangles;

        uvs = new Vector2[vertices.Length];

        for(int i = 0, z = 0; z <= tileDepth;z++){
            for(int x = 0; x <= tileWidth; x++){
                uvs[i] = new Vector2((float)x/xSize,(float)z/zSize);
                i++;
            }
        }


        return heightMap;
    }



    void UpdateMesh(){
        
        this.meshFilter.sharedMesh.RecalculateBounds();
        this.meshFilter.sharedMesh.RecalculateNormals();
        //mesh.RecalculateBounds();
       // mesh.RecalculateNormals();
 
    }

    private void  OnDrawGizmos() {
        if(vertices==null){
            return;
        }
        for (int i = 0; i < vertices.Length; i++){
            Gizmos.DrawSphere(vertices[i],.1f);
        }
    }

    private  Texture2D BuildTexture(float[,] heightMap){
        //Create a color array 
        //Choose shade from A -> B depending on the height value
        int tileDepth = heightMap.GetLength(0) -1 ;
        int tileWidth = heightMap.GetLength(1) -1;

     
        Color[] colorMap = new Color[vertices.Length];

        for(int z = 0; z < tileDepth; z++){
            for(int x = 0; x < tileWidth; x++){
               
                //Get the corresponding 1d coloridx position from the current heightMap coordinate
                int colorIndex =  z * tileWidth + x;
                float heightValue = heightMap[z,x];
        
                //Pick a color between black and white
                colorMap[colorIndex] =chooseTerrainType(heightValue); 
            }
        }

        //Create a new 2d texture from the colormap and return it 
        Texture2D tileTexture = new Texture2D(tileWidth,tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        //mesh.colors=colorMap;
       
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

}

