using System.IO;
using Azoth.Tools.Bootstrap.IL.IO;

namespace Azoth.Tools.Bootstrap.IL.Assembly
{
    public class Disassembler
    {
        public string Disassemble(Stream inputStream)
        {
            var reader = new ILReader();
            var package = reader.Read(inputStream);
            return Disassemble(package);
        }

        public string Disassemble(PackageIL package)
        {
            throw new System.NotImplementedException();
        }
    }
}
