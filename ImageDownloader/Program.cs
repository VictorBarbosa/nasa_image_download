// See https://aka.ms/new-console-template for more information
using ImageDownloader;




Console.WriteLine("Hello, World!");

string nasaUrl = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol=$sol&api_key=";
var downloader = new ImageDownloader.ImageDownloader("");


var startOn = downloader?.imageSolLists?.Count > 0 ? downloader.imageSolLists.Max(x => x.SolNumber) + 1 : 0;

downloader.CreateImageDownloadListAsync(nasaUrl, startOn, 4000);
downloader.DownLoadImages();
var total = downloader.imageSolLists.Sum(x => x.TotalImage);
Console.WriteLine($"Total Images {total}");

Console.ReadLine();

