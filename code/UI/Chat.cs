namespace Stud;

class Chat : Panel
{
	public static Chat Instance { get; set; }

	Panel container;
	TextEntry textEntry;
	Label ghostText;

	TimeSince lastActive;

	public Chat()
	{
		Instance = this;

		container = AddChild<Panel>( "container" );

		textEntry = AddChild<TextEntry>( "entry" );
		textEntry.AddEventListener( "onsubmit", () => submit() );
		textEntry.AcceptsFocus = true;
		textEntry.AllowEmojiReplace = true;
		textEntry.CaretColor = Color.White;

		ghostText = textEntry.AddChild<Label>();
		ghostText.Style.Opacity = 0.25f;
		ghostText.Text = $"To chat press ENTER or click here.";

		Append( new()
		{
			( "Have fun playing ", Color.White ),
			( "Stud Jump", Color.Red ),
			( " by ", Color.White ),
			( "🐟 Small Fish!", Color.Cyan ),
		} );
	}

	public void Append( List<(string text, Color col)> values )
	{
		var entry = container.AddChild<Panel>( "textContainer" );

		foreach ( var val in values )
		{
			var label = entry.AddChild<Label>();
			label.Text = val.text;
			label.Style.FontColor = val.col;
		}

		lastActive = 0f;
	}

	private void submit()
	{
		var msg = textEntry.Text.Trim();
		textEntry.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Player.SayCommand( msg );
	}

	public override void Tick()
	{
		if ( textEntry.HasFocus ) 
			lastActive = 0f;

		ghostText.Style.Opacity = textEntry.Text == "" && !textEntry.HasFocus ? 0.25f : 0f;

		if ( lastActive > 10f )
			Style.Opacity = 0.6f;
		else
			Style.Opacity = 1f;

		if ( Sandbox.Input.Released( InputButton.Chat ) )
			textEntry.Focus();
	}
}
