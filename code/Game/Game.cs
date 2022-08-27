global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text.Json.Serialization;
global using System.IO;
global using System.Threading.Tasks;
global using System.Text.Json;
global using SandboxEditor;
global using System.ComponentModel.DataAnnotations;

namespace Stud;

partial class Game : GameBase
{
	public static Game Instance { get; private set; }

	public Game()
	{
		Instance = this;
		Event.Run( "start" );
		GenerateLevel();
	}

	public override void ClientJoined( Client cl )
	{
		var ply = new Player();
		cl.Pawn = ply;
		ply.Respawn();
		ply.Clothing.LoadFromClient( cl );
		ply.Clothing.DressEntity( ply );
		ply.LoadExperience();
		ply.LoadLevel();
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		cl.Pawn?.Delete();
		cl.Pawn = null;
	}

	public override bool CanHearPlayerVoice( Client from, Client to )
	{
		return true;
	}

	public override void OnVoicePlayed( Client cl ) { }

	public override void Shutdown()
	{
		if ( Instance == this )
			Instance = null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Local.Hud?.Delete( true );
		Local.Hud = null;
	}

	public override void PostLevelLoaded()
	{

		if ( Host.IsClient ) return;

		for ( int i = 0; i < Rand.Int( 2, 8 ); i++ )
		{

			new StudBot( Rand.Float( 7 ) + 2f );

		}

	}

	public override void BuildInput( InputBuilder input )
	{
		Event.Run( "buildinput", input );
		Local.Pawn?.BuildInput( input );
	}

	public override CameraSetup BuildCamera( CameraSetup camSetup )
	{
		Local.Pawn?.PostCameraSetup( ref camSetup );

		return camSetup;
	}

	public override void Simulate( Client cl )
	{
		if ( !cl.Pawn.IsValid() ) return;
		if ( !cl.Pawn.IsAuthority ) return;

		cl.Pawn.Simulate( cl );
	}

	public override void FrameSimulate( Client cl )
	{
		Host.AssertClient();

		if ( !cl.Pawn.IsValid() ) return;
		if ( !cl.Pawn.IsAuthority ) return;

		cl.Pawn?.FrameSimulate( cl );
	}

	public override void RenderHud()
	{
		base.RenderHud();
	}
}
