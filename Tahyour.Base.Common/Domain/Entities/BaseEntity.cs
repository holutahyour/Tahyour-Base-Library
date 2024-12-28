namespace Tahyour.Base.Common.Domain.Entities;

public class BaseEntity<T> : AuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; }

    public string Code { get; set; }
}
