namespace Stud;

partial class Player
{
	public float Distance { get; set; } = 150f;
	private float lastDist;

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		if ( Local.Pawn is not Player pawn ) return;
		if ( pawn.Controller is not PawnController controller ) return;

		lastDist = MathX.LerpTo( lastDist, Distance, 10f * Time.Delta );
		var tr = Trace.Ray( new Ray( pawn.EyePosition, pawn.EyeRotation.Backward ), lastDist )
			.Ignore( pawn )
			.WithoutTags( "player" )
			.Radius( 4f )
			.IncludeClientside()
			.Run();

		setup.Position = tr.EndPosition;
		setup.Rotation = pawn.EyeRotation;
		setup.Viewer = null;
		setup.ZNear = 4f;
		setup.FieldOfView = 70;

		var alpha = MathX.Clamp( CurrentView.Position.Distance( pawn.EyePosition ) / 100f , 0f, 1.1f ) - 0.1f;
		pawn.RenderColor = Color.White.WithAlpha( alpha );
		foreach ( var child in pawn.Children )
			if ( child is ModelEntity ent ) ent.RenderColor = pawn.RenderColor;
	}

	[Event.BuildInput]
	private void cameraInput( InputBuilder input )
	{
		Distance = MathX.Clamp( Distance - input.MouseWheel * 25f * ( 1f + Distance / 100f ), 0f, 1500f );
	}
}
