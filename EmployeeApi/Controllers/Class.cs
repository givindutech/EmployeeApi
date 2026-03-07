//using EmployeeApi.Controllers;

//namespace EmployeeApi.Controllers
//{
//    public interface IDiscountStrategy 
//    { 
//        decimal ApplyDiscount(decimal price); 
//    }
//    public class FixedDiscount :IDiscountStrategy
//    {
//        public decimal ApplyDiscount(decimal p) => p * 0.9m;
//    }
//    public class SeasonalDiscount : IDiscountStrategy
//    {
//        public decimal ApplyDiscount(decimal p) => p * 0.8m;
//    }
//}
//    public class Class
//    {
//    public readonly IDiscountStrategy _strategy;
//    public Class(IDiscountStrategy strategy) {
//        _strategy = strategy;   
//    }
//    public decimal Apply(decimal price)
//    {
//        _strategy = new FixedDiscount(price);
//    }


//    }

