namespace Tahyour.Base.Common.Domain.Entities;

public class AuditableEntity
{
    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime LastModifiedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool? IsActive { get; set; } = true;

    public bool? IsDeleted { get; set; } = false;
}
