using InvoiceManagementApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InvoiceManagementApp.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Invoice> Invoices { get; set; }

        DbSet<InvoiceItem> InvoiceItems { get; set; }

        // we need to override this
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
    }
}
