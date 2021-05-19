using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfWorld.Utils
{
    internal class EmptyWriter : TextWriter
    {
        public override void WriteLine()
        {

        }

        public override void WriteLine(string value)
        {

        }

        public override void WriteLine(object value)
        {

        }

        public override Encoding Encoding => Encoding.ASCII;
    }

}
