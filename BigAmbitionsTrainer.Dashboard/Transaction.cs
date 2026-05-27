namespace BigAmbitionsTrainer.Dashboard;

public class Transaction
{
	public string Description { get; set; }

	public int Day { get; set; }

	public string Type { get; set; }

	public float Amount { get; set; }

	public string Id { get; set; }

	public string Company { get; set; }

	public bool IsIncome => Amount > 0f;

	public bool IsExpense => Amount < 0f;
}
