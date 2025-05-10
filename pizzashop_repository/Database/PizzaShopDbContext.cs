using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Models;

namespace pizzashop_repository.Database;

public partial class PizzaShopDbContext : DbContext
{
    public PizzaShopDbContext(DbContextOptions<PizzaShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<MappingMenuItemWithModifier> MappingMenuItemWithModifiers { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<Modifiergroup> Modifiergroups { get; set; }

    public virtual DbSet<Modifiergroupmodifier> Modifiergroupmodifiers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItemModifier> OrderItemModifiers { get; set; }

    public virtual DbSet<OrderItemsMapping> OrderItemsMappings { get; set; }

    public virtual DbSet<OrderTaxesMapping> OrderTaxesMappings { get; set; }

    public virtual DbSet<OrdersTableMapping> OrdersTableMappings { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Roleandpermission> Roleandpermissions { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<Taxesandfee> Taxesandfees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WaitingToken> WaitingTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby)
                .HasMaxLength(50)
                .HasColumnName("createdby");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Updateat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateat");
            entity.Property(e => e.Updatedby)
                .HasMaxLength(50)
                .HasColumnName("updatedby");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cityname)
                .HasMaxLength(100)
                .HasColumnName("cityname");
            entity.Property(e => e.Stateid).HasColumnName("stateid");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.Stateid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("cities_stateid_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Countryname)
                .HasMaxLength(100)
                .HasColumnName("countryname");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.HasIndex(e => e.Email, "customer_email_key").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "customer_phone_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NoOfPerson).HasColumnName("no_of_person");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.CustomerCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("customer_createdby_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.CustomerUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("customer_updatedby_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ambience).HasColumnName("ambience");
            entity.Property(e => e.Avgrating).HasColumnName("avgrating");
            entity.Property(e => e.Comments)
                .HasMaxLength(200)
                .HasColumnName("comments");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Food).HasColumnName("food");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Service).HasColumnName("service");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.FeedbackCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("feedback_createdby_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_id");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.FeedbackUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("feedback_updatedby_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoice_pkey");

            entity.ToTable("invoice");

            entity.HasIndex(e => e.InvoiceNumber, "invoice_invoice_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(30)
                .HasColumnName("invoice_number");
            entity.Property(e => e.Orderid).HasColumnName("orderid");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("invoice_createdby_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("invoice_orderid_fkey");
        });

        modelBuilder.Entity<MappingMenuItemWithModifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mapping_menu_item_with_modifiers_pkey");

            entity.ToTable("mapping_menu_item_with_modifiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.MaxModifierCount).HasColumnName("max_modifier_count");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.MinModifierCount).HasColumnName("min_modifier_count");
            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.MappingMenuItemWithModifierCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("mapping_menu_item_with_modifiers_createdby_fkey");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.MappingMenuItemWithModifiers)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("menu_item_id");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.MappingMenuItemWithModifiers)
                .HasForeignKey(d => d.ModifierGroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("mapping_menu_item_with_modifiers_modifier_group_id_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.MappingMenuItemWithModifierUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("mapping_menu_item_with_modifiers_updatedby_fkey");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_items_pkey");

            entity.ToTable("menu_items");

            entity.HasIndex(e => e.Name, "menu_items_name_key").IsUnique();

            entity.HasIndex(e => e.ShortCode, "menu_items_short_code_key").IsUnique();

            entity.HasIndex(e => e.Name, "unique_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsAvailable).HasColumnName("is_available");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsdefaultTax).HasColumnName("isdefault_tax");
            entity.Property(e => e.Isfavourite).HasColumnName("isfavourite");
            entity.Property(e => e.ItemImage)
                .HasMaxLength(500)
                .HasColumnName("item_image");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.ShortCode)
                .HasMaxLength(15)
                .HasColumnName("short_code");
            entity.Property(e => e.TaxPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("tax_percentage");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
            entity.Property(e => e.UnitType)
                .HasMaxLength(20)
                .HasColumnName("unit_type");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("menu_items_categoryid_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MenuItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("menu_items_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MenuItemUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("menu_items_updated_by_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifier_pkey");

            entity.ToTable("modifier");

            entity.HasIndex(e => e.Name, "modifier_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Unittype)
                .HasMaxLength(5)
                .HasColumnName("unittype");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.ModifierCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("modifier_createdby_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.ModifierUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("modifier_updatedby_fkey");
        });

        modelBuilder.Entity<Modifiergroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifiergroup_pkey");

            entity.ToTable("modifiergroup");

            entity.HasIndex(e => e.Name, "modifiergroup_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.ModifiergroupCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("modifiergroup_createdby_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.ModifiergroupUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("modifiergroup_updatedby_fkey");
        });

        modelBuilder.Entity<Modifiergroupmodifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifiergroupmodifier_pkey");

            entity.ToTable("modifiergroupmodifier");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");
            entity.Property(e => e.Modifierid).HasColumnName("modifierid");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Modifiergroupmodifiers)
                .HasForeignKey(d => d.Modifiergroupid)
                .HasConstraintName("modifiergroupmodifier_modifiergroupid_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.Modifiergroupmodifiers)
                .HasForeignKey(d => d.Modifierid)
                .HasConstraintName("modifiergroupmodifier_modifierid_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pkey");

            entity.ToTable("order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Ordertype)
                .HasMaxLength(50)
                .HasDefaultValueSql("'DineIn'::character varying")
                .HasColumnName("ordertype");
            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Servingtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("servingtime");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");
            entity.Property(e => e.Subamount)
                .HasPrecision(10, 2)
                .HasColumnName("subamount");
            entity.Property(e => e.Totalamount)
                .HasPrecision(10, 2)
                .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.OrderCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_createdby_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("order_customerid_fkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Paymentid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_paymentid");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.OrderUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_updatedby_fkey");
        });

        modelBuilder.Entity<OrderItemModifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_item_modifiers_pkey");

            entity.ToTable("order_item_modifiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ModifierName)
                .HasMaxLength(100)
                .HasColumnName("modifier_name");
            entity.Property(e => e.Modifierid).HasColumnName("modifierid");
            entity.Property(e => e.Orderitemid).HasColumnName("orderitemid");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Modifier).WithMany(p => p.OrderItemModifiers)
                .HasForeignKey(d => d.Modifierid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_item_modifiers_modifierid_fkey");

            entity.HasOne(d => d.Orderitem).WithMany(p => p.OrderItemModifiers)
                .HasForeignKey(d => d.Orderitemid)
                .HasConstraintName("order_item_modifiers_orderitemid_fkey");
        });

        modelBuilder.Entity<OrderItemsMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_items_mapping_pkey");

            entity.ToTable("order_items_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .HasColumnName("item_name");
            entity.Property(e => e.Menuitemid).HasColumnName("menuitemid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Preparedquantity)
                .HasDefaultValueSql("0")
                .HasColumnName("preparedquantity");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.Menuitem).WithMany(p => p.OrderItemsMappings)
                .HasForeignKey(d => d.Menuitemid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_items_mapping_menuitemid_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItemsMappings)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("order_items_mapping_orderid_fkey");
        });

