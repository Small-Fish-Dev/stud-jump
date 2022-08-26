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

		for ( int i = 0; i < Rand.Int( 12 ); i++ )
		{

			new StudBot( Rand.Float( 8 ) + 2f );

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
	/*
	[ConCmd.Server("movescores")]
	public static async void ResetScores( int skip = 0 )
	{

		var leaderBoard = await GameServices.Leaderboard.Query("fish.stud_jump", null, "stud-jump", skip );

		Log.Info( $"!!! Starting {leaderBoard.Entries.Count} entries skipping {skip}" );

		for ( int i = 0; i < leaderBoard.Entries.Count; i++ )
		{

			LeaderboardResult.Entry entry = leaderBoard.Entries[i];

			await GameServices.UpdateLeaderboard(entry.PlayerId, entry.Rating, "Studs");
			Log.Info($"({i}) Set score of {entry.DisplayName} to {entry.Rating}");

		}

		ResetScores( skip + leaderBoard.Entries.Count );

	}*/

		/*[ServerCmd("set_score")]
		public static async void ResetScores( string input )
		{

			long steamid = long.Parse(input);
			var leaderBoard = await GameServices.Leaderboard.Query("fish.blubber_runner");
			var entry = leaderBoard.Entries.Find(e => e.PlayerId == steamid);
			

			await GameServices.SubmitScore(steamid, 999);
			Log.Info($" Reset score of {entry.DisplayName}");

		}*/


}
