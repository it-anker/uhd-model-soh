using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SOH.Process.Server.Attributes;

/// <summary>
///     Model state validation attribute.
/// </summary>
public class ValidateModelStateAttribute : ActionFilterAttribute
{
    /// <summary>
    ///     Called before the action method is invoked
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ControllerActionDescriptor? descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (descriptor != null)
        {
            foreach (ParameterInfo parameter in descriptor.MethodInfo.GetParameters())
            {
                object? args = null;
                if (context.ActionArguments.TryGetValue(parameter.Name, out object? argument)) args = argument;

                ValidateAttributes(parameter, args, context.ModelState);
            }
        }

        if (!context.ModelState.IsValid) context.Result = new BadRequestObjectResult(context.ModelState);
    }

    private void ValidateAttributes(ParameterInfo parameter, object args, ModelStateDictionary modelState)
    {
        foreach (CustomAttributeData attributeData in parameter.CustomAttributes)
        {
            Attribute? attributeInstance = parameter.GetCustomAttribute(attributeData.AttributeType);

            ValidationAttribute? validationAttribute = attributeInstance as ValidationAttribute;
            if (validationAttribute != null)
            {
                bool isValid = validationAttribute.IsValid(args);
                if (!isValid)
                    modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
            }
        }
    }
}