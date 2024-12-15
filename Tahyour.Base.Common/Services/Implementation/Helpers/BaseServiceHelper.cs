namespace Tahyour.Base.Common.Services.Implementation;

public class BaseServiceHelper
{
    public static Dictionary<string, object> SelectProperties<T>(T entity, string[] properties)
    {
        var result = new Dictionary<string, object>();
        foreach (var property in properties)
        {
            var propInfo = typeof(T).GetProperty(property, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (propInfo != null)
            {
                result[propInfo.Name.ToCamelCase()] = propInfo.GetValue(entity);
            }
        }
        return result;
    }
}
