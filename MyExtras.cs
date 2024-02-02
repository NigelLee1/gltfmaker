using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    class MyExtras : Extras
    {
        public string data { get; set; }
        public override string ToString()
        {
            return data.ToString();
        }
    }
}
