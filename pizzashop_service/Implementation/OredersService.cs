using System.Drawing;
using System.Security.Cryptography;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using pizzashop_repository.Interface;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;
using SelectPdf;



namespace pizzashop_service.Implementation;

public class OredersService : IOredersService
{
    private readonly IOrdersRepository _ordersRepository;

    private readonly IRazorViewToStringRenderer _razorRenderer;

    public OredersService(IOrdersRepository ordersRepository, IRazorViewToStringRenderer razorRenderer)
    {
        _ordersRepository = ordersRepository;
        _razorRenderer = razorRenderer;
    }

    public async Task<PagedResult<OrdersTableViewModel>> GetOrdersAsync(int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm = "", string status = "All", string dateRange = "All time", string fromDate = "", string toDate = "")
    {
        return await _ordersRepository.GetOrdersAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm, status, dateRange, fromDate, toDate);
    }


    public byte[] GenerateExcel(string status, string date, string searchTerm)
    {
        List<OrdersTableViewModel>? orders = _ordersRepository.GetOrders(status, date, searchTerm);

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
                var border = worksheet.Cells[cellRange].Style.Border;
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
            SetBackgroundColor("E9:G9", headerColor);
            SetBackgroundColor("H9:J9", headerColor);
            SetBackgroundColor("K9:L9", headerColor);
            SetBackgroundColor("M9:N9", headerColor);
            SetBackgroundColor("O9:P9", headerColor);



            worksheet.Cells["A9"].Value = "ID";
            worksheet.Cells["A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["B9"].Value = "Order Date";
            worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["E9"].Value = "Customer Name";
            worksheet.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["H9"].Value = "Status";
            worksheet.Cells["H9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["K9"].Value = "Payment Mode";
            worksheet.Cells["K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["M9"].Value = "Rating";
            worksheet.Cells["M9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["O9"].Value = "Total Amount";
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


            worksheet.Cells["A2:B3"].Value = "Status:";
            worksheet.Cells["C2:F3"].Value = string.IsNullOrEmpty(status) || status == "All Status" ? "All Status" : status;
            worksheet.Cells["C2:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C2:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["H2:I3"].Value = "Date:";
            worksheet.Cells["J2:M3"].Value = string.IsNullOrEmpty(date) || date == "AllTime" ? "AllTime" : date;
            worksheet.Cells["J2:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["A5:B6"].Value = "Search Text:";
            worksheet.Cells["C5:F6"].Value = string.IsNullOrEmpty(searchTerm) ? "" : searchTerm;
            worksheet.Cells["C5:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C5:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["H5:I6"].Value = "No. Of Records:";
            worksheet.Cells["J5:M6"].Value = orders.Count;
            worksheet.Cells["J5:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J5:M6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;



            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");
            if (File.Exists(logoPath))
            {
                var logo = worksheet.Drawings.AddPicture("Logo", new FileInfo(logoPath));
                logo.SetPosition(1, 0, 14, 0);
                logo.SetSize(100, 100);
            }


            int row = 10;
            foreach (OrdersTableViewModel? order in orders)
            {

                worksheet.Cells[$"A{row}"].Value = order.Id;
                worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                worksheet.Cells[$"B{row}:D{row}"].Merge = true;
                worksheet.Cells[$"B{row}"].Value = order.OrderDate.ToString("dd-MM-yyyy HH:mm:ss");
                worksheet.Cells[$"B{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                worksheet.Cells[$"E{row}:G{row}"].Merge = true;
                worksheet.Cells[$"E{row}"].Value = order.CustomerName;
                worksheet.Cells[$"E{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"E{row}:G{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"E{row}:G{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"H{row}:J{row}"].Merge = true;
                worksheet.Cells[$"H{row}"].Value = order.Status;
                worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{row}:J{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"H{row}:J{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                worksheet.Cells[$"K{row}:L{row}"].Merge = true;
                worksheet.Cells[$"K{row}"].Value = order.PaymentMethod;
                worksheet.Cells[$"K{row}:L{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"K{row}:L{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"K{row}:L{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"M{row}:N{row}"].Merge = true;
                worksheet.Cells[$"M{row}"].Value = order.Rating;
                worksheet.Cells[$"M{row}:N{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"M{row}:N{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"M{row}:N{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"O{row}:P{row}"].Merge = true;
                worksheet.Cells[$"O{row}"].Value = order.TotalAmount;
                worksheet.Cells[$"O{row}:P{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

    }

    public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int orderId)
    {
        var order = await _ordersRepository.GetOrderWithDetailsAsync(orderId);

        if (order == null)
            return null;

        // Fetch Payment Date where PaymentStatus is true
        DateTime paidOn = order.Payments?.FirstOrDefault(p => p.PaymentStatus)?.Createdat ?? DateTime.MinValue;

        string? paymentMethod = order.Payment != null ? order.Payment.PaymentMethod : "Pending";


        List<OrderItemViewModel>? orderItems = order.OrderItemsMappings.Where(oi => !oi.Isdeleted).Select(oi => new OrderItemViewModel
        {
            ItemName = oi.ItemName,
            Quantity = oi.Quantity,
            Price = oi.Price,
            TotalAmount = oi.TotalPrice,

            ItemModifier = oi.OrderItemModifiers?.Select(m => new OrderItemModifierViewModel
            {
                Name = m.ModifierName,
                Quantity = oi.Quantity,
                Price = m.Price,
                TotalAmount = m.Price * oi.Quantity
            }).ToList() ?? new List<OrderItemModifierViewModel>()

        }).ToList() ?? new List<OrderItemViewModel>();

        decimal subtotal = order.Subamount;

        List<OrderTaxViewModel>? orderTax = order.OrderTaxesMappings
                            .Where(ot => ot.Tax.Isenabled == true)
                            .Select(ot => new OrderTaxViewModel
                            {
                                TaxName = ot.TaxName,
                                TaxAmount = ot.TaxAmount
                            }).ToList() ?? new List<OrderTaxViewModel>();

        List<OrdersTableMapViewModel>? orderTable = order.OrdersTableMappings.Select(ott => new OrdersTableMapViewModel
        {
            TableName = ott.Table.Name,
            SectionName = ott.Table.Section.Name
        }).ToList() ?? new List<OrdersTableMapViewModel>();

        decimal? totalAmount = order.Totalamount;

        TimeSpan orderDuration = order.Updatedat - order.Createdat;

        return new OrderDetailsViewModel
        {
            OrderId = order.Id,
            InvoiceNo = order.Invoices.FirstOrDefault()?.InvoiceNumber,
            Status = order.Status,
            PlacedOn = order.Createdat,
            ModifiedOn = order.Updatedat,
            PaymentMethod = paymentMethod,
            Paidon = paidOn,
            OrderDuration = order.Updatedat - order.Createdat,
            CustomerName = order.Customer.Name,
            CustomerPhone = order.Customer.PhoneNumber,
            CustomerEmail = order.Customer.Email,
            NoOfPerson = order.Customer.NoOfPerson,
            OrderTable = orderTable,
            OrderItems = orderItems,
            OrderTax = orderTax,
            Subtotal = subtotal,
            TotalAmount = totalAmount
        };
    }



    public async Task<byte[]> GenerateInvoicePdfAsync(int orderId)
    {
        OrderDetailsViewModel? model = await GetOrderDetailsAsync(orderId) ?? throw new Exception("Order not found!");

        string htmlContent = await _razorRenderer.RenderViewToStringAsync("Orders/OrderInvoice", model);

        HtmlToPdf? converter = new HtmlToPdf();

        converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;

        PdfDocument doc = converter.ConvertHtmlString(htmlContent);
        byte[] pdfBytes;
        using (MemoryStream? memoryStream = new MemoryStream())
        {
            doc.Save(memoryStream);
            pdfBytes = memoryStream.ToArray();
        }
        doc.Close();

        return pdfBytes;
    }


    public async Task<byte[]> GenerateOrderDetailsPdfAsync(int orderId)
    {
        // Fetch order details
        OrderDetailsViewModel? model = await GetOrderDetailsAsync(orderId);

        if (model == null)
            throw new Exception("Order not found!");
        string htmlContent = await _razorRenderer.RenderViewToStringAsync("Orders/OrderDetailsPdf", model);
        HtmlToPdf? converter = new HtmlToPdf();
        PdfDocument doc = converter.ConvertHtmlString(htmlContent);
        byte[] pdfBytes;
        using (MemoryStream? memoryStream = new MemoryStream())
        {
            doc.Save(memoryStream);
            pdfBytes = memoryStream.ToArray();
        }
        doc.Close();

        return pdfBytes;
    }

    

}







