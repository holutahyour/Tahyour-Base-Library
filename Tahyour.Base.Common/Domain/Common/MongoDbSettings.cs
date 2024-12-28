namespace Tahyour.Base.Common.Domain.Common;

public class MongoDbSettings
{
    public string Host { get; init; }

    public int Port { get; init; }

    public string ConnectionString => $"mongodb://{Host}:{Port}";
}
