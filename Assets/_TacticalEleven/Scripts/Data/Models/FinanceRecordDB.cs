using SQLite;

[Table("finance_records")]
public class FinanceRecordDB
{
    public const int TYPE_TICKET = 1;
    public const int TYPE_SUBSCRIPTION = 2;
    public const int TYPE_SPONSORSHIP = 3;
    public const int TYPE_TV = 4;
    public const int TYPE_RENOVATION = 5;
    public const int TYPE_DISMISSAL = 6;
    public const int TYPE_SALARIES = 7;
    public const int TYPE_EMPLOYEE_SALARY = 8;
    public const int TYPE_LOAN = 9;

    public const int TYPE_PLAYER_SALE = 10;
    public const int TYPE_PLAYER_LOAN = 11;
    public const int TYPE_FOREIGN_COMP = 12;
    public const int TYPE_PLAYER_HIRING = 13;
    public const int TYPE_HOSPITAL = 14;
    public const int TYPE_FINE = 15;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int SeasonId { get; set; }
    public int RecordType { get; set; }
    public int GameDay { get; set; }
    public long Amount { get; set; }
    public string CreatedAt { get; set; }
}
