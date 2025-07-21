using FluentValidation;

namespace Blogger.Areas.Administrator.Models
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator() 
        { 
            RuleFor(p => p.FirstName).NotEmpty().WithMessage("First name is required.")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.");
            RuleFor(p => p.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(p => p.Age).InclusiveBetween(0, 120).WithMessage("Age must be between 0 and 120.");
        }
    }
}
