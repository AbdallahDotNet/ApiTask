using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Interfaces.ViewModels.ToDoVM;
using Interfaces.ViewModels.UserVM;

namespace Services.Validator.ToDoListValidator
{
    public class ToDoListValidator : AbstractValidator<SaveToDoViewModel>
    {
        public ToDoListValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.IsCompleted).NotEmpty();
        }
    }
}
