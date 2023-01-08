using ImageDownloader.Model;
using System.Net;
using System.Net.Http;
namespace ImageDownloader
{
    public class ImageDownloader
    {
        private readonly string NASA_KEY = "eDFG0FGqgWwFTufa7wRPEiQvpnDd7vLKhfeykvvg";
        Thread thread;

        public ImageDownloader(string nasaKey)
        {

#if !DEBUG
            NASA_KEY = nasaKey;
#endif
        }

        public async void DownLoadImagesAsync(string nasaURL, string downloadPath, int solStart = 1000, int solEnd = 1000)
        {


            try
            {

                string newNasaURL = nasaURL + NASA_KEY;
                newNasaURL = newNasaURL.Replace("$sol", solStart.ToString());
                HttpClient http = new HttpClient();
                http.Timeout = Timeout.InfiniteTimeSpan;
                var result = await http.GetStringAsync(newNasaURL);

                if (result != null)
                {
                    var nasaImage = Newtonsoft.Json.JsonConvert.DeserializeObject<NasaImage>(result);

                    nasaImage?.photos.ForEach(async img =>
                    {

                        HttpClient clientImage = new HttpClient();
                        clientImage.Timeout = TimeSpan.FromHours(1);
                        string p = $"{downloadPath}\\Sol-{solStart}";
                        if (!Directory.Exists(p))
                        {
                            Directory.CreateDirectory(p);
                        }

                        string solPath = Path.Combine($"Sol-{solStart}", $"{img.id}.JPG");
                        string path = Path.Combine(downloadPath, solPath);
                        var uri = new Uri(img.img_src);

                        var imgByte = await clientImage.GetByteArrayAsync(uri);

                        await File.WriteAllBytesAsync(path, imgByte);


                    });

                    if (solEnd >= solStart)
                    {
                        DownLoadImagesAsync(nasaURL, downloadPath, solStart + 1, solEnd);
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Erro {nasaURL} ");
            }




        }

    }
}