using iText.Layout;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using Domain.Interfaces;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;

namespace Application.Services
{
    public class KullaniciPdfTumService
    {
        private readonly IKullaniciService _kullaniciService;

        public KullaniciPdfTumService(IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        public async Task<byte[]> GenerateKullaniciPdfTumAsync()
        {
            var kullanicilar = await _kullaniciService.GetKullanicilar();

            var list = kullanicilar.Select(k => new
            {
                k.TC,
                k.AdSoyad,
                k.Tel,
                k.Adres,
                k.Eposta,
                k.KayitTarihi
            })
            .OrderByDescending(x => x.KayitTarihi)
            .ToList();

            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4, false);

            var regularFontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "DejaVuSans.ttf");
            var boldFontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "DejaVuSans-Bold.ttf");

            var regularFont = PdfFontFactory.CreateFont(regularFontPath, PdfEncodings.IDENTITY_H);
            var boldFont = PdfFontFactory.CreateFont(boldFontPath, PdfEncodings.IDENTITY_H);
            document.SetFont(regularFont);

            var headerTable = new Table(UnitValue.CreatePercentArray([1, 3, 1]))
                .UseAllAvailableWidth()
                .SetMarginBottom(10);

            var logoPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "logo.png");
            if (File.Exists(logoPath))
            {
                var logoImage = new Image(ImageDataFactory.Create(logoPath))
                    .ScaleToFit(40, 40)
                    .SetHorizontalAlignment(HorizontalAlignment.LEFT);
                headerTable.AddCell(new Cell().Add(logoImage).SetBorder(Border.NO_BORDER));
            }
            else
            {
                headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
            }

            headerTable.AddCell(new Cell()
                .Add(new Paragraph("TÜM KULLANICILAR RAPORU")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
            document.Add(headerTable);

            var infoTable = new Table(2).UseAllAvailableWidth();
            infoTable.AddCell(new Cell().Add(new Paragraph("Kaynak: Kullanıcı Veritabanı"))
                .SetFontSize(10)
                .SetBorder(Border.NO_BORDER));
            infoTable.AddCell(new Cell().Add(new Paragraph($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}"))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(10)
                .SetBorder(Border.NO_BORDER));
            document.Add(infoTable);

            document.Add(new Paragraph("\n"));

            var table = new Table(UnitValue.CreatePercentArray([2.5f, 3.5f, 2.5f, 3.5f, 2.75f, 3.75f]))
                .SetWidth(UnitValue.CreatePercentValue(100));

            string[] headers = { "TC", "Ad Soyad", "Telefon", "Adres", "E-Posta", "Kayıt Tarihi" };
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(header).SetFont(boldFont))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10)
                    .SetPadding(6));
            }

            bool isGray = false;
            foreach (var k in list)
            {
                var bgColor = isGray ? new DeviceRgb(245, 245, 245) : ColorConstants.WHITE;

                table.AddCell(new Cell().Add(new Paragraph(k.TC)).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(k.AdSoyad)).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(k.Tel)).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(k.Adres)).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(k.Eposta)).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(k.KayitTarihi.ToString("dd.MM.yyyy HH:mm"))).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(9).SetTextAlignment(TextAlignment.CENTER));

                isGray = !isGray;
            }

            document.Add(table);

            int pageCount = pdf.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                var canvas = new PdfCanvas(pdf.GetPage(i));
                var pageSize = pdf.GetPage(i).GetPageSize();

                var footer = new Paragraph($"Sayfa {i} / {pageCount} - Toplam Kayıt: {list.Count}")
                    .SetFont(regularFont)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER);

                new Canvas(canvas, pageSize)
                    .ShowTextAligned(footer, pageSize.GetWidth() / 2, 20, TextAlignment.CENTER);
            }

            document.Close();
            return memoryStream.ToArray();
        }
    }
}
