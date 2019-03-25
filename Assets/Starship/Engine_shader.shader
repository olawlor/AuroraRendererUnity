Shader "Custom/Engine_shader"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    _Metallic ("Metallic", Range(0,1)) = 0.0
  }
  SubShader
  {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface surf Standard fullforwardshadows

    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0

    sampler2D _MainTex;

    struct Input
    {
      float2 uv_MainTex;
      float3 worldPos;
      float3 worldNormal;
    };

    half _Glossiness;
    half _Metallic;
    fixed4 _Color;

    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    // #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
    UNITY_INSTANCING_BUFFER_END(Props)

    // see https://docs.unity3d.com/Manual/SL-SurfaceShaders.html
    void surf (Input IN, inout SurfaceOutputStandard o)
    {
      // Albedo comes from a texture tinted by color
      fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      // Metallic and smoothness come from slider variables
      o.Metallic = _Metallic;
      o.Smoothness = _Glossiness;
      o.Alpha = c.a;
      
      float4 world=float4(IN.worldPos,1.0f);
      float4 obj=mul(unity_WorldToObject,world);
      obj.z+=15+1.5; // shift up to engine bell edge
      float4 worldN=float4(IN.worldNormal,1.0f);
      float4 objN=mul(unity_WorldToObject,worldN);
    
      // Global thrust level: 0.0 = engines off.  1.0 = all engines on full.
      float g_ThrustLevel=0.2;
      
      // Afterglow is the engine bells glowing after running for a bit.
      float g_AfterGlow_1=1.0;
      float g_AfterGlow_3=1.0;
      float g_AfterGlow_7=0.0;
      
      // g_ThrustLevel cut points where we switch between 1-3-7 engines.
      float cut_0=0.0;
      float cut_1=0.15;
      float cut_3=0.4;
      
      float burn=0.0; // blue-white burning
      float glow=0.0; // orange-red afterglow
      float raptor_r=2.1/2.0; // radius of one raptor (in the model)
      if (abs(obj.x)<raptor_r) { // middle three engines
        if (abs(obj.y)<raptor_r) { // middlemost engine
          glow=g_AfterGlow_1;
          if (g_ThrustLevel>cut_0) {
            burn=1.0;
          }
        } else { // outer two
          glow=g_AfterGlow_3;
          if (g_ThrustLevel>cut_1) {
            burn=1.0;
          }
        }
      }
      else {
        glow=g_AfterGlow_7;
        if (g_ThrustLevel>cut_3) {
          burn=1.0;
        }
      }
      
      glow*=clamp(obj.z/2.0,0.0,1.0); // drop off glow near edge
      float3 emit=float3(0.7*glow, (0.1+clamp(obj.z/2.0-1.0,0.0,0.5))*glow, 0.05*glow);
      if (objN.z<0.0) // inside surface of bell
      {
        float face=clamp(-objN.z,0.0,1.0);
        emit += float3(face*burn,face*burn,burn); //frac(1.0f/2.1f*obj.xyz);
      }
      o.Emission = emit;
    }
    ENDCG
  }
  FallBack "Diffuse"
}
