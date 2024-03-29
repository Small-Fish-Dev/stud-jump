//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "Stud Shader";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
MODES
{
	ToolsVis( S_MODE_TOOLS_VIS );
   	Default();
	VrForward();
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
}

//=========================================================================================================================

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
	#include "common/pixelinput.hlsl"
};

//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"

	//
	// Main
	//
	PixelInput MainVs( INSTANCED_SHADER_PARAMS( VS_INPUT i ) )
	{
		PixelInput o = ProcessVertex( i );
		return FinalizeVertex( o );
	}
}

//=========================================================================================================================

PS
{
    #include "common/pixel.hlsl"
    	BoolAttribute( SupportsMappingDimensions, true );
	CreateInputTexture2D( Tint, Srgb, 8, "", "_color", "Settings", Default3( 1.0, 1.0, 1.0 ) );
	CreateTexture2D( g_tTint ) < Channel( RGB, Box( Tint ), Srgb ); Filter( MIN_MAG_LINEAR_MIP_POINT ); OutputFormat( BC7 ); SrgbRead( true ); UiGroup( "Material" ); >;

	//
	// Main
	//
	float4 MainPs( PixelInput i ) : SV_Target
	{
		Material m = Material::From( i );
		m.Albedo *= Tex2D( g_tTint, float2(0.0f, 0.0f) ).rgb;

		return ShadingModelStandard::Shade( i, m );
	}
}