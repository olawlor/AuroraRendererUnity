using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

public class OrbitalCamera_Vive : MonoBehaviour
{
  Vector3 P=new Vector3(); // position, in m
  Vector3 V=new Vector3(); // velocity, in m / second
  
  public SteamVR_Input_Sources RocketHand=SteamVR_Input_Sources.RightHand;
  public SteamVR_Action_Pose RocketPose=SteamVR_Input.GetAction<SteamVR_Action_Pose>("default", "Pose");
  public SteamVR_Action_Single RocketThrustAxis=SteamVR_Input.GetAction<SteamVR_Action_Single>("default", "Squeeze");
  
  public SteamVR_Input_Sources TimeHand=SteamVR_Input_Sources.LeftHand;
  public SteamVR_Action_Pose TimePose=SteamVR_Input.GetAction<SteamVR_Action_Pose>("default", "Pose");
  public SteamVR_Action_Single TimeZoomAxis=SteamVR_Input.GetAction<SteamVR_Action_Single>("default", "Squeeze");
  
  public SteamVR_Action_Boolean ResetButton=SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Reset");
  
  float km=1000.0f; // meters per kilometer
  float Re=6378000.0f; // radius of Earth, in meters
  
  // Start is called before the first frame update
  void Start()
  {
    Reset();
  } 
  
  // Back to initial configuration
  private void Reset() 
  {
     P.Set(-0.09f,0.843f,-0.55f); // earth radii
     P=P*Re; // scale up to meters
     V=Vector3.Cross(P,new Vector3(0.0f,0.0f,-1.0f)).normalized*7.88f; // km/sec
     V=V*km; // scale up to meters
     TimeControl.ui_timelapse=1.0f;
  }

