using Camera.MAUI;
using Reader.Azure;


namespace Reader;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                //string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                using (Stream sourceStream = await photo.OpenReadAsync())
                {
                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                    sourceStream.CopyToAsync(localFileStream).Wait();
                }


                var azureVision = new ComputerVisionHandler();
                var ocrResult = azureVision.CallOcr(localFilePath);

                if (ocrResult.IsValid)
                {
                    // send message
                    DisplayAlert("Siker", $"Drága: {ocrResult.HighCounter}; Olcsó: {ocrResult.LowCounter}", "OK");
                }
                else
                {
                    DisplayAlert("Rossz kép", "Nem sikerült leolvasni a számokat.", "OK");
                }
            }
        }


    }
}

