using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Interfaces;
using Bistrosoft.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bistrosoft.Infrastructure.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Customer>()
            .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Customer>()
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Email.Value == email, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Customer>()
            .Include(c => c.Orders)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Set<Customer>().AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Set<Customer>().Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Set<Customer>().Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
