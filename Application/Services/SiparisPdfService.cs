using MediatR;
using iText.Layout;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using Domain.Interfaces;
using Application.Query;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;

namespace Application.Services
{
    public class SiparisPdfService
    {
        private readonly IMediator _mediator;
        private readonly IKuryeService _kuryeService;
        private readonly IKullaniciService _kullaniciService;

        public SiparisPdfService(IKuryeService kuryeService, IKullaniciService kullaniciService, IMediator mediator)
        {
            _mediator = mediator;
            _kuryeService = kuryeService;
            _kullaniciService = kullaniciService;
        }

        public async Task<byte[]> GenerateSiparisPdfAsync(int id)
        {
            var siparis = await _kuryeService.GetKuryeById(id);
            if (siparis == null) return null!;

            var kullanici = await _kullaniciService.GetKullaniciByTC(siparis.KullaniciTC);
            var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(siparis.KuryeciTC));

            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
            document.SetMargins(30, 30, 30, 30);

            string regularFontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "DejaVuSans.ttf");
            string boldFontPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "DejaVuSans-Bold.ttf");

            PdfFont regularFont = PdfFontFactory.CreateFont(regularFontPath, PdfEncodings.IDENTITY_H);
            PdfFont boldFont = PdfFontFactory.CreateFont(boldFontPath, PdfEncodings.IDENTITY_H);
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
                .Add(new Paragraph("SİPARİŞ RAPORU")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
            document.Add(headerTable);

            var infoTable = new Table(2).UseAllAvailableWidth();
            infoTable.AddCell(new Cell().Add(new Paragraph("Kaynak: Sipariş Veritabanı"))
                .SetFontSize(10)
                .SetBorder(Border.NO_BORDER));
            infoTable.AddCell(new Cell().Add(new Paragraph($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}"))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(10)
                .SetBorder(Border.NO_BORDER));
            document.Add(infoTable);

            document.Add(new Paragraph("\n"));

            var table = new Table(UnitValue.CreatePercentArray([3f, 7f]))
                .SetWidth(UnitValue.CreatePercentValue(80))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            void AddRow(string label, string value)
            {
                table.AddCell(new Cell().Add(new Paragraph(label).SetFont(boldFont)).SetPadding(5));
                table.AddCell(new Cell().Add(new Paragraph(value ?? "------").SetFont(regularFont)).SetPadding(5));
            }

            AddRow("Sipariş ID:", siparis.Id.ToString());
            AddRow("Kullanıcı:", kullanici?.AdSoyad ?? "Bilinmiyor");
            AddRow("Kuryeci:", kuryeci?.AdSoyad ?? "Bilinmiyor");
            AddRow("Ürün:", siparis.UrunAdi);

            if (siparis.Durum == "Aktif")
            {
                AddRow("Durum:", "Kuryeci Tarafından Kabul Edildi");
                AddRow("Teslim Durumu:", siparis.TeslimDurumu);
                AddRow("Sipariş Tarihi:", siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"));

                if (siparis.TeslimDurumu == "Teslim Edilmedi")
                    AddRow("Teslim Tarihi:", "------");
                else if (siparis.TeslimDurumu == "Teslim Edildi")
                    AddRow("Teslim Tarihi:", siparis.TeslimTarihi.ToString("dd.MM.yyyy HH:mm") ?? "------");
            }
            else if (siparis.Durum == "Pasif")
            {
                AddRow("Durum:", "Kuryeci Tarafından Henüz Kabul Edilmedi");
                AddRow("Teslim Durumu:", "------");
                AddRow("Sipariş Tarihi:", siparis.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"));
                AddRow("Teslim Tarihi:", "------");
            }

            document.Add(table);

            int pageCount = pdf.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                var canvas = new PdfCanvas(pdf.GetPage(i));
                var pageSize = pdf.GetPage(i).GetPageSize();

                var footer = new Paragraph($"Sayfa {i} / {pageCount}")
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
