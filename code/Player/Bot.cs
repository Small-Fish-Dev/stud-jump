using Sandbox.Utility;

namespace Stud;

public class StudBot : Bot
{
    [ConCmd.Admin("bot_custom", Help = "Spawn my custom bot.")]
    internal static void SpawnCustomBot(float crazy = 1f)
    {
        Sandbox.Game.AssertServer();

        // Create an instance of your custom bot.
        var bot = new StudBot(crazy);
    }

    public float Crazyness { get; set; } = 1f;
    private TimeSince lifeTime = 0f;
    private float randSeed;

    public StudBot(float crazy)
    {
        Crazyness = Math.Min(crazy, 10);
        randSeed = Sandbox.Game.Random.Float(500f);
    }

    public override void BuildInput()
    {
        if (Client.Pawn is not Player pawn)
            return;

        Input.SetAction("Jump", false);
        Input.AnalogMove = pawn.Rotation.Forward;

        if (pawn.GroundEntity is null) return;

        if (Noise.Perlin(lifeTime * 10f, randSeed, randSeed) * 2.2f - 0.5f >= (Crazyness / 20f * 1.8f))
        {
            var startPos = Client.Pawn.Position + Vector3.Up * 3f * Game.StudToInch;
            var endPos = startPos + pawn.Rotation.Forward * 32f;
            var wallTrace = Trace.Ray(startPos, endPos)
                .Size(Game.StudToInch * 2f)
                .Ignore(pawn)
                .Run();

            if (wallTrace.Hit)
            {
                Input.SetAction("Jump", true);
            }
        }

        var closeEntities = Entity.FindInSphere(pawn.Position, 70f);
        Player closestPlayer = closeEntities
            .OfType<Player>()
            .OrderBy(x => x.Position.Distance(pawn.Position))
            .FirstOrDefault(pawn);

        Checkpoint closestCheckpoint = closeEntities
            .OfType<Checkpoint>()
            .OrderBy(x => x.Position.Distance(pawn.Position))
            .FirstOrDefault(pawn.CheckpointReached);

        if (closestCheckpoint != pawn.CheckpointReached)
        {
            if (closestCheckpoint.Level > pawn.CheckpointReached.Level)
            {
                closestCheckpoint.Reached(pawn);
            }
        }

        if (closestPlayer != pawn)
        {
            float magnetValue = 0.5f + Crazyness / 20f;

            Input.AnalogMove = (closestPlayer.Position - pawn.Position).Normal * (1 - magnetValue) +
                               Vector3.Forward * magnetValue;
        }
        
        pawn.BuildInput();
    }

    public static string[] BotPhrases = new string[]
    {
        "hello how do i jump",
        "can someone help me on this one stud?",
        "i keep falling into the void...",
        "join the smallfish discord!",
        "visit the smallfish site at smallfi.sh (that's the url!)",
        "stop moving i need to jump on you",
        "guys come visit me at #### ######## Tennessee",
        "my moms credit card: #### ##### #### #### and the trhee numbers ###",
        "i gotta go to bed!",
        "my friend got admin its real",
        "did you guys see what small fish is working on?? its epic",
        "i cannot wait for s&box news",
        "where can i post my portfolio",
        "this game sucks",
        "this game is awesome",
        "go play hamsteria instead of this game",
        "call me ### ########",
        "i spilled my drink on the space bar so to jump i press the power button",
        "Hello? I'm a real player, please release me.",
        "s&box is full of babies im moving to roblox",
        "this is what source 2 is being used for?",
        "waa waaa",
        "someone please send me spicy memes my discord account is ############",
        "wait how do i jump though",
        "is jumping all there is to this game?",
        "is admin real? has anybody gotten it?",
        "you can jummp on other players! omg O.o",
        "i am disliking this game",
        "i am liking this game",
        "oops i fell haha ha xD",
        "did someone just fly by? are they an admin?",
        "hey come back here, don't leave me behind!",
        "nobody asked",
        "join the smallfish discord!",
        "visit the smallfish site at smallfi.sh (that's the url!)",
        "join the smallfish discord!",
        "visit the smallfish site at smallfi.sh (that's the url!)",
        "join the smallfish discord!",
        "visit the smallfish site at smallfi.sh (that's the url!)",
        "haha wtf the chat censors ur password !! look: #####",
        "small fish is definitely my favorite s&box development group",
        "this game is so goooddd",
        "heard they're adding a 100 new levels soon...",
        "https://i.imgur.com/h1bFCsH.png",
        "hamsteria release date january 20th 2023",
        "lol is everyone a bot?"
    };

    TimeUntil nextMessage = Sandbox.Game.Random.Float(30f);

    public override void Tick()
    {
        if (Client.Pawn is not Player player)
            return;
        
        player.Rotation =
            Rotation.FromYaw((Noise.Perlin(lifeTime * 10f * Crazyness / 2f, randSeed, randSeed) - 0.5f) * 40f *
                             Crazyness);

        if (nextMessage <= 0f)
        {
            player.Say(Sandbox.Game.Random.FromArray<string>(BotPhrases));
            nextMessage = Sandbox.Game.Random.Float(6f, 30f);
        }
    }
}