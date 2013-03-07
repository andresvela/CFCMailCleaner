using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;

namespace CFCMailCleaner
{
    class CommandLineRader
    {
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string documentSeparator;

        public string DocumentSeparator
        {
            get { return documentSeparator; }
            set { documentSeparator = value; }
        }

        private string templateStructSeparator;

        public string TemplateStructSeparator
        {
            get { return templateStructSeparator; }
            set { templateStructSeparator = value; }
        }
        private string emailDataSeparator;

        public string EmailDataSeparator
        {
            get { return emailDataSeparator; }
            set { emailDataSeparator = value; }
        }
        private string duplicateStructSeparator;

        public string DuplicateStructSeparator
        {
            get { return duplicateStructSeparator; }
            set { duplicateStructSeparator = value; }
        }


        public CommandLineRader(string[] args)
        {            
            Arguments CommandLine = new Arguments(args);
            this.fileName = CommandLine["fileName"];
            this.documentSeparator = CommandLine["documentData"];
            this.templateStructSeparator = CommandLine["templateData"];
            this.emailDataSeparator = CommandLine["emailData"];
            this.duplicateStructSeparator = CommandLine["duplicateData"];
        }
        
    }
}
