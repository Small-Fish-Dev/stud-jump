namespace Stud;

public partial class MouseCapture : Panel
{
	public MouseCapture()
	{
		Style.PointerEvents = PointerEvents.All;
		Style.Cursor = "Default";
	}

	[Event.BuildInput]
	private void buildInput( InputBuilder input )
	{
		Style.PointerEvents = input.Down( InputButton.SecondaryAttack ) 
			? PointerEvents.None
			: PointerEvents.All;

	}
}

