global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Editor;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;

namespace Stud;

partial class Game : GameManager
{
    public static Game Instance { get; private set; }
    static List<long> bannedUsers = new()
    {
        76561198331444223, //nepnep
    };

    public Game()
    {
        Instance = this;
        Event.Run("start");
        GenerateLevel();
    }

    public override void ClientJoined(IClient cl)
    {
        if (bannedUsers.Contains(cl.SteamId))
        {
            cl.Kick();
            return;
        }

        var ply = new Player();
        cl.Pawn = ply;
        ply.Respawn();
        ply.Clothing.LoadFromClient(cl);
        ply.Clothing.DressEntity(ply);
        ply.LoadExperience();
        ply.LoadLevel();
    }

    public override void ClientDisconnect(IClient cl, NetworkDisconnectionReason reason)
    {
        cl.Pawn?.Delete();
        cl.Pawn = null;
    }

    public override bool CanHearPlayerVoice(IClient from, IClient to)
    {
        return true;
    }

    public override void OnVoicePlayed(IClient cl) { }

    public override void Shutdown()
    {
        if (Instance == this)
            Instance = null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        //Sandbox.Game.LocalClient.Hud?.Delete(true);
        //Sandbox.Game.LocalClient.Hud = null;
    }

    public override void PostLevelLoaded()
    {

        if (Sandbox.Game.IsClient) return;

        CreateBots();

    }

    public static void SubmitScore(string bucket, IClient client, int score)
    {

        //var leaderboard = await Leaderboard.FindOrCreate(bucket, false);

        //await leaderboard.Value.Submit(client, score);

        Log.Info("Leaderboard is NO-OP");
        client.SetInt(bucket, score);

    }

    public static int GetScore(string bucket, IClient client)
    {

        //var leaderboard = await Leaderboard.FindOrCreate(bucket, false);

        //return await leaderboard.Value.GetScore(client.SteamId);

        Log.Info("Leaderboard is NO-OP");
        return client.GetInt(bucket, 0);

    }

    public void CreateBots()
    {
        var allClothing = ResourceLibrary.GetAll<Clothing>().ToArray();
        int randAmount = Sandbox.Game.Random.Int(4, 8);

        for (int i = 0; i < randAmount; i++)
        {
            var bot = new StudBot(Sandbox.Game.Random.Float(4));
            Player pawn = bot.Client.Pawn as Player;

            var randClothing = Sandbox.Game.Random.Int(12, 24);

            for (int r = 0; r < randClothing; r++)
            {

                pawn.Clothing.Toggle(Sandbox.Game.Random.FromArray<Clothing>(allClothing));

            }

            pawn.Clothing.DressEntity(pawn);

        }

    }

    public override void Simulate(IClient cl)
    {
        if (!cl.Pawn.IsValid()) return;
        if (!cl.Pawn.IsAuthority) return;
		if ( cl.Pawn is not Player player ) return;

        player.Simulate(cl);
    }

    public override void FrameSimulate(IClient cl)
    {
        Sandbox.Game.AssertClient();

        if (!cl.Pawn.IsValid()) return;
        if (!cl.Pawn.IsAuthority) return;
		if ( cl.Pawn is not Player player ) return;

		player.FrameSimulate(cl);
    }
}