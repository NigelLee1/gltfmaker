using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker.Models
{
    class Comb
    {
        public string id { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        /// <summary>
        /// ground_lev
        /// </summary>
        public float gl { get; set; }
        /// <summary>
        /// invert_lev
        /// </summary>
        public float il { get; set; }
        /// <summary>
        /// x方向的长度
        /// </summary>
        public float xl { get; set; }
        /// <summary>
        /// y方向的长度
        /// </summary>
        public float yl { get; set; }
    }
}
