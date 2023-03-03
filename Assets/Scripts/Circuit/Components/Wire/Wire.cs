using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : CircuitComponent
{
    public void AddConnection(int connectionMask)
    {
        var position = 0;
        while (connectionMask != 0)
        {
            var flag = connectionMask & 1;
            if(flag == 1) transform.Find("GFX").Find("Connections").GetChild(position).gameObject.SetActive(true);

            position++;
            connectionMask >>= 1;
        }
    }

    public void RemoveConnection(int connectionMask)
    {
        var position = 0;
        while (connectionMask != 0)
        {
            var flag = connectionMask & 1;
            if(flag == 1) transform.Find("GFX").Find("Connections").GetChild(position).gameObject.SetActive(false);

            position++;
            connectionMask >>= 1;
        }
    }
}
