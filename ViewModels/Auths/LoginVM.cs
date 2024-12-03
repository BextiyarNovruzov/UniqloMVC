using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FrontToBackMvc.ViewModels.Auths
{
    public class LoginVM
    {
        public string UserNameOrEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
