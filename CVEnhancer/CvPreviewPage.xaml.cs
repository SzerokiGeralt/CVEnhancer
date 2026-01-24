using CVEnhancer.DTO;
using CVEnhancer.Services;
using CVEnhancer.ViewModels;

namespace CVEnhancer;

public partial class CvPreviewPage : ContentPage
{
    public CvPreviewPage(SessionService session, PdfExportService pdf, List<MatchedItemDTO> selected)
    {
        InitializeComponent();
        BindingContext = new CvPreviewViewModel(session, pdf, selected);
    }
}
