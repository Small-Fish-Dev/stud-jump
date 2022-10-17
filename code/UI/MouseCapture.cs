namespace Stud;

public partial class MouseCapture : Panel
{
	public MouseCapture()
	{
		Style.PointerEvents = PointerEvents.All;
		
	}

	[Event.BuildInput]
	private void buildInput( InputBuilder input )
	{

		foreach( var panel in FindRootPanel().Children )
		{

			panel.Style.PointerEvents = input.Down( InputButton.SecondaryAttack )
				? PointerEvents.None
				: PointerEvents.All;

		}

	}
}

