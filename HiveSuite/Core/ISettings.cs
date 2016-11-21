using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    public interface ISettings
    {
        string DefaultPath { get; }
        void GenerateConfig();
        void Load(string filePath);
        void Save(string filePath);

    }
}
