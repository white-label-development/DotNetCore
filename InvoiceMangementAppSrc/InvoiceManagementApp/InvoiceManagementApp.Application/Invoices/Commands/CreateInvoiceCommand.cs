using InvoiceManagementApp.Application.Invoices.VIewModels;
using InvoiceManagementApp.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace InvoiceManagementApp.Application.Invoices.Commands
{
    //The IRequest<int> in CreateInvoiceCommand means that this class is a request that will return an int data type.
    public class CreateInvoiceCommand : IRequest<int>
    {
        public CreateInvoiceCommand()
        {
            this.InvoiceItems = new List<InvoiceItemVm>();
        }
        public string InvoiceNumber { get; set; }
        public string Logo { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public string PaymentTerms { get; set; }
        public DateTime? DueDate { get; set; }
        public double Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public double Tax { get; set; }
        public TaxType TaxType { get; set; }
        public double AmountPaid { get; set; }
        public IList<InvoiceItemVm> InvoiceItems { get; set; }
    }
}
