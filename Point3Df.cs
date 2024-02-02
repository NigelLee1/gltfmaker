using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    class Point3Df
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;

        public Point3Df(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public byte[] ToBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(x));
            list.AddRange(BitConverter.GetBytes(y));
            list.AddRange(BitConverter.GetBytes(z));
            return list.ToArray();
        }

        public List<byte> ToByteList()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(x));
            list.AddRange(BitConverter.GetBytes(y));
            list.AddRange(BitConverter.GetBytes(z));
            return list;
        }

        public override string ToString()
        {
            return "x:" + x.ToString("G9") + ",y:" + y.ToString("G9") + ",z:" + z.ToString("G9");
        }
    }
}
