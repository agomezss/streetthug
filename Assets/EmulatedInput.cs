using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EmulatedInput : MonoBehaviour
{
    public bool Fire1;
    public bool Fire2;

     public void PressFire1()
    {
        Fire1 = true;
        Fire2 = false;
    }

     public void PressFire2()
    {
        Fire2 = true;
        Fire1 = false;
    }
}