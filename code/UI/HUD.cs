namespace Stud;

class HUD : RootPanel
{
	public static HUD Instance { get; set; }

	[Event.Hotload]
	[Event("start")]
	private static void createHUD()
	{
		if ( Host.IsServer ) return;

		if ( Instance != null )
			Instance.Delete();

		Instance = new HUD();
	}

	public HUD()
	{
		SetTemplate( "/UI/Layout.html" );
	}

	[Event("jumpscare")]
	private void AddJumpscare()
	{

		AddChild( new Jumpscare() );

	}

	private Panel adminMenu;

	[Event( "addAdminMenu" )]
	private void AddAdminMenu()
	{

		adminMenu = AddChild<AdminCommands>();

	}

	[Event( "reset" )]
	private void AdminReset()
	{

		adminMenu.Delete();

	}

}
