using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tubumu.Modules.Framework.Extensions
{
    public static class ModelStateExtensions
    {
        public static string FirstErrorMessage(this ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                return String.Empty;
            }
            var item = modelState.FirstOrDefault(m => m.Value.Errors.Count > 0).Value.Errors.First();
            var firstErrorMessage = item.ErrorMessage;
            if (firstErrorMessage.IsNullOrWhiteSpace())
            {
                firstErrorMessage = item.Exception.Message;
            }
            return firstErrorMessage ?? "未指定错误。";
        }
    }
}
