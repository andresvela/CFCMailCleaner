using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace CFCMailCleaner
{
    class FlatTransofmer
    {    
        
        private FileInfo fi ;
        private StreamReader reader ;
        private char[] delimiterChars = { '#' };
        private string line;
        private string headerLine;       
        private string[] emailData;
        private string[] templateData;
        private int lineHeaderInit = 0;
        private int lineHeaderEnd = 0;
        private bool lineHeaderIN = false;
        private int lineIndex = 0;
        private List<string[]> documentLines = new List<string[]>();
        private CommandLineRader commandReader;
        private MemoryStream sw;
        private XmlWriter writer;
        private Encoding encoding;


        private string xmlmStringValue;
        public string XmlmStringValue
        {
            get { return xmlmStringValue; }            
        }

        private string guidString;
        private Guid guid;

        public FlatTransofmer(CommandLineRader commandReaderArg)
        {
            encoding = Encoding.GetEncoding("ISO-8859-1");
            try
            {
                this.commandReader = commandReaderArg;
                this.fi = new FileInfo(this.commandReader.FileName);               
            }
            catch (Exception ex) {
               System.Console.WriteLine(ex.Message);
            }
        }
     
        public string saveFileTransformation(string outPutFileName)
        {
            
            File.WriteAllText(outPutFileName, encoding.GetString(sw.ToArray()), encoding);
            return null;
        }

        private bool createModeleNode(string template) {                                    
                this.writer.WriteElementString("MODELE", template);
                return true;     
        }

        private bool createTypeNode(string type)
        {
            this.writer.WriteElementString("TYPE", type);
            return true;
        }
       
        private bool createGuIdNode(string value)
        {
            this.writer.WriteElementString("ID_UNIQUE", value);
            return true;
        }

       

        private bool createEmailNode(string[] templateData)
        {
            int i = 0;
            writer.WriteStartElement("EMAILDATA");
            foreach (string emailD in emailData)
            {
                if (i != 0 && i != 1 && i != emailData.Length - 1)
                    writer.WriteElementString("EMAIL_" + (i - 1), emailData[i]);
                i++;
            }
            writer.WriteEndElement();
            return true;
        }

        private bool createCDATANode(int lineHeaderIni, int lineHeaderEn)
        {
            writer.WriteStartElement("FLATDATA");
            writer.WriteCData("\r\n" + string.Join("\r\n", System.IO.File.ReadLines(this.commandReader.FileName, encoding).Skip(lineHeaderIni - 1).Take(lineHeaderEn - lineHeaderIni + 1).ToArray()) + "\r\n");
            writer.WriteEndElement();
            return true;
        }

        private string getHeader (string infoLine){
            string finalHeader=null;
            try
            {
                finalHeader = infoLine.Substring(1, infoLine.IndexOf("#", 2) - 1);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("getHeader ERROR:" + ex.Message);
            }
            return finalHeader;
        }

        private bool getXMLBody() {

            while ((line = reader.ReadLine()) != null)
            {
                lineIndex++;
                //read header line      
                //add try catch
                headerLine = getHeader(line);


                //getSeparator
                if (this.commandReader.DocumentSeparator == null)
                    this.commandReader.DocumentSeparator = headerLine;

                //documentSeparator traitement
                if (headerLine == this.commandReader.DocumentSeparator)
                {
                    //detect header document
                    if (lineHeaderIN == false)
                    {

                        lineHeaderInit = lineIndex;
                        lineHeaderIN = true;
                        writer.WriteStartElement("BDOCEDITDATA");
                        createModeleNode(this.commandReader.templateMail);
                    }

                    //detect end document                                
                    else
                    {

                        lineHeaderEnd = lineIndex - 1;
                        createCDATANode(lineHeaderInit, lineHeaderEnd);
                        writer.WriteEndElement();

                        //write duplicate if needed
                        if (this.commandReader.templateSize>1)
                        {
                            writer.WriteStartElement("BDOCEDITDATA");
                            createModeleNode(this.commandReader.templateAttachment);
                            
                            if (this.commandReader.EmailDataSeparator != null)
                            {
                                createGuIdNode(guidString);
                                createTypeNode("ATTACHMENT");
                                createEmailNode(emailData);
                            }
                           
                            createCDATANode(lineHeaderInit, lineHeaderEnd);
                            writer.WriteEndElement();
                        }
                        //new Header
                        lineHeaderInit = lineIndex;
                        writer.WriteStartElement("BDOCEDITDATA");
                        createModeleNode(this.commandReader.templateMail);
                    }
                }               

                
                //email data Treatment
                if (this.commandReader.EmailDataSeparator != null && headerLine == this.commandReader.EmailDataSeparator)
                {
                    createTypeNode("EMAIL");
                    this.guid = Guid.NewGuid();
                    this.guidString = guid.ToString();
                    createGuIdNode(guidString);
                    emailData = line.Split(delimiterChars);
                    createEmailNode(emailData);
                }
            }

            //detect end document       
            lineHeaderEnd = lineIndex--;
            createCDATANode(lineHeaderInit, lineHeaderEnd);
            writer.WriteEndElement();
            //write duplicate if needed
            if (this.commandReader.templateSize > 1)
            {
                writer.WriteStartElement("BDOCEDITDATA");
                createModeleNode(this.commandReader.templateAttachment);
                if (this.commandReader.EmailDataSeparator != null)
                {
                    createGuIdNode(guidString);
                    createTypeNode("ATTACHMENT");
                    createEmailNode(emailData);
                }
                createCDATANode(lineHeaderInit, lineHeaderEnd);
                writer.WriteEndElement();
            }

            return true;
        
        }

        public bool doTransformation()
        {
            this.reader = new StreamReader(this.commandReader.FileName, encoding, true);
            using (sw = new MemoryStream())
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;


                using (writer = System.Xml.XmlWriter.Create(sw, xmlWriterSettings))
                {                                       
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ROOT");

                    getXMLBody();
                    
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    this.xmlmStringValue = encoding.GetString(sw.ToArray());
                }
            }
            return true;
        }

        public  bool addTemplateType(string fileOut)
        {
            string tempfile = Path.GetTempFileName();
            StreamWriter writer = new StreamWriter(tempfile);
            StreamReader reader = new StreamReader(this.commandReader.FileName, encoding);
            String[] partsFile = this.commandReader.FileName.Split('.');
            string extension = partsFile[partsFile.Length - 2];
            //Path.GetExtension(this.commandReader.FileName).Substring(1)
            writer.WriteLine("BDOCEDITMODELE#" + extension);
            while (!reader.EndOfStream)
                writer.WriteLine(reader.ReadLine());
            writer.Close();
            reader.Close();

            File.Copy(tempfile, fileOut, true);
            return true;
        }
    }
}
