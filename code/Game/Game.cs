global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;

namespace Stud;

partial class Game : GameBase
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

    public override void ClientJoined(Client cl)
    {
        if (bannedUsers.Contains(cl.PlayerId))
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

    public override void ClientDisconnect(Client cl, NetworkDisconnectionReason reason)
    {
        cl.Pawn?.Delete();
        cl.Pawn = null;
    }

    public override bool CanHearPlayerVoice(Client from, Client to)
    {
        return true;
    }

    public override void OnVoicePlayed(Client cl) { }

    public override void Shutdown()
    {
        if (Instance == this)
            Instance = null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Local.Hud?.Delete(true);
        Local.Hud = null;
    }

    public override void PostLevelLoaded()
    {

        if (Host.IsClient) return;

        CreateBots();

    }

    public static async void SubmitScore(string bucket, Client client, int score)
    {

        var leaderboard = await Leaderboard.FindOrCreate(bucket, false);

        await leaderboard.Value.Submit(client, score);

    }

    public static async Task<LeaderboardEntry?> GetScore(string bucket, Client client)
    {

        var leaderboard = await Leaderboard.FindOrCreate(bucket, false);

        return await leaderboard.Value.GetScore(client.PlayerId);

    }

    public void CreateBots()
    {

        var allClothing = ResourceLibrary.GetAll<Clothing>();
        int randAmount = Rand.Int(4, 8);

        for (int i = 0; i < randAmount; i++)
        {

            var bot = new StudBot(Rand.Float(4));
            Player pawn = bot.Client.Pawn as Player;

            var randClothing = Rand.Int(12, 24);

            for (int r = 0; r < randClothing; r++)
            {

                pawn.Clothing.Toggle(allClothing.ElementAt(new Random().Next(allClothing.Count())));

            }

            pawn.Clothing.DressEntity(pawn);

        }

    }

    public override void BuildInput()
    {
        Event.Run("buildinput");
        Local.Pawn?.BuildInput();
    }

    public override CameraSetup BuildCamera(CameraSetup camSetup)
    {
        Local.Pawn?.PostCameraSetup(ref camSetup);

        return camSetup;
    }

    public override void Simulate(Client cl)
    {
        if (!cl.Pawn.IsValid()) return;
        if (!cl.Pawn.IsAuthority) return;

        cl.Pawn.Simulate(cl);
    }

    public override void FrameSimulate(Client cl)
    {
        Host.AssertClient();

        if (!cl.Pawn.IsValid()) return;
        if (!cl.Pawn.IsAuthority) return;

        cl.Pawn?.FrameSimulate(cl);
    }

    public override void RenderHud()
    {
        base.RenderHud();
    }
}
