using AutoMapper;

namespace UmaPay.Repository
{

    using Domain;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region Application

            CreateMap<Entities.Application, Application>()
                 .ForMember(domain => domain.Id, entity => entity.MapFrom(src => src.ApplicationId))
                 .ReverseMap();

            CreateMap<Entities.Country, Country>()
               .ForMember(domain => domain.Id, entity => entity.MapFrom(src => src.CountryId))
               .ReverseMap();

            CreateMap<Entities.Customer, Customer>()
                .ForMember(domain => domain.Id, entity => entity.MapFrom(src => src.CustormerId))
                .ReverseMap();

            CreateMap<Entities.Gateway, Gateway>()
                .ForMember(domain => domain.Id, entity => entity.MapFrom(src => src.GatewayId))
                .ReverseMap();

            CreateMap<Entities.GatewayApplication, GatewayApplication>()
                 .ReverseMap();

            CreateMap<Entities.GatewayCountry, GatewayCountry>()
                .ReverseMap();

            CreateMap<Entities.TransactionStatus, TransactionStatus>()
                .ReverseMap();

            CreateMap<Entities.TransactionInvoice, Invoice>()
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsPaid))
                .ForMember(dest => dest.TotalToPay, opt => opt.MapFrom(src => src.TotalToPay))
                .ReverseMap();

            CreateMap<Entities.Transaction, Transaction>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TransactionId))
            .ReverseMap();

            CreateMap<Entities.TransactionStatusLog, TransactionStatusLog>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TransactionStatusLogId))
                .ReverseMap();

            #endregion Application

        }

    }
}
