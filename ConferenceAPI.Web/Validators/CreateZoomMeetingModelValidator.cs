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

            When(m => m.StartTime.HasValue, () =>
            {
                RuleFor(m => m.StartTime)
                    .GreaterThan(DateTime.Now)
                    .WithMessage("Starting date and time must be later than now!");
            });
            
            RuleFor(m => m.Duration)
                .GreaterThan(0)
                .WithMessage("Meeting duration must be a positive number!");

            RuleFor(m => m.Type)
                .IsInEnum()
                .WithMessage("Meeting type must be specified as an existing one!");

            When(m => m.Type == Core.Enums.MeetingType.RecurringMeetingNoFixedTime, () =>
            {
                RuleFor(m => m.StartTime)
                    .Null().WithMessage("Recurring meetings with no fixed time cannot have start time!");
            });

            When(m => m.Settings is not null && m.Settings.WaitingRoom == true && m.Settings.JoinBeforeHost.HasValue, () =>
            {
                RuleFor(m => m.Settings!.JoinBeforeHost)
                    .Equal(false)
                    .WithMessage("If the Waiting Room feature is enabled, JoinBeforeHost setting must be disabled!");
            });
        }
    }
}
