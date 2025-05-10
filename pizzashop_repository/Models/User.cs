using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Status { get; set; }

    public bool Isdeleted { get; set; }

    public int Roleid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updateat { get; set; }

    public string Createdby { get; set; } = null!;

    public string? Updatedby { get; set; }

    public string? Profileimagepath { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? Resettokenexpiry { get; set; }

    public int? Countryid { get; set; }

    public int? Stateid { get; set; }

    public int? Cityid { get; set; }

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<Customer> CustomerCreatedbyNavigations { get; } = new List<Customer>();

    public virtual ICollection<Customer> CustomerUpdatedbyNavigations { get; } = new List<Customer>();

    public virtual ICollection<Feedback> FeedbackCreatedbyNavigations { get; } = new List<Feedback>();

    public virtual ICollection<Feedback> FeedbackUpdatedbyNavigations { get; } = new List<Feedback>();

    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();

    public virtual ICollection<MappingMenuItemWithModifier> MappingMenuItemWithModifierCreatedbyNavigations { get; } = new List<MappingMenuItemWithModifier>();

    public virtual ICollection<MappingMenuItemWithModifier> MappingMenuItemWithModifierUpdatedbyNavigations { get; } = new List<MappingMenuItemWithModifier>();

    public virtual ICollection<MenuItem> MenuItemCreatedByNavigations { get; } = new List<MenuItem>();

    public virtual ICollection<MenuItem> MenuItemUpdatedByNavigations { get; } = new List<MenuItem>();

    public virtual ICollection<Modifier> ModifierCreatedbyNavigations { get; } = new List<Modifier>();

    public virtual ICollection<Modifier> ModifierUpdatedbyNavigations { get; } = new List<Modifier>();

    public virtual ICollection<Modifiergroup> ModifiergroupCreatedbyNavigations { get; } = new List<Modifiergroup>();

    public virtual ICollection<Modifiergroup> ModifiergroupUpdatedbyNavigations { get; } = new List<Modifiergroup>();

    public virtual ICollection<Order> OrderCreatedbyNavigations { get; } = new List<Order>();

    public virtual ICollection<Order> OrderUpdatedbyNavigations { get; } = new List<Order>();

    public virtual ICollection<OrdersTableMapping> OrdersTableMappingCreatedbyNavigations { get; } = new List<OrdersTableMapping>();

    public virtual ICollection<OrdersTableMapping> OrdersTableMappingUpdatedbyNavigations { get; } = new List<OrdersTableMapping>();

    public virtual ICollection<Payment> PaymentCreatedbyNavigations { get; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentUpdatedbyNavigations { get; } = new List<Payment>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Section> SectionCreatedbyNavigations { get; } = new List<Section>();

    public virtual ICollection<Section> SectionUpdatedbyNavigations { get; } = new List<Section>();

    public virtual State? State { get; set; }

    public virtual ICollection<Table> TableCreatedbyNavigations { get; } = new List<Table>();

    public virtual ICollection<Table> TableUpdatedbyNavigations { get; } = new List<Table>();

    public virtual ICollection<Taxesandfee> TaxesandfeeCreatedbyNavigations { get; } = new List<Taxesandfee>();

    public virtual ICollection<Taxesandfee> TaxesandfeeUpdatedbyNavigations { get; } = new List<Taxesandfee>();

    public virtual ICollection<WaitingToken> WaitingTokenCreatedbyNavigations { get; } = new List<WaitingToken>();

    public virtual ICollection<WaitingToken> WaitingTokenUpdatedbyNavigations { get; } = new List<WaitingToken>();
}
