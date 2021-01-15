using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VAIISemka.Models
{
    [DataContract]
    public class Image
    {
        public int Id { get; set; }

        [DataMember(Name = "imageName")]
        public string ImageName { get; set; }

        [DataMember(Name = "imageBytes")]
        public byte[] ImageBytes { get; set; }
    }
}
