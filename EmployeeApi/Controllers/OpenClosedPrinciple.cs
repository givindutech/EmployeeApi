using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenClosedPrinciple : ControllerBase
    {
    }
    public interface IDiscountStrategy
    {
        decimal ApplyDiscount(decimal price);
    }
     public class HolidayDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price) => price * 0.9m;
    }

    public class NoDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price) => price;
    }
    public class OrderService
    {
        private readonly IDiscountStrategy _discount;
        public OrderService(IDiscountStrategy discount) 
        {
            _discount = discount; 
        }

        public decimal ComputeTotal(decimal price)
        {
            return _discount.ApplyDiscount(price);
        }
    }
}
