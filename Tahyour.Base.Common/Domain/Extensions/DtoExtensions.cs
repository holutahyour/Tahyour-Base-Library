namespace Tahyour.Base.Common.Domain.Extensions;

public static class DtoExtensions
{
    public static T Map<T, J>(this J entity, IMapper mapper)
    {
        return mapper.Map<T>(entity);
    }

    public static ItemDTO Map(this Item entity, IMapper mapper)
    {
        return mapper.Map<ItemDTO>(entity);
    }
}
