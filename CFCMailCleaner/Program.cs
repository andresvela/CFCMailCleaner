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
                System.Console.WriteLine("-documentData : DocumentSeparator");
                System.Console.WriteLine("-template : Template Struct Separator");
                System.Console.WriteLine("-emailData : emailData separator");
                System.Console.WriteLine("-duplicate : duplicate data for attachemnt creation");
                return;
            }
            else
            {

                FlatTransofmer cfcTransformer = new FlatTransofmer(commandReader);
                if (cfcTransformer.doTransformation()){
                    cfcTransformer.saveFileTransformation(commandReader.FileName+".new.xml");
                        //System.Console.WriteLine(cfcTransformer.XmlmStringValue);
                }                             
            }
        }
    }
}
