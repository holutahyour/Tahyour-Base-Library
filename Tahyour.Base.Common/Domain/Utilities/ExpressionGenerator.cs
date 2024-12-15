namespace Tahyour.Base.Common.Domain.Utilities;

public static class ExpressionGenerator
{
    public static Expression<Func<T, bool>> GenerateLambda<T>(Dictionary<string, string> fieldValues)
    {
        // Check if the dictionary is empty or null
        if (fieldValues == null || fieldValues.Count == 0)
        {
            throw new ArgumentException("The fieldValues dictionary cannot be null or empty.");
        }

        // Start building the parameter for the lambda expression (e.g., p => ...)
        var parameter = Expression.Parameter(typeof(T), "p");

        // Initialize an expression for combining multiple conditions
        Expression? combinedExpression = null;

        // Loop through the dictionary of field-value pairs
        foreach (var fieldValuePair in fieldValues)
        {
            var fieldName = fieldValuePair.Key;
            var stringValues = fieldValuePair.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);

            Expression? fieldExpression = null;

            foreach (var value in stringValues)
            {
                var condition = GenerateExpression<T>(parameter, fieldName, value);
                fieldExpression = fieldExpression == null
                    ? condition
                    : Expression.OrElse(fieldExpression, condition);
            }

            combinedExpression = combinedExpression == null
                ? fieldExpression
                : Expression.AndAlso(combinedExpression, fieldExpression);
        }

        // If no valid expressions were created, throw an exception
        if (combinedExpression == null)
        {
            throw new ArgumentException("No valid expressions were created from the provided fieldValues.");
        }

        // Build the final lambda expression (e.g., p => p.Field1 == value1 && p.Field2 == value2)
        return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }

    private static Expression GenerateExpression<T>(ParameterExpression parameter, string fieldName, string stringValue)
    {
        // Get the property info for the field
        var propertyInfo = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found on entity '{typeof(T).Name}'.");
        }

        // Ensure the fieldValue is of the correct type for the property
        var targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

        object? fieldValue;

        try
        {
            fieldValue = ConvertFieldValue(targetType, stringValue);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Field '{fieldName}' conversion failed. {ex.Message}", ex);
        }

        // Create the property access expression (e.g., p => p.FieldName)
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

        // Create the constant expression for the fieldValue
        var constantValue = Expression.Constant(fieldValue, targetType);

        // Create the equality expression (e.g., p => p.FieldName == fieldValue)
        return Expression.Equal(propertyAccess, constantValue);
    }

    private static object ConvertFieldValue(Type targetType, string stringValue)
    {
        if (targetType == typeof(Guid))
        {
            return Guid.Parse(stringValue);
        }
        if (targetType == typeof(DateTime))
        {
            return DateTime.Parse(stringValue);
        }
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, stringValue);
        }
        if (targetType == typeof(int))
        {
            return int.Parse(stringValue);
        }
        if (targetType == typeof(decimal))
        {
            return decimal.Parse(stringValue);
        }
        if (targetType == typeof(bool))
        {
            return bool.Parse(stringValue);
        }
        if (targetType == typeof(double))
        {
            return double.Parse(stringValue);
        }
        if (targetType == typeof(float))
        {
            return float.Parse(stringValue);
        }
        if (targetType == typeof(long))
        {
            return long.Parse(stringValue);
        }

        // Default case for strings and other types
        return Convert.ChangeType(stringValue, targetType);
    }
}
