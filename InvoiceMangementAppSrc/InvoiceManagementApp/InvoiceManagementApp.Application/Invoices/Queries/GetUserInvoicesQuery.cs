using InvoiceManagementApp.Application.Invoices.VIewModels;
using MediatR;
using System.Collections.Generic;

namespace InvoiceManagementApp.Application.Invoices.Queries
{
    public class GetUserInvoicesQuery : IRequest<IList<InvoiceVm>>
    {
        public string User { get; set; }
    }
}
