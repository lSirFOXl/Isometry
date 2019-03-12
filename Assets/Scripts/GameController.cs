using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Vector2 gridSize;
    public float gridElementSize;
    public float cubeYOffset = 0.6f;
    public List<ObjectInfo> objectList = new List<ObjectInfo>();

    private float gridOffsetX;
    private float gridOffsetY;
    private GameObject cubeRes;

    public static GameController init;
    public BlockController dragObject;
    public Vector3 selectedGridElementPosition;

    public Transform GridTrans;
    public Transform ObjectsTrans;

    
    // Start is called before the first frame update
    void Start()
    {
        init = this;
        gridOffsetX = gridSize.x * gridElementSize / 2;
        gridOffsetY = gridSize.y * gridElementSize / 2;
        cubeRes = Resources.Load("Cube") as GameObject;
        showGrid();
        showObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            GameObject rayHit = returnRayHit("Cubes");
            if(rayHit != null){
                dragObject = rayHit.GetComponent<BlockController>();
                print(getBlockPosition(dragObject));
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse0)){
            dragObject = null;
        }
        if(dragObject != null && isMouseMove()){
            GameObject rayHit = returnRayHit("Grid");
            if(rayHit != null){
                Vector2 selectedGrifPos = rayHit.GetComponent<GridElementController>().position;
                Vector2 selectedCubePos = getBlockPosition(dragObject);
                if((selectedGrifPos.x != selectedCubePos.x || selectedGrifPos.y != selectedCubePos.y) && (
                    (selectedGrifPos.x-selectedCubePos.x == 1  && selectedGrifPos.y == selectedCubePos.y) ||
                    (selectedGrifPos.x-selectedCubePos.x == -1 && selectedGrifPos.y == selectedCubePos.y) ||
                    (selectedGrifPos.x == selectedCubePos.x && selectedGrifPos.y-selectedCubePos.y == 1) ||
                    (selectedGrifPos.x == selectedCubePos.x && selectedGrifPos.y-selectedCubePos.y == -1)
                )){
                    objectReposition(dragObject, selectedGrifPos-selectedCubePos);
                }
            }
            
        }
    }

    private void objectReposition(BlockController block, Vector2 bias){
        objectList[dragObject.objectId].currentPosition += bias;
        Vector2 pos = objectList[dragObject.objectId].currentPosition;
        List<Vector3> targetPositionList = new List<Vector3>();
        bool canMove = true;
        for (int i = 0; i < objectList[dragObject.objectId].picesObjList.Count; i++)
        {
            Vector2 blockPos = objectList[dragObject.objectId].picesOffset[i];
            Vector2 blockCurrentPosition = getBlockPosition(objectList[dragObject.objectId].picesObjList[i]);

            if(blockCurrentPosition.x >= gridSize.x || blockCurrentPosition.x < 0 || blockCurrentPosition.y >= gridSize.y || blockCurrentPosition.y < 0){
                canMove = false;
                objectList[dragObject.objectId].currentPosition -= bias;
                break;
            }
            

            for (int n = 0; n < objectList.Count; n++)
            {
                if(n != dragObject.objectId){
                    for (int j = 0; j < objectList[n].picesObjList.Count; j++)
                    {
                        if(getBlockPosition(objectList[n].picesObjList[j]) == blockCurrentPosition){
                            canMove = false;
                            objectList[dragObject.objectId].currentPosition -= bias;
                            break;
                        }
                    }
                }
                if(!canMove) break;
                
            }

            if(!canMove) break;

            targetPositionList.Add(new Vector3((pos.x + blockPos.x)* gridElementSize - gridOffsetX, cubeYOffset, (pos.y + blockPos.y)* gridElementSize - gridOffsetX));
            
        }

        if(canMove){
            for (int i = 0; i < targetPositionList.Count; i++)
            {
                objectList[dragObject.objectId].picesObjList[i].targetPosition = targetPositionList[i];
            }
        }   
        

        
    }
    private bool isMouseMove()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    private Vector2 getBlockPosition(BlockController block){
        return new Vector2(objectList[block.objectId].picesOffset[block.pieceId].x+objectList[block.objectId].currentPosition.x, objectList[block.objectId].picesOffset[block.pieceId].y+objectList[block.objectId].currentPosition.y);
    }

    private GameObject returnRayHit(string layer){
        Ray Ray;
        RaycastHit hit;
        int layer_mask = LayerMask.GetMask(layer);
        Ray=Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(Ray, out hit, 9999, layer_mask)){
            return hit.collider.gameObject;
        }
        else return null;
    }

    private void showGrid(){
        
        GameObject gridElementRes = Resources.Load("gridElement") as GameObject;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GameObject gridElementV = Instantiate(gridElementRes);
                gridElementV.transform.localPosition = new Vector3(x * gridElementSize - gridOffsetX, 0, y * gridElementSize - gridOffsetY);
                gridElementV.GetComponent<GridElementController>().position = new Vector2(x, y);
                gridElementV.transform.SetParent(GridTrans);
            }
        }
    }

    private void showObjects(){
        for (int i = 0; i < objectList.Count; i++)
        {
            Vector2 pos = objectList[i].startPosition;
            List<BlockController> pieceList = new List<BlockController>();
            for (int j = 0; j < objectList[i].picesOffset.Count; j++)
            {
                Vector2 blockPos = objectList[i].picesOffset[j];
                GameObject cubeV = Instantiate(cubeRes);
                cubeV.transform.localPosition = new Vector3((pos.x + blockPos.x)* gridElementSize - gridOffsetX, cubeYOffset, (pos.y + blockPos.y)* gridElementSize - gridOffsetX);

                cubeV.transform.SetParent(ObjectsTrans);

                cubeV.GetComponent<Renderer>().material.color = objectList[i].color;

                BlockController bcV = cubeV.GetComponent<BlockController>();
                bcV.objectId = i;
                bcV.pieceId = j;
                bcV.targetPosition = cubeV.transform.localPosition;
                pieceList.Add(bcV);
            }

            objectList[i].Init(pieceList);
        }
    }
}
