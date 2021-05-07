// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
 
Shader "Custom/Edge"  
{  
    Properties  
    {  
        _Edge ("Edge", Range(0, 0.2)) = 0.043  
        _EdgeColor ("EdgeColor", Color) = (1, 1, 1, 1)  
		_FlowColor ("FlowColor", Color) = (1, 1, 1, 1) 
		_FlowSpeed ("FlowSpeed", Range(0, 10)) = 3
		_MainTex ("MainTex", 2D) = "white" {}  
    }  
    SubShader  
    {  
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" } 
 
        Pass  
        {  
			ZWrite Off  
			Blend SrcAlpha OneMinusSrcAlpha 
 
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "UnityCG.cginc"  
  
            fixed _Edge;  
            fixed4 _EdgeColor;  
			fixed4 _FlowColor;
			float _FlowSpeed;
			sampler2D _MainTex;
 
            struct appdata  
            {  
                float4 vertex : POSITION;  
                fixed2 uv : TEXCOORD0;  
            };  
  
            struct v2f  
            {  
                float4 vertex : SV_POSITION;  
                fixed2 uv : TEXCOORD1;  
            };  
  
            v2f vert (appdata v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);   
                o.uv = v.uv;  
                return o;  
            }  
              
            fixed4 frag (v2f i) : SV_Target  
            {     
                fixed x = i.uv.x;  
                fixed y = i.uv.y;  
			
                if((x < _Edge) || (abs(1 - x) < _Edge) || (y < _Edge) || (abs(1 - y) < _Edge))   
                {  
					//点旋转公式：
					//假设对图片上任意点(x,y)，绕一个坐标点(rx0,ry0)逆时针旋转a角度后的新的坐标设为(x0,y0)，有公式：
					//x0 = (x - rx0) * cos(a) - (y - ry0) * sin(a) + rx0 ;
					//y0 = (x - rx0) * sin(a) + (y - ry0) * cos(a) + ry0 ;
 
					float a = _Time.y * _FlowSpeed; 
					float2 rotUV;
 
					x -= 0.5;
					y -= 0.5;
					rotUV.x = x * cos(a) - y * sin(a) + 0.5;
					rotUV.y = x * sin(a) + y * cos(a) + 0.5;
					
					fixed temp = saturate(rotUV.x - 0.5);//-0.5作用是调整流动颜色的比例
                    return _EdgeColor * (1 - temp) + _FlowColor * temp;
                }  
                else   
                {  
                    //fixed4 color = tex2D(_MainTex, i.uv);  
                    return fixed4(1, 1, 1, 0);  
                }   
            }  
            ENDCG  
        }  
    }  
}  
