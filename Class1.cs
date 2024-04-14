using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

class Program
{
    public static string? AudioFileName;
    public static string? BirdName;
    public static List<string> BirdDetails = new();
    public static string? Img;

    internal static void Predict()
    {
        using (Process process = new Process())
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "python";

            // Get the current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Set the arguments for the Python script
            startInfo.Arguments = $"predictor.py \"{AudioFileName}\" \"{currentDirectory}\"";


            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;

            // Set the working directory if needed
            startInfo.WorkingDirectory = currentDirectory;

            process.StartInfo = startInfo; // Set the start info before starting the process

            // Start the process
            process.Start();

            // Wait for the process to exit
            process.WaitForExit();

            // Read the output/error
            string output = process.StandardOutput.ReadToEnd();



            // Process the output
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                BirdName = lines[lines.Length - 1];
            }
            else
            {
                Console.WriteLine("No output received from the Python script.");
            }

        }
    }



    internal static void getBridDetails()
    {
        string jsonFilePath = "species_data.json";

        // Read JSON content from file
        string jsonString = File.ReadAllText(jsonFilePath);

        // Deserialize JSON to a Dictionary<string, object>
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

        // Get value based on key
        if (dictionary.ContainsKey(BirdName))
        {
            JArray jsonArray = (JArray)dictionary[BirdName];

            // Convert JArray to List<string>
            BirdDetails = jsonArray.Select(token => (string)token).ToList();
        }
    }


    internal static BitmapImage getBridImage(string BridKey)
    {
        string Dir = "./images/";
        string[] imageFiles = Directory.GetFiles(Dir);
        string foundImagePath = imageFiles.FirstOrDefault(imagePath =>
                Path.GetFileNameWithoutExtension(imagePath).Equals(BridKey, StringComparison.OrdinalIgnoreCase));
        BitmapImage bitmap = new BitmapImage(new Uri(foundImagePath, UriKind.RelativeOrAbsolute));
        return bitmap;
    }
}