namespace Ecommarce.Application.Features.Products.Commands.DeleteProduct;

using Ecommarce.Application.Common.Exceptions;
using Ecommarce.Application.Common.Interfaces;
using MediatR;

public record DeleteProductCommand(int Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = _context.Products
            .FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.ProductEntity), request.Id);
        }

        product.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
