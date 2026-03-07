using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeApi.Model
{
    public class ChildDepartment
    {
        [Key]
        public int ChildDeptId { get; set; }
        public int ParentDeptId { get; set; }
        public string DepartmentName { get; set; } = null!;

        [ForeignKey(nameof(ParentDeptId))]
        public ParentDepartment? ParentDepartment { get; set; }
    }
}
