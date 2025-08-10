using MediatR;
using iText.Layout;
using iText.IO.Font;
using iText.IO.Image;
using Application.DTOs;
using iText.Kernel.Pdf;
using Application.Query;
using Domain.Interfaces;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;

namespace Application.Services
{
    public class SiparisPdfTumService
    {
        private readonly IMediator _mediator;
        private readonly IKuryeService _kuryeService;
        private readonly IKullaniciService _kullaniciService;

        public SiparisPdfTumService(IKuryeService kuryeService, IKullaniciService kullaniciService, IMediator mediator)
        {
            _mediator = mediator;
            _kuryeService = kuryeService;
            _kullaniciService = kullaniciService;
        }

        public async Task<byte[]> GenerateSiparisPdfTumAsync()
        {
            var dtoList = new List<SiparislerAdminListDto>();

            var siparisler = await _kuryeService.GetKuryeler();
            var kullanicilar = await _kullaniciService.GetKullanicilar();
            var kuryeciler = await _mediator.Send(new GetAllKuryecilerQuery());

            foreach (var siparis in siparisler)
            {
                var kuryeci = kuryeciler.FirstOrDefault(u => u.TC == siparis.KuryeciTC);
                var kullanici = kullanicilar.FirstOrDefault(u => u.TC == siparis.KullaniciTC);
                dtoList.Add(new SiparislerAdminListDto
                {
                    SiparisId = siparis.Id,
                    KullaniciAdSoyad = kullanici?.AdSoyad ?? "Bilinmiyor",
                    KuryeciAdSoyad = kuryeci?.AdSoyad ?? "Bilinmiyor",
                    UrunAdi = siparis.UrunAdi,
                    Durum = siparis.Durum,
                    TeslimDurumu = siparis.TeslimDurumu,
                    SiparisTarihi = siparis.SiparisTarihi,
                    TeslimTarihi = siparis.TeslimTarihi
                });
            }
            var list = dtoList.OrderBy(x => x.SiparisId).ToList();

            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4, false);
            document.SetMargins(30, 30, 30, 30);

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
                var logoImage = new iText.Layout.Element.Image(ImageDataFactory.Create(logoPath))
                    .ScaleToFit(40, 40)
                    .SetHorizontalAlignment(HorizontalAlignment.LEFT);
                headerTable.AddCell(new Cell().Add(logoImage).SetBorder(Border.NO_BORDER));
            }
            else
            {
                headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
            }

            headerTable.AddCell(new Cell()
                .Add(new Paragraph("TÜM SİPARİŞLER RAPORU")
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

            var table = new Table(UnitValue.CreatePercentArray([2f, 4f, 3.5f, 4f, 4.2f, 3.7f, 3f, 3f]))
                .SetWidth(UnitValue.CreatePercentValue(95))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            string[] headers = { "Sipariş ID", "Kullanıcı", "Kuryeci", "Ürün", "Durum", "Teslim Durumu", "Sipariş Tarihi", "Teslim Tarihi" };
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(header).SetFont(boldFont))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(8)
                    .SetPadding(6));
            }

            bool isGray = false;
            foreach (var s in list)
            {
                var bgColor = isGray ? new DeviceRgb(245, 245, 245) : ColorConstants.WHITE;

                table.AddCell(new Cell().Add(new Paragraph(s.SiparisId.ToString())).SetBackgroundColor(bgColor).SetPadding(5).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(s.KullaniciAdSoyad)).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                table.AddCell(new Cell().Add(new Paragraph(s.KuryeciAdSoyad)).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                table.AddCell(new Cell().Add(new Paragraph(s.UrunAdi)).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));

                if (s.Durum == "Aktif")
                {
                    table.AddCell(new Cell().Add(new Paragraph("Kuryeci Tarafından Kabul Edildi")).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                    table.AddCell(new Cell().Add(new Paragraph(s.TeslimDurumu)).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                    table.AddCell(new Cell().Add(new Paragraph(s.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"))).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));

                    if (s.TeslimDurumu == "Teslim Edilmedi")
                        table.AddCell(new Cell().Add(new Paragraph("------")).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                    else if (s.TeslimDurumu == "Teslim Edildi")
                        table.AddCell(new Cell().Add(new Paragraph(s.TeslimTarihi.ToString("dd.MM.yyyy HH:mm"))).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                }
                else if (s.Durum == "Pasif")
                {
                    table.AddCell(new Cell().Add(new Paragraph("Kuryeci Tarafından Henüz Kabul Edilmedi")).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                    table.AddCell(new Cell().Add(new Paragraph("------")).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                    table.AddCell(new Cell().Add(new Paragraph(s.SiparisTarihi.ToString("dd.MM.yyyy HH:mm"))).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                    table.AddCell(new Cell().Add(new Paragraph("------")).SetBackgroundColor(bgColor).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER).SetPadding(5));
                }
                isGray = !isGray;
            }

            document.Add(table);

            int pageCount = pdf.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                var canvas = new PdfCanvas(pdf.GetPage(i));
                var pageSize = pdf.GetPage(i).GetPageSize();

                var footer = new Paragraph($"Sayfa {i} / {pageCount} - Toplam Sipariş: {list.Count}")
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

