using SQLite;

[Table("loans")]
public class LoanDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int Slot { get; set; }
    public long Amount { get; set; }
    public int Months { get; set; }
    public long MonthlyPayment { get; set; }
    public double InterestRate { get; set; }
    public int RemainingMonths { get; set; }
    public int IsActive { get; set; }
}