  // Update is called once per frame
  void Update()
  {
    // Adapted from: http://3dcognition.com/unity-flight-simulator-phase-2/
    //   and http://wiki.unity3d.com/index.php/SmoothMouseLook
    
    if (ResetButton.GetState(SteamVR_Input_Sources.Any))
      Reset();
    
    float altitude = transform.position.magnitude;
    
    float rotateSpeed = 30.0f; // degrees/second
    float speed = 0.5f * (altitude-0.99f); // WASD movement, earth radius/second
    float mouseSpeed=140.0f; // degrees rotation per pixel of mouse movement / second
  
    float transAmount = speed * Time.deltaTime;
    float rotateAmount = rotateSpeed * Time.deltaTime;
    
    // Update time zooming
    if (Input.GetKeyDown(".")) TimeControl.ui_timelapse*=4.0f;
    if (Input.GetKeyDown(",")) TimeControl.ui_timelapse/=4.0f;
    if (Input.GetKeyDown("/")) TimeControl.ui_timelapse=1.0f;

    // Debug.Log("Orbital camera vive running");
    float zoom=TimeZoomAxis.GetAxis(TimeHand);
    if (zoom>0.0f) { 
      Debug.Log("  time axis active: "+zoom);
      TimeControl.ui_timelapse=16.0f*Mathf.Pow(4.0f,1.0f+2.0f*zoom); 
    } else {
      TimeControl.ui_timelapse=1.0f;
    }
    TimeControl.Update();
    
    
    float rotX = 0.0f;
    float rotY = 0.0f;
    if (Input.GetMouseButton(0)) {
      rotX += Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
      rotY -= Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
    }
    if (Input.GetKey("up")) {
      rotY+=rotateAmount;	
    }
    if (Input.GetKey("down")) {
      rotY-=rotateAmount;	
    }
    if (Input.GetKey("left")) {
      rotX-=rotateAmount;
    }
    if (Input.GetKey("right")) {
      rotX+=rotateAmount;
    }
    
    Vector3 rocket=new Vector3(0.0f,0.0f,0.0f);
    float rocketAccel=50.0f; // m/s^2 acceleration in vacuum
    if (Input.GetKey ("a")) { rocket.x=-rocketAccel; }
    if (Input.GetKey ("d")) { rocket.x=+rocketAccel; }

    if (Input.GetKey ("z")) { rocket.y=-rocketAccel; }
    if (Input.GetKey ("q")) { rocket.y=+rocketAccel; }

    if (Input.GetKey ("s")) { rocket.z=-rocketAccel; }
    if (Input.GetKey ("w")) { rocket.z=+rocketAccel; }
    
    // Rotate keyboard rocket thrust to match local motion frame
    rocket=rocket.x*transform.right + rocket.y*transform.up + rocket.z*transform.forward;
    

    float thrust=RocketThrustAxis.GetAxis(RocketHand);
    Quaternion rot=RocketPose.GetLocalRotation(RocketHand);
    rot = transform.rotation*rot; // local to world rotation
    Vector3 rocketForward = rot * Vector3.forward;
    if (thrust>0.0f) {    
      Debug.Log("  thrust axis active: "+thrust+"  direction "+rocketForward);
      rocket+=thrust*rocketForward*rocketAccel; // FIXME: rotate to match controller orientation
      Engine_manager.g_ThrustLevel=thrust;
    } else {
      Engine_manager.g_ThrustLevel=0.0f;
    }
    
    
    float me=5.972e24f; // mass of Earth, in kilograms
    float G=6.67408e-11f; // gravitational constant, MKS units
    float r=P.magnitude; // distance to spacecraft, in meters
    float accel=-G*me/(r*r); // scalar acceleration due to gravity (m/s^2)
    Vector3 A=P*(accel/P.magnitude); // vector acceleration due to gravity
    A+=rocket; // acceleration due to rocket
    float dt=Time.deltaTime*TimeControl.timelapse;
    V = V + dt*A; // Euler update for velocity
    P = P + dt*V; // Euler update for position
    
    float height=(P.magnitude-Re)/(km); // kilometers altitude
    float airdrag=0.0f;
    if (height<60.0f) {
      float air_density=Mathf.Exp(-height/8.0f);
      float dragfactor=0.01f+0.2f*Vector3.Cross(rocketForward.normalized,V.normalized).magnitude;
      
      airdrag=dragfactor*air_density;
      float dragloss=(1.0f-airdrag*dt);
      if (dragloss<0.5f) dragloss=0.5f;
      V=V*dragloss;
      airdrag*=V.magnitude;
    }
    
    float min_altitude = 1.00001f*Re; // stay outside of the planet
    if (P.magnitude<min_altitude) {
      P=P*(min_altitude/P.magnitude);
    }
    float max_altitude = 100.0f*Re; // stay fairly near the planet
    if (P.magnitude>max_altitude) {
      P=P*(max_altitude/P.magnitude);
      V=V*0.001f;
    }
    
    // Bake output camera position
    Vector3 pos=P*(1.0f/km);// copy out simulated position to GUI position (in Earth radii)
    //pos.y-=1.8f; // make up for user height
    transform.position=pos;

    transform.Rotate(rotY,rotX,0);
    
    // Update the text readout gizmo
    var TimeReadout=GameObject.FindWithTag("TimeReadout");
    if (TimeReadout) {
      TimeReadout.transform.localPosition=TimePose.GetLocalPosition(TimeHand);
      TimeReadout.transform.localRotation=TimePose.GetLocalRotation(TimeHand);
    }
    var TextReadout=GameObject.FindWithTag("TimeReadoutText");
    if (TextReadout) {
      float alt=(P.magnitude-Re)/km;
      float vel=V.magnitude/km;
      string pre="";
      if (airdrag>0.0) {
        pre="Air drag: "+string.Format("{0:F1}",airdrag)+" m/s/s\n";
      }
      string text=pre+
        "Time: x"+string.Format("{0:F1}",TimeControl.timelapse)+"\n"+
        "Thrust: "+string.Format("{0:F0}",rocket.magnitude)+" m/s/s\n"+
        "Speed: "+string.Format("{0:F2}",vel)+" km/s\n"+
        "Vertical: "+string.Format("{0:F2}",Vector3.Dot(P.normalized,V)/km)+" km/s\n"+
        "Altitude: "+(int)alt+" km";
      
      TextReadout.GetComponent<TextMesh>().text=text;
    }
    
    
    if (Input.GetKey("x") || Input.GetKey("escape"))
    {
        Application.Quit();
    }
    
  }
}
