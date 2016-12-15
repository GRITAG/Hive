using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Core
{
    /// <summary>
    /// Simple settings interface
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// The default file path for the config file
        /// </summary>
        string DefaultPath { get; }
        
        /// <summary>
        /// Creates and stores a config with in the file system
        /// </summary>
        void GenerateConfig();
        
        /// <summary>
        /// Loads the file at the path given
        /// </summary>
        /// <param name="filePath"></param>
        void Load(string filePath);

        /// <summary>
        /// Saves a file at the path given
        /// </summary>
        /// <param name="filePath"></param>
        void Save(string filePath);

        int Port { get; set; }

        int NetworkTimeout { get; set; }

    }
}
