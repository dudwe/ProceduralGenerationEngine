using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Generates and assembles multiple tiles together
public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    private int mapWidthTiles;
    [SerializeField]
    private int mapDepthTiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private GameObject waterPrefab;

    //[SerializeField]
    //private int waterOffsetY;

    void Start(){
        GenerateMap();
    }


    void GenerateMap(){
        //Get the tile dimensions form the tile prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int) tileSize.x;
        int tileDepth = (int) tileSize.z;

        //for each tile, generate a Tile in the correct posistion
        for(int xTileIndex = 0;xTileIndex < mapWidthTiles;xTileIndex++){
            for(int zTileIndex=0;zTileIndex < mapDepthTiles; zTileIndex++){
                //Calculate the tile position based on x and z values
                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                this.gameObject.transform.position.y,
                this.gameObject.transform.position.z + zTileIndex * tileDepth);

                //Vector3 waterTilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                //this.gameObject.transform.position.y+waterOffsetY,
                //this.gameObject.transform.position.z + zTileIndex * tileDepth);

                //Instantiate new tile
                GameObject tile = Instantiate(tilePrefab,tilePosition,Quaternion.identity) as GameObject;
                //GameObject watertile = Instantiate(waterPrefab,waterTilePosition,Quaternion.identity) as GameObject;
            }   
        }
    }
}
