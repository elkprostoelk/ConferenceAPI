using ConferenceAPI.Web.Models;
using FluentValidation;

namespace ConferenceAPI.Web.Validators
{
    public class CreateZoomMeetingModelValidator : AbstractValidator<CreateZoomMeetingModel>
    {
        public CreateZoomMeetingModelValidator()
        {
            RuleFor(m => m.Topic)
                .NotEmpty().WithMessage("Topic should be specified!")
                .MaximumLength(200).WithMessage("Topic should not be longer than 200 characters!"); // constraint based on the Zoom API rules

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("User email must be specified!")
                .EmailAddress().WithMessage("Email address is not valid!")
                .MaximumLength(100).WithMessage("Email address should not be longer than 100 symbols!");

            RuleFor(m => m.StartTime)
                .NotEmpty().WithMessage("Starting date and time must be specified!")
                .GreaterThan(DateTime.Now).WithMessage("Starting date and time must be later than now!");

            RuleFor(m => m.Duration)
                .GreaterThan(0).WithMessage("Meeting duration must be a positive number!");

            RuleFor(m => m.Type)
                .InclusiveBetween(1, 4).WithMessage("Meeting type must be specified as 1, 2, 3 or 4!");
        }
    }
}
