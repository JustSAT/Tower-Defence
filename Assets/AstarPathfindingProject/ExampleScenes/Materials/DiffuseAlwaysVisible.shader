
Shader "Unlit/AlphaSelfIllum" {
    Properties {
        _Color ("Color Tint", Color) = (1,1,1,1)
		_NotVisibleColor ("X-ray color (RGB)", Color) = (0,1,0,1)
        _MainTex ("SelfIllum Color (RGB) Alpha (A)", 2D) = "white"
    }
    Category {
       Lighting On
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
       Tags {Queue=Transparent}
       SubShader {
	   Tags { "RenderType"="Opaque-1" }
		LOD 200
            Material {
               Emission [_Color]
            }
            Pass {
               SetTexture [_MainTex] {
                      Combine Texture * Primary, Texture * Primary
                }
            }

			Pass {
            
        	ZTest Greater
        	
        	Material {
        		Diffuse [_NotVisibleColor]
        	}
        	
        	Color [_NotVisibleColor]
        	
        }
        } 
    }
}

           
		 
