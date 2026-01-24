using CVEnhancer.DTO;
using CVEnhancer.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QColors = QuestPDF.Helpers.Colors;

namespace CVEnhancer.Services;

public class PdfExportService
{
    private readonly SessionService _session;

    public PdfExportService(SessionService session)
    {
        _session = session;
    }

    public byte[] GenerateCvPdfBytes(List<MatchedItemDTO> selectedItems)
    {
        var user = _session.ActiveUser ?? throw new InvalidOperationException("Brak zalogowanego użytkownika.");

        var work = selectedItems.Where(x => x.Type == "WorkExperience").ToList();
        var proj = selectedItems.Where(x => x.Type == "Project").ToList();
        var cert = selectedItems.Where(x => x.Type == "Certificate").ToList();
        var edu = selectedItems.Where(x => x.Type == "Education").ToList();

        var skills = selectedItems
            .SelectMany(x => x.MatchedSkills ?? new List<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s)
            .ToList();

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                page.Content().Column(col =>
                {
                    // HEADER
                    col.Item().Text($"{user.FirstName} {user.LastName}")
                        .FontSize(20).SemiBold().FontColor(QColors.Black);

                    if (!string.IsNullOrWhiteSpace(user.JobTitle))
                        col.Item().Text(user.JobTitle).FontSize(12).FontColor(QColors.Black);

                    // CONTACTS (VERTICAL) - avoids Row overflow
                    col.Item().PaddingTop(6).Column(c =>
                    {
                        c.Spacing(2);

                        void AddLine(string label, string? value)
                        {
                            if (string.IsNullOrWhiteSpace(value)) return;

                            c.Item().Text(t =>
                            {
                                t.Span(label + ": ").SemiBold().FontSize(10);
                                t.Span(value).FontSize(10);
                            });
                        }

                        AddLine("Email", user.Email);
                        AddLine("Tel", user.PhoneNumber);
                        AddLine("LinkedIn", user.LinkedInUrl);
                        AddLine("GitHub", user.GitHubUrl);
                        AddLine("Portfolio", user.PortfolioUrl);
                    });

                    col.Item().PaddingTop(10);
                    col.Item().LineHorizontal(1).LineColor(QColors.Black);

                    // SUMMARY
                    if (!string.IsNullOrWhiteSpace(user.ProfessionalSummary))
                    {
                        col.Item().PaddingTop(8).Text("Podsumowanie").SemiBold();
                        col.Item().Text(user.ProfessionalSummary).FontColor(QColors.Black);
                    }

                    //SKILLS as bullet list (safe wrapping)
                    if (skills.Count > 0)
                    {
                        col.Item().PaddingTop(10).Text("Skille").SemiBold();
                        col.Item().Column(list =>
                        {
                            list.Spacing(2);
                            foreach (var s in skills)
                                list.Item().Text("• " + s).FontSize(10).FontColor(QColors.Black);
                        });
                    }

                    AddSection(col, "Doświadczenie", work);
                    AddSection(col, "Projekty", proj);
                    AddSection(col, "Certyfikaty", cert);
                    AddSection(col, "Edukacja", edu);
                });
            });
        });

        return doc.GeneratePdf();
    }

    private static void AddSection(ColumnDescriptor col, string title, List<MatchedItemDTO> items)
    {
        if (items.Count == 0) return;

        col.Item().PaddingTop(10).Text(title).SemiBold();
        col.Item().PaddingTop(4).Column(list =>
        {
            list.Spacing(6);

            foreach (var item in items)
            {
                list.Item().Column(x =>
                {
                    x.Spacing(2);
                    x.Item().Text(item.Title).SemiBold().FontColor(QColors.Black);

                    if (!string.IsNullOrWhiteSpace(item.Description))
                        x.Item().Text(item.Description).FontColor(QColors.Black);
                });
            }
        });
    }
}
