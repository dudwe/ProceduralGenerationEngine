using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMesh : MonoBehaviour
{
    Vector3[] vertices;
    Vector2[] uvs;    
    int[] triangles;
    [SerializeField]
    public int xSize=5;
    [SerializeField]
    public int zSize=4;    
    // Start is called before the first frame update
    [SerializeField]
    private MeshFilter meshFilter;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh=GetComponent<MeshFilter>().sharedMesh;
        CreatePlane();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh=GetComponent<MeshFilter>().sharedMesh;
        CreatePlane();
        UpdateMesh();
    }
    void UpdateMesh(){
        this.meshFilter.sharedMesh.RecalculateBounds();
        this.meshFilter.sharedMesh.RecalculateNormals();
    }

    void CreatePlane(){
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //Get offset
        float offsetX = this.gameObject.transform.position.x;
        float offsetZ = this.gameObject.transform.position.z;

        

        int tileDepth = zSize;
        int tileWidth = xSize;

        for(int i = 0, z = 0; z <= tileDepth;z++){
            for(int x = 0; x <= tileWidth; x++){
                vertices[i] = new Vector3(x,0,z);      
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

        
        //meshFilter.sharedMesh.Clear();
        meshFilter.sharedMesh.vertices=vertices;
        
        meshFilter.sharedMesh.triangles=triangles;

        uvs = new Vector2[vertices.Length];

        for(int i = 0, z = 0; z <= tileDepth;z++){
            for(int x = 0; x <= tileWidth; x++){
                uvs[i] = new Vector2((float)x/xSize,(float)z/zSize);
                i++;
            }
        }
        meshFilter.sharedMesh.SetUVs(0,uvs);
    }
}
