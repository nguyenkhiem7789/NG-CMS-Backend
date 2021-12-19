using NG.BaseCommands;
using NG.BaseReadModels;
using NG.Common;
using NG.EventBusModels;
using NG.Extensions;

namespace NG.BaseDomains;

public abstract class BaseDomain
{
    public long NumericalOrder { get; protected set; }
    public string? Id => Code;
    public string? Code { get; protected set; }
    public DateTime CreatedDate { get; protected set; }
    public DateTime CreatedDateUtc { get; protected set; }
    public string? CreatedUid { get; protected set; }
    public DateTime UpdatedDate { get; protected set; }
    public DateTime UpdatedDateUtc { get; protected set; }
    public string? UpdatedUid { get; protected set; }
    public string? LoginUid { get; protected set; }
    public int Version { get; protected set; }
    
    public BaseDomain()
    {
        Code = CommonUtility.GenerateGuid();
        CreatedUid = string.Empty;
        CreatedDate = Extension.GetCurrentDate();
        UpdatedUid = CreatedUid;
        UpdatedDate = CreatedDate;
        UpdatedDateUtc = CreatedDate;
        Version = 0;
    }

    public BaseDomain(BaseReadModel? model)
    {
        if(model == null) return;
        NumericalOrder = model.NumericalOrder;
        Code = model.code;
        CreatedDate = model.CreatedDate;
        CreatedDateUtc = model.CreatedDateUtc;
        CreatedUid = model.CreatedUid;
        UpdatedDate = model.UpdatedDate;
        UpdatedDateUtc = model.UpdatedDateUtc;
        UpdatedUid = model.UpdatedUid;
        Version = model.Version;
        LoginUid = model.LoginUid;
    }

    public BaseDomain(BaseCommand? message)
    {
        if (message == null) return;
        CreatedDate = message.ProcessDate;
        CreatedDateUtc = message.ProcessDateUtc;
        CreatedUid = message.ProcessUid.AsEmpty();
        UpdatedDate = message.ProcessDate;
        UpdatedDateUtc = message.ProcessDateUtc;
        UpdatedUid = message.ProcessUid.AsEmpty();
        LoginUid = message.LoginUid.AsEmpty();
        Version = 0;
    }

    public BaseDomain(BaseDomain? message)
    {
        if(message == null) return;
        CreatedDate = message.CreatedDate;
        CreatedDateUtc = message.CreatedDateUtc;
        CreatedUid = message.CreatedUid.AsEmpty();
        UpdatedDate = message.UpdatedDate;
        UpdatedDateUtc = message.UpdatedDateUtc;
        UpdatedUid = message.UpdatedUid.AsEmpty();
        LoginUid = message.LoginUid.AsEmpty();
        Version = 0;
    }

    public BaseDomain(Event @event)
    {
        CreatedDate = @event.ProcessDate;
        CreatedDateUtc = @event.ProcessDateUtc;
        CreatedUid = @event.ProcessUid.AsEmpty();
        UpdatedDate = @event.ProcessDate;
        UpdatedDateUtc = @event.ProcessDateUtc;
        UpdatedUid = @event.ProcessUid.AsEmpty();
        LoginUid = @event.LoginUid.AsEmpty();
        Version = 0;
    }

    public void Changed(BaseCommand message)
    {
        UpdatedDate = message.ProcessDate;
        UpdatedDateUtc = message.ProcessDateUtc;
        UpdatedUid = message.ProcessUid.AsEmpty();
        LoginUid = message.LoginUid.AsEmpty();
    }

    public void Changed(BaseDomain message)
    {
        UpdatedDate = message.UpdatedDate;
        UpdatedDateUtc = message.UpdatedDateUtc;
        UpdatedUid = message.UpdatedUid.AsEmpty();
        LoginUid = message.LoginUid.AsEmpty();
    }

    public void Changed(Event @event)
    {
        UpdatedDate = @event.ProcessDate;
        UpdatedDateUtc = @event.ProcessDateUtc;
        UpdatedUid = @event.ProcessUid.AsEmpty();
        LoginUid = @event.LoginUid.AsEmpty();
    }
}