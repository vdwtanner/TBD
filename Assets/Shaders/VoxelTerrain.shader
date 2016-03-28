Shader "Custom/VoxelTerrain" {
	SubShader {
		Pass {
			Cull back

			CGPROGRAM
			#pragma enable_d3d11_debug_symbols
			#pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" 

			struct VertexOut{
				float3 position;
				float3 normal;
				float4 color;
				float2 texCoords;
			};

			uniform StructuredBuffer<VertexOut> vertBuffer;

			struct v2f
			{
				float4  pos : SV_POSITION;
				float3 col : Color;
			};

			v2f vert(uint id : SV_VertexID)
			{
				VertexOut vert = vertBuffer[id];

				v2f OUT;
				OUT.pos = mul(UNITY_MATRIX_MVP, float4(vert.position.xyz, 1));

				OUT.col = dot(float3(0, 1, 0), vert.normal) * 0.5 + 0.5;

				return OUT;
			}

			float4 frag(v2f IN) : COLOR
			{
				return float4(IN.col, 1.0f);
			}


			ENDCG

		}
	}
}
