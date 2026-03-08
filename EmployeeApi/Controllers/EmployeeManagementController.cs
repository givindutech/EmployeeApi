using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using EmployeeApi.ExceptionFilter;
using EmployeeApi.Helper;
using EmployeeApi.Model;
using LazyCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeManagementController : ControllerBase
    {
        public readonly DataBaseContext _dataBaseContext;
        public ICacheProvider _cacheProvider;
        public readonly ILogger<EmployeeManagementController> _logger;
        public EmployeeManagementController(DataBaseContext dataBaseContext,ICacheProvider cacheProvider, ILogger<EmployeeManagementController> logger)
        {
            _dataBaseContext = dataBaseContext;
            _cacheProvider = cacheProvider;
            _logger = logger;
        }


        [HttpGet("GetEmployee")]
        public async Task<IActionResult> GetEmployee()
        {
        var    employee = await _dataBaseContext.Employees.ToListAsync();
            return Ok(employee);
        }

        [HttpGet("GetEmployees")]
      //  [TypeFilter(typeof(HttpResponseExceptionFilter))]
        public async Task<IActionResult> GetEmployees()
        {
         //   throw new Exception("Excpetion Occures fetching all the employees");
            //if(!_cacheProvider.TryGetValue(CacheKeys.Employee,out List<Employee> employee))
            //{
            //    employee= await _dataBaseContext.Employees.ToListAsync();
            //    var chacheEntryOptions = new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpiration = DateTime.Now.AddSeconds(30),
            //        SlidingExpiration=TimeSpan.FromSeconds(30),
            //        Size=1024
            //    };
            //    _cacheProvider.Set(CacheKeys.Employee,employee, chacheEntryOptions);
            //}
            return Ok(await _dataBaseContext.Employees.ToListAsync());

            //var employeeDetails =await _dataBaseContext.Employees.ToListAsync();
            //if (employeeDetails == null)
            //{
            //    return NotFound(new ApiResponse<object>
            //    {

            //        Message = "User not found",
            //        Result = false,
            //        Data = null
            //    });
            //}

            //var response = new ApiResponse<object>
            //{
            //    Message = "user found",
            //    Result = true,
            //    Data = employeeDetails
            //};
            //return Ok(response);
        }
        //Add Employee
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody]Employee employee)
        {
            if(_dataBaseContext.Employees.Any(e => e.EmployeeId == employee.EmployeeId))
            {
                return BadRequest("Employee ID Already Exists");
            }
             await _dataBaseContext.Employees.AddAsync(employee);
            _dataBaseContext.SaveChangesAsync();
            var response = new ApiResponse<object>
            {
                Message = "Employee Add Succesfully",
                Result = true,
                Data = employee
            };
            return Ok(response);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody]Employee updateEmployee)
        {
            var employee = _dataBaseContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
            if(employee == null)
            {
                return NotFound(new ApiResponse<object>
                {

                    Message = "User not found",
                    Result = false,
                    Data = null
                });
            }
            employee.Result.EmployeeName = updateEmployee.EmployeeName;
            employee.Result.ContactNo = updateEmployee.ContactNo;
            employee.Result.EmailId = updateEmployee.EmailId;
            employee.Result.DeptId = updateEmployee.DeptId;
            employee.Result.Password = updateEmployee.Password;
            employee.Result.Gender = updateEmployee.Gender;
            employee.Result.Role = updateEmployee.Role;
            employee.Result.CreatedDate = updateEmployee.CreatedDate;
            _dataBaseContext.SaveChangesAsync();
            var response = new ApiResponse<object>
            {
                Message = "Employee Updated Succesfully",
                Result = true,
                Data = updateEmployee
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getEmployeesById(int? id)
        {
            var employeeDetails=await _dataBaseContext.Employees.FindAsync(id);
            if(employeeDetails == null)
            {
                return NotFound(new ApiResponse<object>
                {

                    Message = "User not found",
                    Result = false,
                    Data = null
                });
            }

            var response = new ApiResponse<object>
            {
                Message = "user found",
                Result = true,
                Data = employeeDetails
                //new
                //{
                //    employeeDetails.Result.EmployeeId,
                //    employeeDetails.Result.EmployeeName,
                //    employeeDetails.Result.ContactNo,
                //    employeeDetails.Result.EmailId,
                //    employeeDetails.Result.DeptId,
                //    employeeDetails.Result.Password,
                //    employeeDetails.Result.Gender,
                //    employeeDetails.Result.Role,
                //    employeeDetails.Result.CreatedDate
                //}
            };
            return Ok(response);
        }
        [HttpPost("getLogin")]
        public async Task<IActionResult> GetLogin(LoginDetails employee)
        {
            var user=await _dataBaseContext.Employees.SingleOrDefaultAsync(e=>e.EmailId==employee.email &&e.Password==employee.password);
            if(user == null)
            {
                return NotFound(new ApiResponse<object>
                { Data = null, Message = "user not found", Result = false });
            }

                var response = new ApiResponse<object>
                {
                    Message = "user found",
                    Result = true,
                    Data = new
                    {
                        user.EmployeeId,
                        user.EmployeeName,
                        user.ContactNo,
                        user.EmailId,
                        user.DeptId,
                        user.Password,
                        user.Gender,
                        user.Role,
                        user.CreatedDate
                    }
                };
            
            return Ok(response);
        }
        [HttpPost("AddNewDepartment")]
        public async Task<IActionResult> AddNewDepartment([FromBody]ParentDepartmentDto parentDepartmentDto)
        {
            var parentItem = new ParentDepartment
            {
                DepartmentName= parentDepartmentDto.DepartmentName,
                DepartmentLogo = parentDepartmentDto.DepartmentLogo
            };
           var result=  _dataBaseContext.ParentDepartments.AddAsync(parentItem);
            await _dataBaseContext.SaveChangesAsync();
            var response = new ApiResponse<object>
            {
                Message = "Data Creation Successfull",
                Result = true,
                Data = new ParentDepartment
                {
                    DepartmentId = parentDepartmentDto.DepartmentId,
                    DepartmentName = parentDepartmentDto.DepartmentName,
                    DepartmentLogo = parentDepartmentDto.DepartmentLogo

                }
            };
            return Ok(response);
               
        }
        [HttpPost("AddChildDepartment")]
        public async Task<IActionResult> AddChildDepartment([FromBody] ChildDepartmentDto parentDepartmentDto)
        {
            var childItem = new ChildDepartment
            {
                DepartmentName = parentDepartmentDto.DepartmentName,
                ParentDeptId = parentDepartmentDto.ParentDeptId,
               
            };
            var result = _dataBaseContext.ChildDepartments.AddAsync(childItem);
            await _dataBaseContext.SaveChangesAsync();
            var response = new ApiResponse<object>
            {
                Message = "Data Creation Successfull",
                Result = true,
                Data = new ChildDepartment
                {
                    ChildDeptId = parentDepartmentDto.ChildDeptId,
                    DepartmentName = parentDepartmentDto.DepartmentName,
                    ParentDeptId = parentDepartmentDto.ParentDeptId

                }
            };
            return Ok(response);

        }
        [HttpGet("GetParentDepartments")]
        public async Task<IActionResult> GetParentDepartments()
        {

            var parentResult = _dataBaseContext.ParentDepartments.ToList();

            var response = new ApiResponse<object>
            {
                Message = "Data Createment succesfull",
                Result = true,
                Data= parentResult
            };
            _logger.LogInformation("Get the Parentdepartments data {Time}", DateTime.UtcNow.ToString());
            return Ok(response);
        }
       
        [HttpGet("GetChildDepartments/{id}")]
        public async Task<IActionResult> GetChildDepartments(int ? id)
        {
            var children = await _dataBaseContext.ChildDepartments.Where(c => c.ParentDeptId == id).Select
                (c => new
                {
                    childDeptId = c.ChildDeptId,
                    parentDeptId= c.ParentDeptId,
                    departmentName= c.DepartmentName
                }).ToListAsync();
            var response = new ApiResponse<object>
            {
                Message = "Data Createment succesfull",
                Result = true,
                Data= children
            };
            return Ok(response);
        }

    }

    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public T Data { get; set; }
    }

}
