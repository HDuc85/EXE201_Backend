using FluentValidation;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Data.ViewModel.Authen
{
    public class LoginRequest
    {

        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x .Username).NotEmpty();

            RuleFor(x => x.Password).NotEmpty();
        }
    }


}
