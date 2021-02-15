using System.Collections.Generic;
using System.IO;
using Azoth.Tools.Bootstrap.IL;
using Azoth.Tools.Bootstrap.IL.IO;

namespace Azoth.Tools.Bootstrap.Interpreter
{
    public class AzothInterpreter
    {
        private readonly List<PackageIL> packages = new List<PackageIL>();

        public void LoadPackage(Stream inputStream)
        {
            var reader = new ILReader();
            var package = reader.Read(inputStream);
            // TODO validate package references
            packages.Add(package);
        }

        public InterpreterProcess Execute()
        {
            return new InterpreterProcess();
        }
    }
}
