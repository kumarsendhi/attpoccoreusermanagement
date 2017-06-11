using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood.ViewModels
{
    public class ChangePasswordViewModel
    {
        
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
