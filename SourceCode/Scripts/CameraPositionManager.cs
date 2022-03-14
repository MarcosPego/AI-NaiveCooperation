using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionManager : MonoBehaviour
{
    // Start is called before the first frame update
    public CameraControler camControl;
    public Transform naivePlayer;
    public Transform protectorPlayer;

    void Start()
    {

        Vector3 playersVector = naivePlayer.position - protectorPlayer.position;
        Vector3 cameraToPosistion = protectorPlayer.position + playersVector / 2;
        camControl.target = cameraToPosistion;
        camControl.offset = camControl.target - camControl.transform.position;
        //Debug.Log(cameraToPosistion);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 playersVector = naivePlayer.position - protectorPlayer.position;
        Vector3 cameraToPosistion = protectorPlayer.position + playersVector / 2;
        camControl.target = cameraToPosistion;

        camControl.playersDistance = Vector3.Distance(naivePlayer.position, protectorPlayer.position);
        //Debug.Log(cameraToPosistion);
    }
}
