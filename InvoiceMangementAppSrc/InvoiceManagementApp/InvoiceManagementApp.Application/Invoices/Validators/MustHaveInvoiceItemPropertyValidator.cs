using FluentValidation.Validators;
using InvoiceManagementApp.Application.Invoices.VIewModels;
using System.Collections.Generic;
using System.Linq;

namespace InvoiceManagementApp.Application.Invoices.Validators
{
    public class MustHaveInvoiceItemPropertyValidator : PropertyValidator
    {
        public MustHaveInvoiceItemPropertyValidator() : base("Property {PropertyName} should not be an empty list.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var list = context.PropertyValue as IList<InvoiceItemVm>;
            return list != null && list.Any();
        }
    }
}
