using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//We add waves to our generator to produce a less repetetive result
[System.Serializable]
public class Wave{
    public float seed;
    public float frequency;
    public float amplitude;
}

public class NoiseMapGeneration : MonoBehaviour{

    //Use Perlin noise to generate a 2d map matrix
    /*
    mapDepth - Z coord
    mapWidth - X coord
    mapScale - Y coord
    offsetX - offset X to ensure tile matching
    offsetZ - offset Z to ensure tile matching
    */ 
    public float[,] GenerateNoiseMap(int mapDepth,int mapWidth, float mapScale,float offsetX, float offsetZ,Wave[] waves){
        //Create empty 2d noise map
        float[,] noiseMap = new float[mapDepth,mapWidth];

        //Apply Perlin noise to map
        for(int zIdx = 0; zIdx < mapDepth; zIdx++){
            for(int xIdx = 0; xIdx < mapWidth; xIdx++){
                //Use the mapScale to control the level of randomness
                float sampleX = (xIdx + offsetX) / mapScale;
                float sampleZ = (zIdx + offsetZ) / mapScale;

                /*
                print("OFFSET"+offsetX+":"+offsetZ);
                print( (xIdx)/mapScale +" VS " +sampleX);
                print( (zIdx)/mapScale +" VS " +sampleZ);
                print(Mathf.PerlinNoise(((xIdx)/mapScale),((zIdx)/mapScale)) +"VS"+noise);
                */

                float noise =0f;
                float normalization = 0f;

                foreach(Wave wave in waves){
                    //Generate noise value 
                    noise+=wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed,sampleZ * wave.frequency + wave.seed);
                    normalization+=wave.amplitude;
                }
                
                //Scale  noise value to 0 -> 1
                noise = noise/normalization;
                noiseMap[zIdx,xIdx] = noise;
            }
        }
        return noiseMap;
    }
}
