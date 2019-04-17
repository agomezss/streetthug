using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame updatepublic Transform target;
     public Transform target;
     void Update()
     {
         if (target)
         {
             var newPosition = new Vector3(target.position.x, 0, -100);
             transform.position = Vector3.Lerp(transform.position, newPosition, 0.98f);
             //transform.position.Set(transform.position.x,transform.position.y, -100);
         }
     }
}
