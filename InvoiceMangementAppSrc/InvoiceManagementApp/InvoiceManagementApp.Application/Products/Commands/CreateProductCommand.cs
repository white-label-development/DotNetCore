using InvoiceManagementApp.Application.Common.Interfaces;
using InvoiceManagementApp.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceManagementApp.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Rate { get; set; }


        public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateProductCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            {
                var product = new Product
                {
                    Barcode = command.Barcode,
                    Name = command.Name,
                    BuyingPrice = command.BuyingPrice,
                    Rate = command.Rate,
                    Description = command.Description
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync(cancellationToken);

                return product.Id;
            }
        }
    }
}
