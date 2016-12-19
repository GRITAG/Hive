using HiveSuite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSuite.Queen
{
    public class QueenSettings : ISettings
    {
        ISettings Settings { get; set; }

        public string DefaultFilePath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DefaultPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int NetworkTimeout
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Port
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public QueenSettings()
        {
        }

        public void GenerateConfig()
        {
            throw new NotImplementedException();
        }

        public void Load(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Save(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
