using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectInfo
{
    public Vector2 startPosition = new Vector2();
    public List<Vector2> picesOffset = new List<Vector2>(); 
    public Color color = new Color(1,1,1,1);

    [HideInInspector]
    public Vector2 currentPosition = new Vector2();

    [HideInInspector]
    public List<BlockController> picesObjList = new List<BlockController>();

    public void Init(List<BlockController> pOL){
        currentPosition = startPosition;
        picesObjList = pOL;
    }
}
