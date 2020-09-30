using InvoiceManagementApp.Application.Common.Interfaces;
using InvoiceManagementApp.Application.Products.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceManagementApp.Application.Products.Queries
{
    public class GetAllProductsQuery : IRequest<IEnumerable<AllProductVm>>
    {
        // in this style the handler is withing the query rather than in it's own /Handlers folder
        // the advantage of this is ...
        public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<AllProductVm>>
        {
            private readonly IApplicationDbContext _context;
            public GetAllProductsQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<AllProductVm>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
            {
                var productList = await _context.Products.ToListAsync();
                if (productList == null)
                {
                    return null;
                }
                return productList.Select(p => new AllProductVm { Id=p.Id, Name=p.Name, BuyingPrice=p.BuyingPrice }).ToList().AsReadOnly();
            }
        }
    }
}
