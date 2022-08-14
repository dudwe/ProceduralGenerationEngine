using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //FindLand();
    }

    void Update(){
        //FindLand();
    }


    public void FindLand(){
        //Use Ray cast to attach the object to the land
        Ray ray = new Ray(transform.position,-transform.up);
        RaycastHit hitInfo;
        Vector3 pos = this.gameObject.transform.position ;
        if(Physics.Raycast(ray,out hitInfo)){
            
            //Place the object on the land
            pos =  new Vector3(hitInfo.point.x,hitInfo.point.y,hitInfo.point.z);
        }else{
            //If no terrain below, try above
            ray = new Ray(this.gameObject.transform.position,transform.up);
            if(Physics.Raycast(ray,out hitInfo)){
                pos =   new Vector3(hitInfo.point.x,hitInfo.point.y,hitInfo.point.z);
            }
        }
         Debug.Log("OLD:"+this.gameObject.transform.position+" NEW Position:"+pos);
        this.gameObject.transform.position=pos;
       
    }

}
