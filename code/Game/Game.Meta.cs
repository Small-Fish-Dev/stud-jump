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

partial class Game
{
	// Score manipulation stuff!

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



	[ClientRpc]
	public static void UpdateStudCollisions( bool state )
	{

		Game.Instance.Map.PhysicsBody.Enabled = state;

	}

	[ClientRpc]
	public static void BroadcastJumpscare()
	{

		Event.Run( "jumpscare" );

	}

	[ClientRpc]
	public static void AddAdminMenu()
	{

		Event.Run( "addAdminMenu" );

	}

	[ClientRpc]
	public static void RemoveAdminMenu()
	{

		Event.Run( "reset" );

	}


}

// ANTICHEAT STUFF!

public static class DevCommands
{

	[ConCmd.Admin( null )]
	public static void setpos( float x = 0f, float y = 0f, float z = 0f, float pitch = 0f, float yaw = 0f, float roll = 0f )
	{

		Log.Info( "NO CHEATING" );

	}

	[Event.Tick.Client]
	public static void AntiCheat()
	{

		if ( Global.TimeScale > 1.1f ) // Only client sees the difference in timescale?? I really tried
		{

			Log.Warning( "Nice try smart guy, set the time scale back to 1" );
			KickMe();

		}

	}

	[ConCmd.Server]
	private static void KickMe()
	{

		ConsoleSystem.Caller.Kick();

	}

}
