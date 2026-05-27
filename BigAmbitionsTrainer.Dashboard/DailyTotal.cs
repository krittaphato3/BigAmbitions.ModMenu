namespace BigAmbitionsTrainer.Dashboard;

public class DailyTotal
{
	public int Day { get; set; }

	public float Income { get; set; }

	public float Expenses { get; set; }

	public float Net => Income + Expenses;
}
