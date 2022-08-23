namespace Stud;

class HUD : RootPanel
{
	public static HUD Instance { get; set; }

	[Event.Hotload]
	[Event( "start" )]
	private static void createHUD()
	{
		if ( Host.IsServer ) return;

		Instance?.Delete( true );
		Instance = new HUD();
	}

	public HUD()
	{
		Instance = this;
		StyleSheet.Load( "/UI/Style.scss" );
	}
}
