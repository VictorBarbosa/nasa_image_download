// See https://aka.ms/new-console-template for more information
using ImageDownloader;

Console.WriteLine("Hello, World!");

string nasaUrl = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol=$sol&api_key=";
var downloader = new ImageDownloader.ImageDownloader("");
downloader.DownLoadImagesAsync(nasaUrl, "C:\\ImageNasa", 0, 5000);



Console.ReadLine();
