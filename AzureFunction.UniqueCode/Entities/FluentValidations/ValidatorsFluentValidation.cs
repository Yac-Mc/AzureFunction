using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.Entities.FluentValidations
{

    /// <summary>
    /// Validator class Event (The Event class is general for all events that UniqueCode handles)
    /// </summary>
    public class EventValidator : AbstractValidator<Event>
    {
        readonly IEnumerable<string> listEventTypeCodes = typeof(EnumUniqueCodeEventType).GetFields().Select(x => x.GetValue(null).ToString()).ToList();
        public EventValidator()
        {
            RuleFor(eventUniqueCode => eventUniqueCode.Code).NotEmpty();
            RuleFor(eventUniqueCode => eventUniqueCode.Date).NotEmpty();
            RuleFor(eventUniqueCode => eventUniqueCode.Area).NotEmpty();
            RuleFor(eventUniqueCode => eventUniqueCode.Code).Must(x => listEventTypeCodes.Contains(x)).WithMessage(x => $"El código del evento ({x.Code}) no es valido.");
        }
    }

    /// <summary>
    /// Validator class EventBRU (Event Born USDA)
    /// </summary>
    public class EventBRUValidator : AbstractValidator<EventBRU>
    {
        readonly IEnumerable<string> listTypeBRU = EnumEventBRU.GetListTypeBRU();
        readonly IEnumerable<string> listSerializationType = EnumEventBRU.GetListSerializationType();
        public EventBRUValidator()
        {
            RuleFor(eventBRU => eventBRU.Type).NotEmpty();
            RuleFor(eventBRU => eventBRU.SerializationType).NotEmpty();
            RuleFor(eventBRU => eventBRU.Type).Must(x => listTypeBRU.Contains(x.ToUpper())).WithMessage(x => $"El tipo para el vento BRU debe ser {string.Join(" o ", listTypeBRU)}");
            RuleFor(eventBRU => eventBRU.SerializationType).Must(x => listSerializationType.Contains(x.ToUpper())).WithMessage(x => $"El tipo de serialización del vento BRU debe ser {string.Join(", ", listSerializationType)}");
            RuleFor(eventBRU => eventBRU.Parameter1).NotEmpty().When(eventBRU => eventBRU.Type.Equals(EnumEventBRU.SERIALIZE));
            RuleFor(eventBRU => eventBRU.Parameter2).NotEmpty();
        }
    }

    public static class FluentValidationResponse
    {
        public static async Task<bool> ResultFluentValidation(ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                string errorMessage = string.Join("/", validationResult.Errors.Select(e => $"Error FluentValidation: El campo: {e.PropertyName} - {e.ErrorMessage} //").ToList());
                Console.WriteLine(errorMessage);
                throw new ArgumentException(errorMessage, EnumTypeExecptions.FLUENTVALIDATION);
            }
            return await Task.Run(() => validationResult.IsValid);
        }
    }
}
