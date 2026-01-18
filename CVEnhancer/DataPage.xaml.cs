namespace CVEnhancer;

public partial class DataPage : ContentPage
{
	public DataPage()
	{
		InitializeComponent();
		ShowPersonalData();
	}

	private void OnPersonalDataClicked(object sender, EventArgs e)
	{
		ShowPersonalData();
	}

	private void OnWorkClicked(object sender, EventArgs e)
	{
		HideAllContent();
		WorkContent.IsVisible = true;
	}

	private void OnEducationClicked(object sender, EventArgs e)
	{
		HideAllContent();
		EducationContent.IsVisible = true;
	}

	private void OnProjectsClicked(object sender, EventArgs e)
	{
		HideAllContent();
		ProjectsContent.IsVisible = true;
	}

	private void ShowPersonalData()
	{
		HideAllContent();
		PersonalDataContent.IsVisible = true;
	}

	private void HideAllContent()
	{
		PersonalDataContent.IsVisible = false;
		WorkContent.IsVisible = false;
		EducationContent.IsVisible = false;
		ProjectsContent.IsVisible = false;
	}
}