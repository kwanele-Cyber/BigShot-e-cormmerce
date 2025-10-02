using BigShotCore.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using BigShotCore.Data.Services;
using BigShotCore.Extensions;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Only Admins can see all orders
    [HttpGet("all")]
    [AuthorizeRole("Admin")]
    public async Task<IActionResult> ListAll(int pageSize = 10, int pageIndex = 0)
    {
        var orders = await _orderService.ListOrdersAsync(pageSize, pageIndex);
        return Ok(orders);
    }

    // Admins see all orders; customers see only their own
    [HttpGet("my-orders")]
    [AuthorizeRole("Admin", "Customer")]
    public async Task<IActionResult> ListMyOrders(int pageSize = 10, int pageIndex = 0)
    {
        var user = HttpContext.GetCurrentUser();
        IEnumerable<OrderDto> orders;

        if (user.Role.Name == "Admin")
        {
            orders = await _orderService.ListOrdersAsync(pageSize, pageIndex);
        }
        else
        {
            orders = await _orderService.ListOrdersAsync(pageSize, pageIndex, user.Id);
        }

        return Ok(orders);
    }

    // Only customers can add orders
    [HttpPost]
    [AuthorizeRole("Customer")]
    public async Task<IActionResult> AddOrder(CreateOrderDto dto)
    {
        var user = HttpContext.GetCurrentUser();
        var order = await _orderService.AddOrderAsync(dto, user.Id);
        return Ok(order);
    }

    // Only Admins can delete any order
    [HttpDelete("{id}")]
    [AuthorizeRole("Admin")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var deleted = await _orderService.DeleteOrderAsync(id);
        return deleted ? Ok() : NotFound();
    }
}
