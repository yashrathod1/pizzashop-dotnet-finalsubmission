using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Order
{
    public int Id { get; set; }

    public int Customerid { get; set; }

    public string Status { get; set; } = null!;

    public string? Comment { get; set; }

    public int? Paymentid { get; set; }

    public decimal Subamount { get; set; }

    public decimal Totalamount { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public string Ordertype { get; set; } = null!;

    public DateTime? Servingtime { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; } = new List<Feedback>();

    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();

    public virtual ICollection<OrderItemsMapping> OrderItemsMappings { get; } = new List<OrderItemsMapping>();

    public virtual ICollection<OrderTaxesMapping> OrderTaxesMappings { get; } = new List<OrderTaxesMapping>();

    public virtual ICollection<OrdersTableMapping> OrdersTableMappings { get; } = new List<OrdersTableMapping>();

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Payment> Payments { get; } = new List<Payment>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
