namespace Tahyour.Base.Common.Domain.Entities;

public class BaseEntity<T> : AuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required T Id { get; set; }

    public required string Code { get; set; }
}
