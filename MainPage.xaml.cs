using Reader.Azure;
using System.IO;
using System.Threading;


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
                    var smsBody = ocrResult.ToString();
                    await Clipboard.Default.SetTextAsync(smsBody);
                    await DisplayAlert("Siker", smsBody, "Üzenet másolása");                   
                }
                else
                {
                    await DisplayAlert("Rossz kép", "Nem sikerült leolvasni a számokat.", "OK");
                }
            }
        }


    }
}

