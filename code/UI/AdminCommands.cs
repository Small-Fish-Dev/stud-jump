namespace Stud;

class AdminCommands : Panel
{

	Panel container;
	Label title;
	Button flyModeButton;
	Button invisibleButton;
	Button studCollisionButton;

	public AdminCommands()
	{

		container = AddChild<Panel>( "container" );
		title = container.Add.Label( "Admin Commands", "title" );

		flyModeButton = container.Add.Button( "Fly Mode: ON", "button", () => FlyMode() );
		invisibleButton = container.Add.Button( "Invisible: OFF", "button", () => Invisible() );
		container.Add.Button( "Jumpscare All", "button", () => JumpscareAll() );
		studCollisionButton = container.Add.Button( "Stud Collisions: ON", "button", () => StudCollisions() );
		var resetButton = container.Add.Button( "Reset", "button", () => Reset() );
		resetButton.Style.FontSize = 28f;
		resetButton.Style.BackgroundColor = new Color( 0.9f, 0.5f, 0.5f, 1f );

	}

	[Event.BuildInput]
	private void buildInput( InputBuilder input )
	{
		Style.PointerEvents = input.Down( InputButton.SecondaryAttack )
			? PointerEvents.None
			: PointerEvents.All;

	}

	public override void Tick()
	{

		if ( Local.Pawn is not Player player ) return;

		flyModeButton.SetText( $"Fly Mode: {(player.Controller is AdminController ? "ON" : "OFF")}" );
		invisibleButton.SetText( $"Invisible: {(!player.EnableDrawing ? "ON" : "OFF")}" );
		studCollisionButton.SetText( $"Stud Collisions: {(Game.Instance.Map.PhysicsBody.Enabled ? "ON" : "OFF")}" );

	}

	[ConCmd.Server]
	public static void FlyMode()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player player ) return;
		if ( !player.IsAdmin ) return;

		bool active = player.Controller is AdminController;

		player.Controller = active ? new PlayerController() : new AdminController();

	}

	[ConCmd.Server]
	public static void Invisible()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player player ) return;
		if ( !player.IsAdmin ) return;

		bool active = player.EnableDrawing;

		player.EnableDrawing = !active;

	}

	[ConCmd.Server]
	public static void StudCollisions()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player player ) return;
		if ( !player.IsAdmin ) return;

		bool active = Game.Instance.Map.PhysicsBody.Enabled;

		Game.Instance.Map.PhysicsBody.Enabled = !active;

		Game.UpdateStudCollisions( !active );

	}

	[ConCmd.Server]
	public static void Reset()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player player ) return;
		//if ( !player.IsAdmin ) return;

		player.CheckpointReached = Entity.All.OfType<Checkpoint>().Where( x => x.Level == 0 ).First();
		player.Position = player.CheckpointReached.Position;
		player.Experience = 0;

		Game.RemoveAdminMenu( To.Single( player.Client ) );


		bool flyActive = player.Controller is AdminController;
		if ( flyActive ) player.Controller = new PlayerController();

		bool invisActive = !player.EnableDrawing;
		if ( invisActive ) player.EnableDrawing = true;

		bool studActive = Game.Instance.Map.PhysicsBody.Enabled;
		if ( !studActive )
		{
			Game.Instance.Map.PhysicsBody.Enabled = true;
			Game.UpdateStudCollisions( true );
		}

	}

	[ConCmd.Server]
	public static void JumpscareAll()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player player ) return;
		if ( !player.IsAdmin ) return;

		Game.BroadcastJumpscare();

	}

}
