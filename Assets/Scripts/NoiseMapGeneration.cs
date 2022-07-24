using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoiseMapGeneration : MonoBehaviour{

    //Use Perlin noise to generate a 2d map matrix
    /*
    mapDepth - Z coord
    mapWidth - X coord
    mapScale - Y coord
    */ 
    public float[,] GenerateNoiseMap(int mapDepth,int mapWidth, float mapScale){
        //Create empty 2d noise map
        float[,] noiseMap = new float[mapDepth,mapWidth];

        //Apply Perlin noise to map
        for(int zIdx = 0; zIdx < mapDepth; zIdx++){
            for(int xIdx = 0; xIdx < mapWidth; xIdx++){
                //Use the mapScale to control the level of randomness
                float sampleX = xIdx/mapScale;
                float sampleZ = zIdx/mapScale;

                float noise = Mathf.PerlinNoise(sampleX,sampleZ);
                
                noiseMap[zIdx,xIdx] = noise;
            }
        }
        return noiseMap;
    }
}
