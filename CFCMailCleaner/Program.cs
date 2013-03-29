using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;
using System.Text.RegularExpressions;
using System.Xml;


namespace CFCMailCleaner
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CommandLineRader commandReader = new CommandLineRader(args);

            if (commandReader.FileName == null)
            {
                System.Console.WriteLine("CFCMailCleaner Parameters: .");
                System.Console.WriteLine("-file: DatFile.");
                //System.Console.WriteLine("-documentData : DocumentSeparator");
                //System.Console.WriteLine("-template : Template Struct Separator");
                System.Console.WriteLine("-emailData : emailData separator");
                System.Console.WriteLine("-fileOut : fileOut name");
                //System.Console.WriteLine("-duplicate : duplicate data for attachemnt creation");
                //System.Console.WriteLine("-batchWork : =TRUE if bathcWork, not to be employed if emails");
                return;
            }
            else if (commandReader.BatchWork == "TRUE") {
                FlatTransofmer cfcTransformer = new FlatTransofmer(commandReader);
                cfcTransformer.addTemplateType(commandReader.FileOut);
            }
            else
            {

                FlatTransofmer cfcTransformer = new FlatTransofmer(commandReader);
                if (cfcTransformer.doTransformation())
                {
                    cfcTransformer.saveFileTransformation(commandReader.FileOut);
                    //System.Console.WriteLine(cfcTransformer.XmlmStringValue);
                }
            }
        }
    }
}