        modelBuilder.Entity<OrderTaxesMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_taxes_mapping_pkey");

            entity.ToTable("order_taxes_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("tax_amount");
            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.TaxName)
                .HasMaxLength(50)
                .HasColumnName("tax_name");
            entity.Property(e => e.TaxValue)
                .HasPrecision(5, 2)
                .HasColumnName("tax_value");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxesMappings)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_taxes_mapping_order_id_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxesMappings)
                .HasForeignKey(d => d.TaxId)
                .HasConstraintName("order_taxes_mapping_tax_id_fkey");
        });

        modelBuilder.Entity<OrdersTableMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_table_mapping_pkey");

            entity.ToTable("orders_table_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.NoOfPerson).HasColumnName("no_of_person");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Tableid).HasColumnName("tableid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.OrdersTableMappingCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_table_mapping_createdby_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersTableMappings)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("orders_table_mapping_orderid_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.OrdersTableMappings)
                .HasForeignKey(d => d.Tableid)
                .HasConstraintName("orders_table_mapping_tableid_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.OrdersTableMappingUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_table_mapping_updatedby_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(15)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus).HasColumnName("payment_status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.PaymentCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_createdby_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("payment_orderid_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.PaymentUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_updatedby_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permission_pkey");

            entity.ToTable("permission");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Permissiomname)
                .HasMaxLength(200)
                .HasColumnName("permissiomname");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Rolename, "roles_rolename_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Roleandpermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roleandpermission_pkey");

            entity.ToTable("roleandpermission");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanaddEdit).HasColumnName("canadd_edit");
            entity.Property(e => e.Candelete).HasColumnName("candelete");
            entity.Property(e => e.Canview).HasColumnName("canview");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby)
                .HasMaxLength(50)
                .HasColumnName("createdby");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Updateat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateat");
            entity.Property(e => e.Updatedby)
                .HasMaxLength(50)
                .HasColumnName("updatedby");

            entity.HasOne(d => d.Permission).WithMany(p => p.Roleandpermissions)
                .HasForeignKey(d => d.Permissionid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("roleandpermission_permissionid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Roleandpermissions)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("roleandpermission_roleid_fkey");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sections_pkey");

            entity.ToTable("sections");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.SectionCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sections_createdby_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.SectionUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sections_updatedby_fkey");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("states_pkey");

            entity.ToTable("states");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Statename)
                .HasMaxLength(100)
                .HasColumnName("statename");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("states_countryid_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Available'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.TableCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tables_createdby_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.Sectionid)
                .HasConstraintName("tables_sectionid_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.TableUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tables_updatedby_fkey");
        });

        modelBuilder.Entity<Taxesandfee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("taxesandfees_pkey");

            entity.ToTable("taxesandfees");

            entity.HasIndex(e => e.Name, "taxesandfees_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Isdefault).HasColumnName("isdefault");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled).HasColumnName("isenabled");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");
            entity.Property(e => e.Value)
                .HasPrecision(5, 2)
                .HasColumnName("value");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.TaxesandfeeCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("taxesandfees_createdby_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.TaxesandfeeUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("taxesandfees_updatedby_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby)
                .HasMaxLength(50)
                .HasColumnName("createdby");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.PasswordResetToken).HasColumnType("character varying");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Profileimagepath)
                .HasMaxLength(300)
                .HasColumnName("profileimagepath");
            entity.Property(e => e.Resettokenexpiry).HasColumnName("resettokenexpiry");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updateat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateat");
            entity.Property(e => e.Updatedby)
                .HasMaxLength(50)
                .HasColumnName("updatedby");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(50)
                .HasColumnName("zipcode");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.Cityid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_users_city");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_users_country");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("users_roleid_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.Stateid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_users_state");
        });

        modelBuilder.Entity<WaitingToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("waiting_tokens_pkey");

            entity.ToTable("waiting_tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsAssign).HasColumnName("is_assign");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.NoOfPersons).HasColumnName("no_of_persons");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.WaitingTokenCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("waiting_tokens_createdby_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_tokens_customer_id_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_tokens_section_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("waiting_tokens_table_id_fkey");

            entity.HasOne(d => d.UpdatedbyNavigation).WithMany(p => p.WaitingTokenUpdatedbyNavigations)
                .HasForeignKey(d => d.Updatedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("waiting_tokens_updatedby_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
