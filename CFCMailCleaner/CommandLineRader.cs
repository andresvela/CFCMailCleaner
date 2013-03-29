using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;
using System.IO;

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

        private string templatesData;

        public string TemplatesData
        {
            get { return templatesData; }
            set { templatesData = value; }
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


        private string batchWork;

        public  string BatchWork
        {
	        get { return batchWork;}
	        set { batchWork = value;}
        }

        private string fileOut;

        public string FileOut
        {
            get { return fileOut; }
            set { fileOut = value; }
        }


        private String[] templateValues;

        public int templateSize
        {
            get { return templateValues.Length; }
           
        }

        public string templateMail
        {
            get { return templateValues[0]; }
        }

        public string templateAttachment
        {
            get { return templateValues[1]; }
        }

        public CommandLineRader(string[] args)
        {            
            Arguments CommandLine = new Arguments(args);
            this.fileName = CommandLine["fileName"];
            this.documentSeparator = CommandLine["documentData"];            
            this.emailDataSeparator = CommandLine["emailData"];
            this.duplicateStructSeparator = CommandLine["duplicateData"];
            this.batchWork = CommandLine["batchWork"];
            this.fileOut = CommandLine["fileOut"];

            //if no model name needed
            this.templatesData = CommandLine["templateData"];
            if (templatesData != null)
            {
                templateValues = templatesData.Split('|');
            }
            else { 
                string tempfile = Path.GetTempFileName();
                String[] partsFile = this.fileName.Split('.');
                string extension = partsFile[partsFile.Length - 3];
                this.templatesData = extension;
                templateValues = this.templatesData.Split('-');

            }
           
        }
        


    }
}
