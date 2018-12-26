using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Tubumu.Modules.Admin.Models
{
    public enum UserStatus
    {
        [Display(Name = "默认")]
        Normal = 1,
        [Display(Name = "待审")]
        PendingApproval = 2,
        [Display(Name = "待删")]
        Removed = 3
    }
}
