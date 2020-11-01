using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Lab.Config;

namespace Azoth.Tools.Bootstrap.Lab.Build
{
    internal class Project
    {
        public string Path { get; }
        public FixedList<string> RootNamespace { get; }
        public string Name { get; }
        public FixedList<string> Authors { get; }
        public ProjectTemplate Template { get; }
        public FixedList<ProjectReference> References { get; }

        public Project(
            ProjectConfig file,
            IEnumerable<ProjectReference> references)
        {
            Path = System.IO.Path.GetDirectoryName(file.FullPath) ?? throw new InvalidOperationException("Null directory name");
            Name = file.Name ?? throw new InvalidOperationException();
            RootNamespace = (file.RootNamespace ?? "").SplitOrEmpty('.');
            Authors = (file.Authors ?? throw new InvalidOperationException())
                      .Select(a => a ?? throw new InvalidOperationException()).ToFixedList();
            Template = file.Template ?? throw new InvalidOperationException();
            References = references.ToFixedList();
        }
    }
}
