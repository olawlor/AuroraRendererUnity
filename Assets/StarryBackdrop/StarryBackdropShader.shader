// Fix orientation of starry background.

Shader "LawlorCode/Starry Backdrop Shader" {
// From: https://en.wikibooks.org/wiki/Cg_Programming/Unity/Skyboxes
   Properties {
      _ColorScale ("Color Scaling (RGB)", Color) = (1,1,1,1)
      _EclipticAngle ("Rotation Angle of Ecliptic (degrees)", Range(0,360)) = 0
      _Cube ("Environment Map (RGB)", CUBE) = "" {}
   }
   SubShader {
      Tags { "RenderType"="Background"    "Queue" = "Background" }
      
      Pass {   
         ZWrite Off
         Cull Off
         Fog { Mode Off }

         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"

         // User-specified uniforms
         samplerCUBE _Cube;   
         float4 _ColorScale;
         float _EclipticAngle;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
         };
         
         float3 rotateY(float3 v,float angle_deg) {
            float angle_rad = UNITY_PI/180.0 * angle_deg;
            float s,c; sincos(angle_rad, s, c);
            return float3(
              v.x*c - v.z*s,
              v.y,
              v.x*s + v.z*c
            );
         }
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            output.vertex = UnityObjectToClipPos(input.vertex);
            output.texcoord = rotateY(input.texcoord,_EclipticAngle);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 tc=input.texcoord;
            tc=float3(-tc.x,tc.z,tc.y); // flip to Z up (as baked into cubemap)
            return texCUBE(_Cube, tc) * _ColorScale;
         }
 
         ENDCG
      }
   }
}
