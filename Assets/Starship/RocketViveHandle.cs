using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RocketViveHandle : MonoBehaviour
{
  public SteamVR_Action_Pose Poser=SteamVR_Input.GetAction<SteamVR_Action_Pose>("default", "Pose");
  public SteamVR_Input_Sources source=SteamVR_Input_Sources.RightHand;
  
  // Start is called before the first frame update
  void Start()
  {
      
  }

  // Update is called once per frame
  void Update()
  {
    Vector3 pos=Poser.GetLocalPosition(source);
    Quaternion rot=Poser.GetLocalRotation(source);
    float rotSpeed=0.6f;
    transform.localRotation=Quaternion.Slerp(transform.localRotation,rot,rotSpeed*Time.deltaTime);
    float movSpeed=0.8f;
    transform.localPosition=Vector3.Slerp(transform.localPosition,pos,movSpeed*Time.deltaTime);
  }
}
