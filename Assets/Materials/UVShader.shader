Shader "Custom/UVShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0,0,0,1) // Invisible au départ
        _LightPos ("Light Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _EmissionColor;
            float4 _LightPos;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distanceToLight = distance(i.worldPos, _LightPos.xyz);
                float visibility = saturate(1.0 - distanceToLight / 2.0); // Ajuste la distance
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _EmissionColor.rgb * visibility;
                return col;
            }
            ENDCG
        }
    }
}
