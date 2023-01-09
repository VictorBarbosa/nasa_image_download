using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Model
{
    public class ImageSolList
    {
        public int SolNumber { get; set; }
        public List<ImageSol> Images  = new List<ImageSol>();
        public int TotalImage { get; set; }
    }

    public class ImageSol
    {
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }   
    }
}
