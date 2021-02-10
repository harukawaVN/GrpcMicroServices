using System;
using AutoMapper;
using BookGrpcService;
using BookGrpcServer.Models;
namespace BookGrpcServer.Mapper
{
    public class BookMapper : Profile
    {
        public BookMapper()
        {
            CreateMap<Book,CustomerBookDataReponse>();
            CreateMap<CustomerCreateOrUpdateBookRequest, Book>();
        }
    }
}
