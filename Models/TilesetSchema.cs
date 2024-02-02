using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker.Models
{
    class TilesetSchema
    {
        public int zoom { get; set; }
        /// <summary>
        /// 单位高德经纬度 xmin,ymin,xmax,ymax
        /// </summary>
        public double[] region { get; set; }
        public string gltfUri { get; set; }
        public string[] childrenUri { get; set; }
    }
}
