using Microsoft.AspNetCore.Http;

namespace Tahyour.Base.Common.Domain.Entities;

public class AuditLog : BaseEntity<Guid>
{
    public AuditLog(IHttpContextAccessor httpContextAccessor, string actionType, string entityName, string oldValues = null, string newValues = null, string additionalInfo = null)
    {
        Id = Guid.NewGuid();
        Code = RandomGenerator.RandomString(10);
        ActionType = actionType;
        EntityName = entityName;
        UserId = httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM";
        Timestamp = DateTime.UtcNow;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        AdditionalInfo = additionalInfo;
    }
    public string ActionType { get; set; } // Create, Update, Delete, Read
    public string EntityName { get; set; } // Entity being modified
    public string UserId { get; set; } // User performing the action
    public DateTime Timestamp { get; set; } // When the action occurred
    public string OldValues { get; set; } // Previous state of the entity
    public string NewValues { get; set; } // New state of the entity
    public string IpAddress { get; set; } // IP address of the user
    public string AdditionalInfo { get; set; }
}
