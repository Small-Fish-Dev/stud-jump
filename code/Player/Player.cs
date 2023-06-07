using System.Text;

namespace Stud;

public partial class Player : AnimatedEntity
{
    public static readonly HashSet<string> BannedWords = new() { "simple", "complicated", "confusing" };

    [Net, Predicted] public PawnController Controller { get; set; }

    public IBaseInventory Inventory { get; protected set; }
    public HashSet<string> OwnedItems { get; set; } = new();

    public Checkpoint CheckpointReached { get; set; }

    public ClothingContainer Clothing = new();
    public bool IsAdmin => CheckpointReached.Level >= 119;
    public TimeSince LastJumpscare;
    public TimeSince LastCollision;
    public TimeSince LastHamster;

    public override void Spawn()
    {
        Inventory ??= new BaseInventory(this);
        Inventory.Add(new CocaColaEspuma());
        Inventory.Add(new JumpCoil());

        Controller ??= new PlayerController();

        SetModel("models/citizenstud/citizenstud.vmdl");

        var controller = Controller as PlayerController;

        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, controller.CollisionBox.Mins, controller.CollisionBox.Maxs);

        UseAnimGraph = false;
        PlaybackRate = 1f;

        Tags.Add("player");

        CheckpointReached = All
            .OfType<Checkpoint>()
            .First(x => x.Level == 0);

