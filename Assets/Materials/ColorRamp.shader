Shader "Custom/ColorRamp"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DissolveMap ("Dissolve Map", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.0
        _EdgeWidth ("Edge Width", Range(0,1)) = 0.1
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        // We want alpha blending so we can gradually fade out
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Tell Unity we're writing a Surface Shader that uses the Standard lighting model and supports alpha
        #pragma surface surf Standard alpha

        sampler2D _MainTex;
        sampler2D _DissolveMap;
        float _DissolveAmount;
        float _EdgeWidth;
        fixed4 _EdgeColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DissolveMap;
            // If you want world-space fade from bottom to top, you can also add:
            // float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Sample main texture
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            // Sample dissolve texture
            fixed dissolveSample = tex2D(_DissolveMap, IN.uv_DissolveMap).r;

            // We compare dissolveSample against _DissolveAmount to create a dissolving threshold.
            // Usually, you'd do a smoothstep around the boundary for a softer edge.
            float edgeMin = _DissolveAmount - _EdgeWidth * 0.5;
            float edgeMax = _DissolveAmount + _EdgeWidth * 0.5;

            // Clamped dissolve factor (0 = fully dissolved, 1 = fully present)
            float dissolveFactor = smoothstep(edgeMin, edgeMax, dissolveSample);

            // Base albedo color
            o.Albedo = c.rgb;

            // This sets up an emission around the 'edge' area,
            // so it glows where the object is dissolving.
            // The further away from the center of the threshold, the more emission you get.
            float edgeGlow = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, dissolveSample) -
                             smoothstep(_DissolveAmount - _EdgeWidth, _DissolveAmount, dissolveSample);
            o.Emission = _EdgeColor.rgb * edgeGlow;

            // Alpha is the dissolveFactor. If dissolveFactor is 0, it's invisible.
            o.Alpha = dissolveFactor * c.a;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}
