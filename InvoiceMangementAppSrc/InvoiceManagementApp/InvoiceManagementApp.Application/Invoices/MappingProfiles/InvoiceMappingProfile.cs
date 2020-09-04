using AutoMapper;
using InvoiceManagementApp.Application.Invoices.Commands;
using InvoiceManagementApp.Application.Invoices.VIewModels;
using InvoiceManagementApp.Domain.Entities;

namespace InvoiceManagementApp.Application.Invoices.MappingProfiles
{
    public class InvoiceMappingProfile : Profile
    {
        public InvoiceMappingProfile()
        {
            CreateMap<Invoice, InvoiceVm>().ReverseMap();
           
            //just an example of how to do it if the names are different.
            CreateMap<InvoiceItem, InvoiceItemVm>().ConstructUsing(i => new InvoiceItemVm
            {
                Id = i.Id,
                Item = i.Item,
                Quantity = i.Quantity,
                Rate = i.Rate
            });         
            
            CreateMap<InvoiceItemVm, InvoiceItem>(); // use reverse map in reality

            CreateMap<CreateInvoiceCommand, Invoice>();
        }
    }
}
