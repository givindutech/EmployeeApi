namespace EmployeeApi.Model
{
    public class ChildDepartmentDto
    {
        public int ChildDeptId { get; set; }   // ignore for insert
        public int ParentDeptId { get; set; }
        public string DepartmentName { get; set; } = null!;
    }
}
