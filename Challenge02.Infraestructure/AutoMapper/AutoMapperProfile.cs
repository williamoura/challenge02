using AutoMapper;
using Challenge02.Domain.Commands;
using Challenge02.Domain.Events;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Clients.Contracts;

namespace Challenge02.Infraestructure.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpdateDevCommand, Dev>().ReverseMap();
            CreateMap<UpdateDevCommand, UpdateDevEvent>();

            CreateMap<UpdateDevEvent, UpdateDevsRequest>();
            CreateMap<UpdateDevEvent, Dev>().ReverseMap();

            CreateMap<AddDevCommand, Dev>();
            CreateMap<AddDevCommand, AddDevEvent>();
            CreateMap<AddDevEvent, CreateDevsRequest>();
            CreateMap<AddDevEvent, Dev>().ReverseMap();

            CreateMap<Dev, CreateDevsRequest>();
            CreateMap<Dev, UpdateDevsRequest>();
        }
    }
}
