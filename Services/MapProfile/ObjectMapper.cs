using AutoMapper;
using Entities.Models;
using Interfaces.Helpers;
using Interfaces.ViewModels.ToDoVM;
using Interfaces.ViewModels.UserVM;

namespace Services.MapProfile
{
    public class ObjectMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<mapperProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class mapperProfile : Profile
    {
        public mapperProfile()
        {
            #region User
            CreateMap<User, SaveUserViewModel>().ReverseMap();
            #endregion

            #region ToDo
            CreateMap<ToDo, GetToDoViewModel>().ReverseMap();
            CreateMap<ToDo, SaveToDoViewModel>().ReverseMap();
            #endregion
        }
    }
}
