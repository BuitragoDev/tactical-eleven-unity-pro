using SQLite;

[Table("employees")]
public class EmployeeDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Position { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long Salary { get; set; }
    public int Reputation { get; set; }
    public int ContractYears { get; set; }
    public int CandidateDay { get; set; }
}
