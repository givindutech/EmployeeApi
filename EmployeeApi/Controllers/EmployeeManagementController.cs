using EmployeeApi.Helper;
using EmployeeApi.Model;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeManagementController : ControllerBase
    {
        public readonly DataBaseContext _dataBaseContext;
        public ICacheProvider _cacheProvider;
        public readonly ILogger<EmployeeManagementController> _logger;
        public EmployeeManagementController(DataBaseContext dataBaseContext, ICacheProvider cacheProvider, ILogger<EmployeeManagementController> logger)
        {
            _dataBaseContext = dataBaseContext;
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        [HttpGet("GetEmployee")]
        public async Task<IActionResult> GetEmployee()
        {
            const string method = nameof(GetEmployee);
            _logger.LogInformation("{Method} - start", method);
            try
            {
                var employee = await _dataBaseContext.Employees.ToListAsync();
                _logger.LogInformation("{Method} - fetched {Count} employees", method, employee?.Count ?? 0);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - unexpected error", method);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while fetching employees", Result = false, Data = null });
            }
        }

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            const string method = nameof(GetEmployees);
            _logger.LogInformation("{Method} - start", method);
            try
            {
                var employees = await _dataBaseContext.Employees.ToListAsync();
                _logger.LogInformation("{Method} - returning {Count} employees", method, employees?.Count ?? 0);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - unexpected error", method);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while fetching employees", Result = false, Data = null });
            }
        }

        //Add Employee
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            const string method = nameof(AddEmployee);
            _logger.LogInformation("{Method} - start. EmployeeId: {Id}, Email: {Email}", method, employee?.EmployeeId, employee?.EmailId);
            try
            {
                if (await _dataBaseContext.Employees.AnyAsync(e => e.EmployeeId == employee.EmployeeId))
                {
                    _logger.LogWarning("{Method} - duplicate employee id {Id}", method, employee.EmployeeId);
                    return BadRequest("Employee ID Already Exists");
                }

                await _dataBaseContext.Employees.AddAsync(employee);
                await _dataBaseContext.SaveChangesAsync();

                _logger.LogInformation("{Method} - employee added with id {Id}", method, employee.EmployeeId);

                var response = new ApiResponse<object>
                {
                    result_Message = "Employee Add Succesfully",
                    Result = true,
                    Data = employee
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error adding employee", method);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while adding employee", Result = false, Data = null });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee updateEmployee)
        {
            const string method = nameof(UpdateEmployee);
            _logger.LogInformation("{Method} - start. Id: {Id}", method, id);
            try
            {
                var employee = await _dataBaseContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
                if (employee == null)
                {
                    _logger.LogWarning("{Method} - employee not found. Id: {Id}", method, id);
                    return NotFound(new ApiResponse<object>
                    {
                        result_Message = "User details not found",
                        Result = false,
                        Data = null
                    });
                }

                employee.EmployeeName = updateEmployee.EmployeeName;
                employee.ContactNo = updateEmployee.ContactNo;
                employee.EmailId = updateEmployee.EmailId;
                employee.DeptId = updateEmployee.DeptId;
                employee.Password = updateEmployee.Password;
                employee.Gender = updateEmployee.Gender;
                employee.Role = updateEmployee.Role;
                employee.CreatedDate = updateEmployee.CreatedDate;

                await _dataBaseContext.SaveChangesAsync();

                _logger.LogInformation("{Method} - employee updated. Id: {Id}", method, id);

                var response = new ApiResponse<object>
                {
                    result_Message = "Employee Updated Succesfully now",
                    Result = true,
                    Data = updateEmployee
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error updating employee. Id: {Id}", method, id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while updating employee", Result = false, Data = null });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getEmployeesById(int? id)
        {
            const string method = nameof(getEmployeesById);
            _logger.LogInformation("{Method} - start. Id: {Id}", method, id);
            try
            {
                var employeeDetails = await _dataBaseContext.Employees.FindAsync(id);
                if (employeeDetails == null)
                {
                    _logger.LogWarning("{Method} - not found. Id: {Id}", method, id);
                    return NotFound(new ApiResponse<object>
                    {
                        result_Message = "User details not found",
                        Result = false,
                        Data = null
                    });
                }

                _logger.LogInformation("{Method} - found employee. Id: {Id}", method, id);

                var response = new ApiResponse<object>
                {
                    result_Message = "user details found",
                    Result = true,
                    Data = employeeDetails
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error fetching employee by id. Id: {Id}", method, id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while fetching employee", Result = false, Data = null });
            }
        }

        [HttpPost("getLogin")]
        public async Task<IActionResult> GetLogin([FromBody] LoginDetails employee)
        {
            const string method = nameof(GetLogin);
            _logger.LogInformation("{Method} - login attempt for Email: {Email}", method, employee?.email);
            try
            {
                var user = await _dataBaseContext.Employees.SingleOrDefaultAsync(e => e.EmailId == employee.email && e.Password == employee.password);
                if (user == null)
                {
                    _logger.LogWarning("{Method} - invalid credentials for Email: {Email}", method, employee?.email);
                    return NotFound(new ApiResponse<object> { Data = null, result_Message = "user details not found", Result = false });
                }

                _logger.LogInformation("{Method} - login successful for EmployeeId: {Id}", method, user.EmployeeId);

                var response = new ApiResponse<object>
                {
                    result_Message = "user details found",
                    Result = true,
                    Data = new
                    {
                        user.EmployeeId,
                        user.EmployeeName,
                        user.ContactNo,
                        user.EmailId,
                        user.DeptId,
                        // Do not include password in logs/response in production
                        user.Password,
                        user.Gender,
                        user.Role,
                        user.CreatedDate
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error during login for Email: {Email}", method, employee?.email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while processing login", Result = false, Data = null });
            }
        }

        [HttpPost("AddNewDepartment")]
        public async Task<IActionResult> AddNewDepartment([FromBody] ParentDepartmentDto parentDepartmentDto)
        {
            const string method = nameof(AddNewDepartment);
            _logger.LogInformation("{Method} - start. DepartmentName: {Dept}", method, parentDepartmentDto?.DepartmentName);
            try
            {
                var parentItem = new ParentDepartment
                {
                    DepartmentName = parentDepartmentDto.DepartmentName,
                    DepartmentLogo = parentDepartmentDto.DepartmentLogo
                };
                await _dataBaseContext.ParentDepartments.AddAsync(parentItem);
                await _dataBaseContext.SaveChangesAsync();

                _logger.LogInformation("{Method} - parent department created with id {Id}", method, parentItem.DepartmentId);

                var response = new ApiResponse<object>
                {
                    result_Message = "Data Creation Successfull",
                    Result = true,
                    Data = new ParentDepartment
                    {
                        DepartmentId = parentItem.DepartmentId,
                        DepartmentName = parentDepartmentDto.DepartmentName,
                        DepartmentLogo = parentDepartmentDto.DepartmentLogo
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error creating parent department", method);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while creating department", Result = false, Data = null });
            }
        }

        [HttpPost("AddChildDepartment")]
        public async Task<IActionResult> AddChildDepartment([FromBody] ChildDepartmentDto parentDepartmentDto)
        {
            const string method = nameof(AddChildDepartment);
            _logger.LogInformation("{Method} - start. ParentDeptId: {ParentId}, Name: {Name}", method, parentDepartmentDto?.ParentDeptId, parentDepartmentDto?.DepartmentName);
            try
            {
                var childItem = new ChildDepartment
                {
                    DepartmentName = parentDepartmentDto.DepartmentName,
                    ParentDeptId = parentDepartmentDto.ParentDeptId,
                };
                await _dataBaseContext.ChildDepartments.AddAsync(childItem);
                await _dataBaseContext.SaveChangesAsync();

                _logger.LogInformation("{Method} - child department created with id {Id}", method, childItem.ChildDeptId);

                var response = new ApiResponse<object>
                {
                    result_Message = "Data Creation Successfull",
                    Result = true,
                    Data = new ChildDepartment
                    {
                        ChildDeptId = childItem.ChildDeptId,
                        DepartmentName = parentDepartmentDto.DepartmentName,
                        ParentDeptId = parentDepartmentDto.ParentDeptId
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error creating child department", method);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while creating child department", Result = false, Data = null });
            }
        }

        [HttpGet("GetParentDepartments")]
        public async Task<IActionResult> GetParentDepartments()
        {
            const string method = nameof(GetParentDepartments);
            _logger.LogInformation("{Method} - start", method);
            try
            {
                var parentResult = await _dataBaseContext.ParentDepartments.ToListAsync();

                var response = new ApiResponse<object>
                {
                    result_Message = "Data Createment succesfull",
                    Result = true,
                    Data = parentResult
                };
                _logger.LogInformation("{Method} - returning {Count} parent departments", method, parentResult?.Count ?? 0);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error fetching parent departments", method);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while fetching parent departments", Result = false, Data = null });
            }
        }

        [HttpGet("GetChildDepartments/{id}")]
        public async Task<IActionResult> GetChildDepartments(int? id)
        {
            const string method = nameof(GetChildDepartments);
            _logger.LogInformation("{Method} - start. ParentId: {ParentId}", method, id);
            try
            {
                var children = await _dataBaseContext.ChildDepartments.Where(c => c.ParentDeptId == id).Select
                    (c => new
                    {
                        childDeptId = c.ChildDeptId,
                        parentDeptId = c.ParentDeptId,
                        departmentName = c.DepartmentName
                    }).ToListAsync();
                _logger.LogInformation("{Method} - returning {Count} children for parent {ParentId}", method, children?.Count ?? 0, id);

                var response = new ApiResponse<object>
                {
                    result_Message = "Data Createment succesfull",
                    Result = true,
                    Data = children
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} - error fetching child departments for parent {ParentId}", method, id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> { result_Message = "An error occurred while fetching child departments", Result = false, Data = null });
            }
        }
    }

    public class ApiResponse<T>
    {
        public string result_Message { get; set; }
        public bool Result { get; set; }
        public T Data { get; set; }
    }
}