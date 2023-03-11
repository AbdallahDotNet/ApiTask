using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Interfaces.ViewModels.UserVM;

namespace Services.Validator.UserValidator
{
    public class UserValidator : AbstractValidator<SaveUserViewModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
