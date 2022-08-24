namespace Stud;

class Nameplate : WorldPanel
{
	private static Dictionary<Player, WorldPanel> nameplates = new();
	private Player ply;

	private Nameplate( Player ply )
	{
		this.ply = ply;

		var width = 1000f;
		var height = 200f;
		PanelBounds = new Rect( -width / 2f, -height / 2f, width, height );
		StyleSheet = HUD.Instance.StyleSheet;
		AddClass( "nameplate" );

		var container = AddChild<Panel>( "container" );

		var avatar = container.AddChild<Panel>( "avatar" );
		avatar.Style.SetBackgroundImage( $"avatarbig:{ply?.Client?.PlayerId}" );

		var lbl = container.AddChild<Label>( "name" );
		lbl.Text = ply?.Client?.Name ?? "noname";
	}

	[Event.Frame]
	private void onFrame()
	{
		if ( ply == null || !ply.IsValid )
		{
			Delete( true );
			return;
		}

		var transform = ply.Transform;
		transform.Position += Vector3.Up * 84f;
		transform.Rotation = Rotation.LookAt( -CurrentView.Rotation.Forward );

		Transform = transform;
	}

	[Event.Tick.Client]
	private static void onTick()
	{
		var players = Client.All.Select( cl => cl.Pawn as Player );

		foreach( var player in players )
		{
			if ( !nameplates.ContainsKey( player ) )
			{
				var nameplate = new Nameplate( player );
				nameplates.Add( player, nameplate );
			}
		}
	}
}
