using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Reader.Logic;

namespace Reader.Azure
{
    internal class ComputerVisionHandler
    {
        // Add your Computer Vision key and endpoint
        private static string key = "efaaa0a5cce0432daf48120b94f5206f";
        private static string endpoint = "https://eps-processor.cognitiveservices.azure.com";
 
        public EnergyMeter CallOcr(string imagePath)
        {
            // Create a client
            ComputerVisionClient client = Authenticate(endpoint, key);

            // Analyze an image to get features and other properties.
            var stat = AnalyzeImageUrl(client, imagePath);
            return stat;
        }


        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
            return client;
        }

        private EnergyMeter AnalyzeImageUrl(ComputerVisionClient client, string imagePath)
        {
            using FileStream fs = File.OpenRead(imagePath);

            // Read text from URL
            var textHeaders = client.ReadInStreamAsync(fs).Result;
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            do
            {
                results = client.GetReadResultAsync(Guid.Parse(operationId)).Result;
            }
            while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

            var stat = new EnergyMeter();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    stat.AddReading(line.Text);

                    if (stat.IsValid)
                        break;
                }
            }

            return stat;
        }
    }
}
