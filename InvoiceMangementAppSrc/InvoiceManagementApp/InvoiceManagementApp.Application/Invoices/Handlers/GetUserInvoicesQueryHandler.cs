using InvoiceManagementApp.Application.Common.Interfaces;
using InvoiceManagementApp.Application.Invoices.Queries;
using InvoiceManagementApp.Application.Invoices.VIewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceManagementApp.Application.Invoices.Handlers
{
    public class GetUserInvoicesQueryHandler : IRequestHandler<GetUserInvoicesQuery, IList<InvoiceVm>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserInvoicesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<InvoiceVm>> Handle(GetUserInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _context.Invoices.Include(i => i.InvoiceItems)
                .Where(i => i.CreatedBy == request.User).ToListAsync();

            var vm = invoices.Select(i => new InvoiceVm
            {
                AmountPaid = i.AmountPaid,
                Date = i.Date,
                Discount = i.Discount,
                DiscountType = i.DiscountType,
                DueDate = i.DueDate,
                From = i.From,
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                Logo = i.Logo,
                PaymentTerms = i.PaymentTerms,
                Tax = i.Tax,
                TaxType = i.TaxType,
                To = i.To,
                InvoiceItems = i.InvoiceItems.Select(i => new InvoiceItemVm
                {
                    Id = i.Id,
                    Item = i.Item,
                    Quantity = i.Quantity,
                    Rate = i.Rate
                }).ToList()
            }).ToList();

            return vm;
        }
    }
}
