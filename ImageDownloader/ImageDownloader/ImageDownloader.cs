using ImageDownloader.Model;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
namespace ImageDownloader
{
    public class ImageDownloader
    {
        private readonly string NASA_KEY = "z7OIxy7yJExInU6TjPDcKqdz5zEa85tmHfjbZ8ic";
        private readonly string current_path = "";
        private readonly string json_log = "";
        public List<ImageSolList> imageSolLists = new List<ImageSolList>();


        public ImageDownloader(string nasaKey)
        {

            current_path = Directory.GetCurrentDirectory();
            current_path = current_path.Replace("\\", "/").Replace(@"/ImageDownloader/bin/Debug/net7.0", "");
            json_log = Path.Combine(current_path, "log.json");


            if (!File.Exists(json_log))
            {
                File.Create(json_log);
            }
            else
            {

                var json = File.ReadAllText(json_log);
                imageSolLists = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImageSolList>>(json);
                if (imageSolLists == null)
                {
                    imageSolLists = new List<ImageSolList>();
                }

            }

#if !DEBUG
            NASA_KEY = nasaKey;
#endif
        }


        public void CreateImageDownloadListAsync(string nasaURL, int solStart = 1000, int solEnd = 1000)
        {
            if (solStart > solEnd)
            {
                return;
            }


            string newNasaURL = nasaURL + NASA_KEY;

            newNasaURL = nasaURL + NASA_KEY;
            newNasaURL = newNasaURL.Replace("$sol", solStart.ToString());
            HttpClient http = new HttpClient();
            http.Timeout = Timeout.InfiniteTimeSpan;
            var result = http.GetStringAsync(newNasaURL);
            result.Wait();
            if (result.Result != null)
            {
                var nasaImage = Newtonsoft.Json.JsonConvert.DeserializeObject<NasaImage>(result.Result);
                List<ImageSol>? images = nasaImage?.photos?.Select(x => new ImageSol
                {
                    ImageUrl = x.img_src,
                    ImageName = x.id.ToString()
                }).ToList();

                imageSolLists.Add(new ImageSolList()
                {
                    SolNumber = solStart,
                    Images = images,
                    TotalImage = images.Count

                });
                Console.WriteLine($"SOL:{solStart}\nTotal Found {images.Count};");
            }
            else
            {
                imageSolLists.Add(new ImageSolList()
                {
                    SolNumber = solStart,
                    TotalImage = 0

                });

            }


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(imageSolLists);
            var file = File.Open(json_log, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter stream = new StreamWriter(file);
            stream.WriteLine(json.ToString());
            stream.Close();
            if (solEnd >= solStart)
            {
                CreateImageDownloadListAsync(nasaURL, solStart + 1, solEnd);
            }
        }

        public void DownLoadImages()
        {
            try
            {
                string downloadPath = Path.Combine(current_path, "Images");
                imageSolLists.Where(x => x.TotalImage > 0)?.ToList()?.ForEach(f =>
                {
                    Console.Clear();
                    Console.WriteLine($"Creating images folder SOL{f.SolNumber}");
                    string points = "..";
                    int count = 1;
                    f.Images.ForEach(async img =>
                    {
                        Console.Clear();
                        Console.WriteLine($"Creating images folder SOL{f.SolNumber} Total Images - {count} - {points}");
                        points += ".";
                        count++;
                        HttpClient clientImage = new HttpClient();
                        clientImage.Timeout = TimeSpan.FromHours(1);
                        string p = $"{downloadPath}\\Sol-{f.SolNumber}";

                        if (!Directory.Exists(p))
                        {
                            Directory.CreateDirectory(p);
                        }

                        string solPath = Path.Combine($"Sol-{f.SolNumber}", $"{img.ImageName}.JPG");
                        string path = Path.Combine(downloadPath, solPath);
                        if (!File.Exists(path))
                        {
                            var uri = new Uri(img.ImageUrl);
                            var imgByte = clientImage.GetByteArrayAsync(uri);
                            imgByte.Wait();
                            File.WriteAllBytesAsync(path, imgByte.Result).Wait();
                        }

                    });

                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro {ex.Message} ");
            }

        }

    }
}