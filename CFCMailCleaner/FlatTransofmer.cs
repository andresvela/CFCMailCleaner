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
        private int lineHeaderCount = 0;
        private int lineIndex = 0;
        private List<string[]> documentLines = new List<string[]>();
        private CommandLineRader commandReader;
        private MemoryStream sw;

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
        public string saveFileTransformation()
        {
            File.WriteAllText(this.commandReader.FileName + ".new.xml", Encoding.UTF8.GetString(sw.ToArray()));
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
                            if (lineHeaderCount == 0)
                            {
                                lineHeaderInit = lineIndex;
                                lineHeaderCount++;

                                writer.WriteStartElement("BDOCEDITDATA");


                            }
                            //detect end document
                            else
                            {

                                lineHeaderCount = 0;
                                lineHeaderEnd = lineIndex--;
                                //print info between lineHeaderInit and lineHeaderEnd
                                //documentLines.Add(System.IO.File.ReadLines(this.commandReader.FileName, Encoding.GetEncoding("iso-8859-1")).Skip(lineHeaderInit - 1).Take(lineHeaderEnd - 1).ToArray());
                                writer.WriteCData("\r\n" + string.Join("\r\n", System.IO.File.ReadLines(this.commandReader.FileName, Encoding.GetEncoding("iso-8859-1")).Skip(lineHeaderInit - 1).Take(lineHeaderEnd - 1).ToArray()) + "\r\n");
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


                        /* Match m = reg.Match(line, 0);
                         if (m.Success)
                         {
                             //int myInt = int.Parse(m.Group(1).Value);
                             //string path = m.Group(2).Value;

                             // At this point, `myInteger` and `path` contain the values we want
                             // for the current line. We can then store those values or print them,
                             // or anything else we like.
                         }*/
                    }

                    //detect end document                      
                    lineHeaderCount = 0;
                    lineHeaderEnd = lineIndex--;
                    //print info between lineHeaderInit and lineHeaderEnd
                    // documentLines.Add(System.IO.File.ReadLines(CommandLine["file"]).Skip(lineHeaderInit).Take(lineHeaderEnd - 1).ToArray());
                    //System.Console.WriteLine(documentLines);
                    writer.WriteCData("\r\n" + string.Join("\r\n", System.IO.File.ReadLines(this.commandReader.FileName).Skip(lineHeaderInit).Take(lineHeaderEnd - 1).ToArray()) + "\r\n");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                }
            }
            return true;
        }
    }
}
