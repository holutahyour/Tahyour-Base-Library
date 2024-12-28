namespace Tahyour.Base.Common.Services
{
    public class MapperConfig : Profile
    {
        private List<Type> mappings;
        public MapperConfig()
        {
            ConfigureStandardMappings();
            ConfigureCustomMappings();
        }

        private void ConfigureStandardMappings()
        {

            foreach (var type in mappings)
            {
                CreateMap(type, GetDtoType(type)).ReverseMap();
                //CreateMap(type, GetCreateDtoType(type)).ReverseMap();
                //CreateMap(type, GetUpdateDtoType(type)).ReverseMap();
            }
        }

        public virtual void AddMappingType(Type type)
        {
            mappings.Add(type);
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
