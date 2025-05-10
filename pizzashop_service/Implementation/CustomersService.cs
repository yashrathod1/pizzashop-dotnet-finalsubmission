using System.Drawing;
using System.Security.Cryptography;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class CustomersService : ICustomersService
{
    private readonly ICustomersRepository _customersRepository;

    public CustomersService(ICustomersRepository customersRepository)
    {
        _customersRepository = customersRepository;
    }

    public async Task<PagedResult<CustomerTableViewModel>> GetCustomersAsync(int pageNumber = 1, int pageSize = 5, string sortBy = "Date", string sortOrder = "desc", string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        return await _customersRepository.GetCustomersAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm, dateRange, customStartDate, customEndDate);
    }

    public async Task<CustomerHistoryViewModel> GetCustomerHistoryAsync(int id)
    {
        Customer? customer = await _customersRepository.GetCustomerByIdAsync(id);

        if (customer == null) return null;

        return new CustomerHistoryViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            PhoneNumber = customer.PhoneNumber,
            AverageBill = customer.Orders.Any() ? customer.Orders.Average(o => o.Totalamount) : 0,
            ComingSince = customer.Createdat.ToString("dd-MM-yyyy HH:mm:ss"),
            MaxOrder = customer.Orders.Any() ? customer.Orders.Max(o => o.Totalamount) : 0,
            Visits = customer.Orders
                .Select(o => o.Createdat.Date)
                .Distinct()
                .Count(),

            Orders = customer.Orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.Id,
                Date = o.Createdat.ToString("dd-MM-yyyy"),
                Payment = o.Payment != null ? o.Payment.PaymentMethod : "Pending",
                OrderType = o.Ordertype,
                Items = o.OrderItemsMappings.Count(),
                Amount = o.Totalamount
            }).ToList()
        };
    }


    public async Task<byte[]> GenerateExcel(string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        List<CustomerTableViewModel>? customers = await _customersRepository.GetCustomerAsync(searchTerm, dateRange, customStartDate, customEndDate);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Orders");


            Color headerColor = ColorTranslator.FromHtml("#0568A8");
            Color borderColor = Color.Black; // Black border

            // Function to Set Background Color
            void SetBackgroundColor(string cellRange, Color color)
            {
                worksheet.Cells[cellRange].Merge = true;
                worksheet.Cells[cellRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[cellRange].Style.Fill.BackgroundColor.SetColor(color);
                worksheet.Cells[cellRange].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[cellRange].Style.Font.Bold = true;
                worksheet.Cells[cellRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[cellRange].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            void SetBorder(string cellRange)
            {
                worksheet.Cells[cellRange].Merge = true;
                Border? border = worksheet.Cells[cellRange].Style.Border;
                border.Top.Style = ExcelBorderStyle.Thin;
                border.Bottom.Style = ExcelBorderStyle.Thin;
                border.Left.Style = ExcelBorderStyle.Thin;
                border.Right.Style = ExcelBorderStyle.Thin;
                border.Top.Color.SetColor(borderColor);
                border.Bottom.Color.SetColor(borderColor);
                border.Left.Color.SetColor(borderColor);
                border.Right.Color.SetColor(borderColor);
            }


            SetBackgroundColor("A2:B3", headerColor);
            SetBorder("C2:F3");

            SetBackgroundColor("H2:I3", headerColor);
            SetBorder("J2:M3");

            SetBackgroundColor("A5:B6", headerColor);
            SetBorder("C5:F6");

            SetBackgroundColor("H5:I6", headerColor);
            SetBorder("J5:M6");


            SetBackgroundColor("A9", headerColor);
            SetBackgroundColor("B9:D9", headerColor);
            SetBackgroundColor("E9:H9", headerColor);
            SetBackgroundColor("I9:K9", headerColor);
            SetBackgroundColor("L9:N9", headerColor);
            SetBackgroundColor("O9:P9", headerColor);



            worksheet.Cells["A9"].Value = "ID";
            worksheet.Cells["A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["B9"].Value = "Name";
            worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["E9"].Value = "Email";
            worksheet.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["I9"].Value = "Date";
            worksheet.Cells["I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["L9"].Value = "Mobile Number";
            worksheet.Cells["L9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["O9"].Value = "Total Order";
            worksheet.Cells["O9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 20;
            worksheet.Column(6).Width = 20;
            worksheet.Column(7).Width = 20;
            worksheet.Column(8).Width = 15;
            worksheet.Column(9).Width = 15;
            worksheet.Column(10).Width = 15;
            worksheet.Column(11).Width = 15;
            worksheet.Column(12).Width = 15;
            worksheet.Column(15).Width = 15;
            worksheet.Column(16).Width = 15;


            worksheet.Cells["A2:B3"].Value = "Account:";
            worksheet.Cells["C2:F3"].Value ="Admin";
            worksheet.Cells["C2:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C2:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["H2:I3"].Value = "Search Text:";
            worksheet.Cells["J2:M3"].Value = string.IsNullOrEmpty(searchTerm) ? "" : searchTerm; 
            worksheet.Cells["J2:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["A5:B6"].Value = "Date:";
            worksheet.Cells["C5:F6"].Value = $"{customStartDate:dd-MM-yyyy} to {customEndDate:dd-MM-yyyy}";
            worksheet.Cells["C5:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C5:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["H5:I6"].Value = "No. Of Records:";
            worksheet.Cells["J5:M6"].Value = customers.Count;
            worksheet.Cells["J5:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J5:M6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;



            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");
            if (File.Exists(logoPath))
            {
                OfficeOpenXml.Drawing.ExcelPicture? logo = worksheet.Drawings.AddPicture("Logo", new FileInfo(logoPath));
                logo.SetPosition(1, 0, 14, 0);
                logo.SetSize(100, 100);
            }


            int row = 10;
            foreach (CustomerTableViewModel? customer in customers)
            {

                worksheet.Cells[$"A{row}"].Value = customer.Id;
                worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                worksheet.Cells[$"B{row}:D{row}"].Merge = true;
                worksheet.Cells[$"B{row}"].Value = customer.Name ;
                worksheet.Cells[$"B{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                worksheet.Cells[$"E{row}:H{row}"].Merge = true;
                worksheet.Cells[$"E{row}"].Value = customer.Email;
                worksheet.Cells[$"E{row}:H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"E{row}:H{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"E{row}:H{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"I{row}:K{row}"].Merge = true;
                worksheet.Cells[$"I{row}"].Value = customer.Date.ToString("dd-MM-yyyy");
                worksheet.Cells[$"I{row}:K{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"I{row}:K{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"I{row}:K{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"L{row}:N{row}"].Merge = true;
                worksheet.Cells[$"L{row}"].Value = customer.PhoNo;
                worksheet.Cells[$"L{row}:N{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"L{row}:N{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"L{row}:N{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"O{row}:P{row}"].Merge = true;
                worksheet.Cells[$"O{row}"].Value = customer.TotalOrder;
                worksheet.Cells[$"O{row}:P{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

    }

}