        Respawn();
    }

    public void Respawn()
    {
        EnableAllCollisions = true;
        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;

        Position = All
            .OfType<Checkpoint>()
            .First(x => x.Level == 0).Position;

        Rotation = Transform.Zero.Rotation;

        ResetInterpolation();
    }

    [GameEvent.Tick.Client]
    public void AnimateOthers() // The PlayerAnimator doesn't play the jump animation?
    {
        if (Sandbox.Game.LocalPawn == this) return;

        ComputeAnimation();
    }

    TimeSince animationStart = 0f;
    string currentAnimation = "idle";

    public void ComputeAnimation()
    {
        bool midair = GroundEntity == null;
        bool moving = Velocity.Length >= 5f;

        CurrentSequence.Name = midair ? "jump" : moving ? "run" : "idle";

        if (CurrentSequence.Name != currentAnimation)
        {
            currentAnimation = CurrentSequence.Name;
            animationStart = 0f;
        }

        CurrentSequence.Time =
            midair ? Math.Min(animationStart, CurrentSequence.Duration) : animationStart % CurrentSequence.Duration;
    }

    [Net] public Vector3 EyePosition { get; set; }
    public Vector3 EyeLocalPosition = new Vector3(0f, 0f, 64f);
    [Net] public Rotation EyeRotation { get; set; }

    public void UpdateEyePosition()
    {
        if (Controller is not PlayerController controller) return;

        EyePosition = Position
                      + EyeLocalPosition;

        EyeRotation = InputLook.ToRotation();
    }

    public override void Simulate(IClient cl)
    {
        Controller?.Simulate(cl, this);

        if (Sandbox.Game.IsServer)
            UpdateEyePosition();

        if (Position.z <= Game.StudToInch * -4)
        {
            if (CheckpointReached == null)
            {
                Position = All
                    .OfType<Checkpoint>()
                    .First(x => x.Level == 0).Position;
            }
            else
            {
                Velocity = Vector3.Zero;
                Position = CheckpointReached.Position;
                ResetInterpolation();
            }
        }

        if (Controller is PlayerController plyController)
        {
            var rot = Rotation.LookAt(plyController.LastMoveDir.WithZ(0f), Vector3.Up);
            Rotation = Rotation.Lerp(Rotation, rot, Time.Delta * 10f);
        }

        if (Controller is AdminController admController)
        {
            var rot = Rotation.LookAt(admController.LastMoveDir.WithZ(0f), Vector3.Up);
            Rotation = Rotation.Lerp(Rotation, rot, Time.Delta * 10f);
        }


        ComputeAnimation();
    }

    public void LoadExperience()
    {
        var result = Game.GetScore("Experience", Client);

        Experience = result;
        Log.Info($"{Client.Name}'s Experience ({Experience}) was fetched.");
    }

    public void LoadLevel()
    {
        var studs = Game.GetScore("Studs", Client);


        Log.Info($"{Client.Name}'s Studs ({studs}) was fetched.");

        Position = All
            .OfType<Checkpoint>()
            .First(x => x.Level == studs).Position;
    }

    public float Distance { get; set; } = 150f;
    private float lastDist;

    public override void FrameSimulate(IClient cl)
    {
        Controller?.FrameSimulate(cl, this);
        ComputeAnimation();
        UpdateEyePosition();
        Distance = MathX.Clamp(Distance - Input.MouseWheel * 25f * (1f + Distance / 100f), 0f, 1500f);

        lastDist = MathX.LerpTo(lastDist, Distance, 10f * Time.Delta);
        var tr = Trace.Ray(new Ray(EyePosition, EyeRotation.Backward), lastDist)
            .Ignore(this)
            .WithoutTags("player")
            .Radius(4f)
            .IncludeClientside()
            .Run();
        Camera.Rotation = EyeRotation;
        Camera.Position = tr.EndPosition;
        Camera.FieldOfView = Sandbox.Game.Preferences.FieldOfView;
        Camera.ZNear = 4f;
        Camera.ZFar = 5000.0f;

        var alpha = MathX.Clamp(Camera.Position.Distance(EyePosition) / 100f, 0f, 1.1f) - 0.1f;
        RenderColor = Color.White.WithAlpha(alpha);
        foreach (var child in Children)
            if (child is ModelEntity ent)
                ent.RenderColor = RenderColor;
    }


    [ClientInput] public Vector3 InputDirection { get; set; }
    [ClientInput] public Angles InputLook { get; set; }

    public override void BuildInput()
    {
        //if (!Input.UsingMouse || Input.Down("LockMouse"))
        //{
            InputLook += Input.AnalogLook;
        //}

        InputDirection = Input.AnalogMove;
    }

    TimeSince timeSinceLastFootstep = 0;

    public override void OnAnimEventFootstep(Vector3 pos, int foot, float volume)
    {
        if (!Sandbox.Game.IsClient)
            return;

        if (timeSinceLastFootstep < 0.2f)
            return;

        timeSinceLastFootstep = 0;

        var tr = Trace.Ray(pos, pos + Vector3.Down * 20f)
            .Radius(1)
            .Ignore(this)
            .IncludeClientside()
            .Run();

        if (!tr.Hit) return;

        volume *= Velocity.WithZ(0).Length.LerpInverse(0.0f, 200.0f);
        tr.Surface.DoFootstep(this, tr, foot, 1f);
    }

    [ClientRpc]
    public void BuyShit(BaseItem item)
    {
        Log.Info($"TODO: Buy this {item.Name} for {item.Cost}!");
    }

    [ClientRpc]
    public static void sendChat(byte[] data)
    {
        List<(string text, Color col)> result = new();

        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadInt16();

                for (int i = 0; i < count; i++)
                    result.Add((reader.ReadString(), reader.ReadColor()));
            }
        }

        Chat.Instance?.Append(result);
    }

    public static void SendChat(To target, List<(string text, Color col)> message)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write((short)message.Count);

            for (int i = 0; i < message.Count; i++)
            {
                writer.Write(message[i].text);
                writer.Write(message[i].col);
            }
        }

        sendChat(target, stream.ToArray());
    }

    [ConCmd.Server("say")]
    public static void SayCommand(string text)
    {
        if (ConsoleSystem.Caller.Pawn is not Player pawn) return;

        pawn.Say(text);
    }

    public void Say(string text)
    {
        var sb = new StringBuilder(text);
        var i = 0;
        while (i < text.Length)
        {
            var j = i;
            while (j < text.Length && text[j] != ' ')
                j++;

            if (j != i)
            {
                if (Sandbox.Game.Random.Int(12) == 0 || BannedWords.Contains(text.Substring(i, j - i)))
                {
                    // 1984
                    for (var k = i; k < j; k++)
                        sb[k] = '#';
                }

                i = j;
            }
            else
                i++;
        }

        text = $"{sb}";

        var emoji = CurrentRank.Name.Split(" ");

        Player.SendChat(To.Everyone, new List<(string text, Color col)>
        {
            ($"{emoji.FirstOrDefault()} {Client.Name}: ", CurrentRank.Color),
            ($"{text}", Color.White),
        });
    }
}