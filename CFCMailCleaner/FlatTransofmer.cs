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

        public bool doTransformation()
        {
            using (sw = new MemoryStream())
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;


                using (var writer = System.Xml.XmlWriter.Create(sw, xmlWriterSettings))
                {
                    // Build Xml with xw.                    
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ROOT");


                    while ((line = reader.ReadLine()) != null)
                    {
                        lineIndex++;
                        //read header line      
                        //add try catch
                        headerLine = line.Substring(1, line.IndexOf("#", 2) - 1);

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
                                //lineHeaderIN = false;
                                lineHeaderEnd = lineIndex-1;

                                writer.WriteCData("\r\n" + string.Join("\r\n", System.IO.File.ReadLines(this.commandReader.FileName, Encoding.GetEncoding("iso-8859-1")).Skip(lineHeaderInit - 1).Take(lineHeaderEnd - lineHeaderInit+1).ToArray()) + "\r\n");
                                writer.WriteEndElement();
                                //new Header
                                lineHeaderInit = lineIndex;
                                writer.WriteStartElement("BDOCEDITDATA");
                            }
                                 
                        }

                        //template data Treatment
                        if (headerLine == this.commandReader.TemplateStructSeparator)
                        {
                            templateData = line.Split(delimiterChars);
                            writer.WriteElementString("MODELE", templateData[2]);
                        }

                        //email data Treatment
                        if (headerLine == this.commandReader.EmailDataSeparator)
                        {
                            emailData = line.Split(delimiterChars);
                            writer.WriteElementString("EMAIL", emailData[2]);
                        }

                        if (headerLine == this.commandReader.DuplicateStructSeparator)
                        {
                            //duplicate data Treatment
                        }
                        //this.xmlmStringValue = Encoding.UTF8.GetString(sw.ToArray());
                    }

                    //detect end document       
                    lineHeaderEnd = lineIndex--;
                    //print info between lineHeaderInit and lineHeaderEnd
                   // writer.WriteStartElement("FLATDATA");
                    writer.WriteCData("\r\n" + string.Join("\r\n", System.IO.File.ReadLines(this.commandReader.FileName, Encoding.GetEncoding("iso-8859-1")).Skip(lineHeaderInit).Take(lineHeaderEnd - 1).ToArray()) + "\r\n");
                   // writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                     
                    this.xmlmStringValue = Encoding.UTF8.GetString(sw.ToArray());

                }
            }
            return true;
        }
    }
}
