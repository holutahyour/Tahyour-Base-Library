namespace Tahyour.Base.Common.Services
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            ConfigureStandardMappings();
            ConfigureCustomMappings();
        }

        private void ConfigureStandardMappings()
        {
            // Standard mappings without custom member configurations
            var mappings = new[]
            {
                typeof(Item)
            };

            foreach (var type in mappings)
            {
                CreateMap(type, GetDtoType(type)).ReverseMap();
                //CreateMap(type, GetCreateDtoType(type)).ReverseMap();
                //CreateMap(type, GetUpdateDtoType(type)).ReverseMap();
            }
        }

        private void ConfigureCustomMappings()
        {
            // Custom mappings with member configurations           
        }

        private Type GetDtoType(Type entityType)
        {
            return Type.GetType($"{entityType.Namespace}.{entityType.Name}DTO");
        }

        private Type GetCreateDtoType(Type entityType)
        {
            return Type.GetType($"{entityType.Namespace}.Create{entityType.Name}DTO");
        }

        private Type GetUpdateDtoType(Type entityType)
        {
            return Type.GetType($"{entityType.Namespace}.Update{entityType.Name}DTO");
        }
    }
}
