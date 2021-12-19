namespace NG.BaseCommands;

public abstract class BaseCommand
{
    public abstract string ObjectId { get; set; }
    public abstract string ProcessUid { get; set; }
    public abstract DateTime ProcessDate { get; set; }
    public DateTime ProcessDateUtc => ProcessDate.ToUniversalTime();
    public abstract string LoginUid { get; set; }
    public abstract bool IsCache { get; set; }
    public BaseCommand()
    {
        ObjectId = string.Empty;
        ProcessUid = string.Empty;
        ProcessDate = DateTime.Now;
    }

    public BaseCommand(string processUid) : this()
    {
        ProcessUid = processUid;
    }

    public BaseCommand(string objectId, string processUid) : this(objectId, processUid, DateTime.Now)
    {
        
    }

    public BaseCommand(string objectId, string processUid, DateTime processDate)
    {
        ObjectId = objectId;
        ProcessUid = processUid;
        ProcessDate = processDate;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}