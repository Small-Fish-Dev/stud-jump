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
	public static Leaderboard? StudsLeaderboard { get; set; }
	public static Leaderboard? ExperienceLeaderboard { get; set; }

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

		StudsLeaderboard = Leaderboard.Find( "Studs" ).Result;
		ExperienceLeaderboard = Leaderboard.Find( "Experience" ).Result;

		if ( Host.IsClient ) return;

		CreateBots();

	}

	public void CreateBots()
	{

		var allClothing = ResourceLibrary.GetAll<Clothing>();
		int randAmount = Rand.Int( 4, 8 );

		for ( int i = 0; i < randAmount; i++ )
		{

			var bot = new StudBot( Rand.Float( 4 ) );
			Player pawn = bot.Client.Pawn as Player;

			var randClothing = Rand.Int( 12, 24 );

			for ( int r = 0; r < randClothing; r++ )
			{

				pawn.Clothing.Toggle( allClothing.ElementAt( new Random().Next( allClothing.Count() ) ) );

			}

			pawn.Clothing.DressEntity( pawn );

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
