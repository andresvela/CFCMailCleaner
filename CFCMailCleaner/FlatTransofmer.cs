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


        private string xmlmStringValue;
        public string XmlmStringValue
        {
            get { return xmlmStringValue; }            
        }


        public FlatTransofmer(CommandLineRader commandReaderArg)
        {
            try
            {
                this.commandReader = commandReaderArg;
                this.fi = new FileInfo(this.commandReader.FileName);
                this.reader = new StreamReader(this.commandReader.FileName, Encoding.GetEncoding("iso-8859-1"), true);
            }
            catch (Exception ex) {
               System.Console.WriteLine(ex.Message);
            }
        }
     
        public string saveFileTransformation(string outPutFileName)
        {
            File.WriteAllText(outPutFileName , Encoding.UTF8.GetString(sw.ToArray()));
            return null;
        }

        private bool createModeleNode(string[] templateData, int index) {                                    
                this.writer.WriteElementString("MODELE", templateData[index]);
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
            writer.WriteCData("\r\n" + string.Join("\r\n", System.IO.File.ReadLines(this.commandReader.FileName, Encoding.GetEncoding("iso-8859-1")).Skip(lineHeaderIni - 1).Take(lineHeaderEn - lineHeaderIni + 1).ToArray()) + "\r\n");
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

                //documentSeparator traitement
                if (headerLine == this.commandReader.DocumentSeparator)
                {
                    //detect header document
                    if (lineHeaderIN == false)
                    {

                        lineHeaderInit = lineIndex;
                        lineHeaderIN = true;
                        writer.WriteStartElement("BDOCEDITDATA");
                    }

                    //detect end document                                
                    else
                    {

                        lineHeaderEnd = lineIndex - 1;
                        createCDATANode(lineHeaderInit, lineHeaderEnd);
                        writer.WriteEndElement();

                        //write ducplicate if needed
                        if (templateData.Length > 4)
                        {
                            writer.WriteStartElement("BDOCEDITDATA");
                            createModeleNode(templateData, 3);
                            createEmailNode(emailData);
                            createCDATANode(lineHeaderInit, lineHeaderEnd);
                            writer.WriteEndElement();
                        }
                        //new Header
                        lineHeaderInit = lineIndex;
                        writer.WriteStartElement("BDOCEDITDATA");
                    }

                }

                //template data Treatment
                if (headerLine == this.commandReader.TemplateStructSeparator)
                {
                    templateData = line.Split(delimiterChars);
                    createModeleNode(templateData, 2);
                }

                //email data Treatment
                if (headerLine == this.commandReader.EmailDataSeparator)
                {
                    emailData = line.Split(delimiterChars);
                    createEmailNode(emailData);
                }
            }

            //detect end document       
            lineHeaderEnd = lineIndex--;
            createCDATANode(lineHeaderInit, lineHeaderEnd);
            return true;
        
        }

        public bool doTransformation()
        {
            using (sw = new MemoryStream())
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;


                using (writer = System.Xml.XmlWriter.Create(sw, xmlWriterSettings))
                {                                       
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ROOT");

                    getXMLBody();
                    
                    writer.WriteEndElement();
                    writer.WriteEndDocument();                     
                    this.xmlmStringValue = Encoding.UTF8.GetString(sw.ToArray());
                }
            }
            return true;
        }
    }
}
