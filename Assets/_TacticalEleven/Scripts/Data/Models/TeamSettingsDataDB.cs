using SQLite;

[Table("team_settings")]
public class TeamSettingsDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int TicketPrice { get; set; }
    public int SubscriptionPrice { get; set; }
    public int SponsorId { get; set; }
    public int SponsorYearsRemaining { get; set; }
    public int TvChannelId { get; set; }
    public int TvYearsRemaining { get; set; }
}
