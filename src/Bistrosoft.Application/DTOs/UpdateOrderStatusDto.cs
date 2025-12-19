using Bistrosoft.Domain.Enums;

namespace Bistrosoft.Application.DTOs;

public record UpdateOrderStatusDto(
    Guid OrderId,
    OrderStatus Status
);

