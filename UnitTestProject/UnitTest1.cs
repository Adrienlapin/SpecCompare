using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public static void Main()
        {
            try
            {

                Console.WriteLine("hello world !");

                string InjectionLastResult = @"D:\eData_PII\MigrationReporting\REPORT_AUGUST\Copy of Result Migration tool 2_DoubleLigne_Customer Param_24072023.txt";
                string ReportLocation = @"D:\eData_PII\MigrationReporting\REPORT_AUGUST\Result-Customer_Param-29082023-105529.csv";
                string ReportLocation2 = @"D:\eData_PII\MigrationReporting\REPORT_AUGUST\Result-CustomerParam-29082023-105529.csv";

                //On test l'alimentation du StringBuilder

                //test du fichier d'entré
                using (StreamReader reader = new StreamReader(ReportLocation, System.Text.Encoding.Default))
                {
                    string line;

                    while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        
                           string[] split = line.Split(';');  
                           string key = split[0] + split[1] + split[7] + split[5] + split[14] + split[21];  
                     
                    }



                    List<String> DicKey = new List<String>();
                    List<String> DicKey2 = new List<String>();

                    /*
                var CsvOut = new StringBuilder();

                using (StreamReader reader = new StreamReader(ReportLocation, System.Text.Encoding.Default))
                {
                    string line;

                    while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        string[] split = line.Split(';');
                        string key = split[0] + split[1] + split[7] + split[5] + split[14] + split[21];

                        using (StreamReader fileLastResult = new StreamReader(InjectionLastResult, System.Text.Encoding.Default))
                        {
                            string ligne;
                            while ((ligne = fileLastResult.ReadLine()) != null)
                            {                                
                                string[] cell = ligne.Split(';');

                                try { 


                                if ((split[0] == cell[0]) && (split[1] == cell[1]) && (split[7] == cell[7]) && (split[5] == cell[5]) && (split[14] == cell[14]) && (split[21] == cell[21]) && !DicKey2.Contains(key))
                                {
                                    string Add = string.Format(";{0};{1};{2};{3}", cell[22], cell[23], cell[24], cell[25]);
                                    line = line + Add;
                                    DicKey2.Add(key);
                                }
                                else if ((split[0] == cell[0]) && (split[1] == cell[1]) && (split[7] == cell[7]) && (split[5] == cell[5]) && (split[14] == cell[14]) && !DicKey2.Contains(key))
                                {
                                    line = line + ";;;;";
                                    DicKey2.Add(key);
                                }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(split[1] + " " + split[7]);
                                }
                                
                            }
                        }    
                        if(!DicKey.Contains(key))
                        {
                            DicKey.Add(key);
                            CsvOut.AppendLine(line);
                        }

                    }
                }

                DicKey.Clear();

                // Write result in file with Date
                using (StreamWriter outputFile = new StreamWriter(ReportLocation2))
                {
                    outputFile.WriteLine(CsvOut.ToString());
                }

                //delete first ReportFinal 
                //File.Delete(ReportLocation);

*/
                    /*

                                    using (StreamReader reader = new StreamReader(ReportLocation))
                                {
                                    string line;

                                    while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                                    {

                                        string[] split = line.Split(';');

                                        using (StreamReader fileLastResult = new StreamReader(InjectionLastResult, System.Text.Encoding.Default))
                                        {
                                            string ligne;

                                            while ((ligne = fileLastResult.ReadLine()) != null)
                                            {
                                                string[] cell = ligne.Split(';');
                                                    if ((split[0] == cell[0]) && (split[6] == cell[6]) && (split[4] == cell[4]) && (split[13] == cell[13]) && (split[20] == cell[20]))
                                                {

                                                            string Add = string.Format(";{0};{1};{2};{3}", cell[21], cell[22], cell[23], cell[24]);
                                                            line = line + Add;

                                                }                                                       
                                                }
                                                //Reinjection des resultats
                                               if (!DicKey.Contains(line))
                                                {
                                                    DicKey.Add(line);
                                                    CsvOut.AppendLine(line);
                                                }

                                            }


                                    }

                                }


                                    //string ReportFinalLocation = ReportLocation + "_final.txt";
                                    File.WriteAllText(ReportLocation, CsvOut.ToString());

                                    */

                    /*
                                    using (StreamReader file = new StreamReader(ReportLocation, System.Text.Encoding.Default))
                                    {
                                        string ln;
                                        while ((ln = file.ReadLine()) != null)
                                        {
                                            string[] cellOne = ln.Split(';');

                                            using (StreamReader fileLastResult = new StreamReader(InjectionLastResult, System.Text.Encoding.Default))
                                    {
                                        string ligne;

                                        while ((ligne = fileLastResult.ReadLine()) != null)
                                        {
                                            string[] cell = ligne.Split(';');

                                                    if ((cellOne[0] == cell[0]) && (cellOne[6] == cell[6]) && (cellOne[4] == cell[4]) && (cellOne[13] == cell[13]) && (cellOne[20] == cell[20]))
                                                    {
                                                        Console.WriteLine("J'ai trouvé la correspondance ! -> " + cellOne[0] + " " + cell[6] + " " + cell[20]);

                                                        //Ajout des 4 colonnes


                                                        //Add in Result file the CP not correct and a message to describe the difference
                                                        var newLine = string.Format("{0};{1};{2};{3};{4}", ln, cell[21], cell[22], cell[23], cell[24]);
                                                        csv.AppendLine(newLine);
                                                        csv.Remove



                                                    }
                                                }

                                            }
                                        }
                                    }


                            */
                    /*

                        // Vocabulary :
                        // PC = Part Commercial & QG = GG Parameter
                        // 1 Part commercial contain a list of QG Parameter

                        //Start
                        var csv = new StringBuilder();

                        //Serialized CSV FILE in QGParamFile
                        //Add data from file to object
                        QGParamFile MigrationQGFile = SerializeQGFile(MigrationFile);
                        QGParamFile ProdQGFile = SerializeQGFile(ProdFile);

                        //At this step we have two object contains all data from Prod file and Migration file.
                        bool PCFound = false;
                        bool QGFound = false;

                        //add Header in Result file
                        WriteCsvError("", ProdQGFile.QGConfigs[0].QGParams[0], csv);

                        //Start treatment with 3 level of control
                        foreach (QGConfigs PCprod in ProdQGFile.QGConfigs)
                        {
                            PCFound = false;
                            //Search PC in migration file
                            foreach (QGConfigs PCmigration in MigrationQGFile.QGConfigs)
                            {
                                if (PCprod.PC_CODEARTICLE == PCmigration.PC_CODEARTICLE)
                                {
                                    // PC found in migration
                                    PCFound = true;
                                }
                            }
                            if (!PCFound)
                            {
                                //PC not found !
                                WriteCsvError("PC not found !", PCprod.QGParams[0], csv);
                            }
                            else
                            {
                                //PC found, then search all QG for this PC
                                foreach (QGParam QGProd in PCprod.QGParams)
                                {
                                    QGFound = false;
                                    foreach (QGConfigs PCmigration in MigrationQGFile.QGConfigs)
                                    {
                                        if (PCprod.PC_CODEARTICLE == PCmigration.PC_CODEARTICLE)
                                        {
                                            foreach (QGParam QGMig in PCmigration.QGParams)
                                            {
                                                //NB a QG is define by "SOI Name + Type_Calcul"
                                                if (QGProd.SOIPARAM_NAME == QGMig.SOIPARAM_NAME && QGProd.QGPARAM_TYPECALCUL == QGMig.QGPARAM_TYPECALCUL)
                                                {
                                                    //QG found in migration
                                                    QGFound = true;
                                                }
                                            }
                                        }
                                    }
                                    if (!QGFound)
                                    {
                                        //QG not Found !
                                        WriteCsvError("QG not found ! (Type_Calcul can be different)", QGProd, csv);
                                    }
                                    else
                                    {
                                        foreach (QGConfigs PCmigration in MigrationQGFile.QGConfigs)
                                        {
                                            if (PCprod.PC_CODEARTICLE == PCmigration.PC_CODEARTICLE)
                                            {
                                                foreach (QGParam QGMig in PCmigration.QGParams)
                                                {
                                                    //CP found : search difference between value =>
                                                    if (QGProd.SOIPARAM_NAME == QGMig.SOIPARAM_NAME && QGProd.QGPARAM_TYPECALCUL == QGMig.QGPARAM_TYPECALCUL)
                                                    {
                                                        if (QGProd.CUSTOMER_NAME != QGMig.CUSTOMER_NAME) { WriteCsvError("Le champ CUSTOMER_NAME est different", QGProd, csv); }

                                                        if (QGProd.QGPARAM_UNIT != QGMig.QGPARAM_UNIT) { WriteCsvError("Le champ QGPARAM_UNIT est different", QGProd, csv); }

                                                        if (QGProd.QGPARAM_UNITCONVFACTOR != QGMig.QGPARAM_UNITCONVFACTOR) { WriteCsvError("Le champ QGPARAM_UNITCONVFACTOR est different", QGProd, csv); }

                                                        if (QGProd.QGPARAM_MIN_VALUE != QGMig.QGPARAM_MIN_VALUE) { WriteCsvError("Le champ QGPARAM_MIN_VALUE est different", QGProd, csv); }

                                                        if (QGProd.QGPARAM_MAX_VALUE != QGMig.QGPARAM_MAX_VALUE) { WriteCsvError("Le champ QGPARAM_MAX_VALUE est different", QGProd, csv); }

                                                        if (QGProd.QGPARAM_TYPECALCUL != QGMig.QGPARAM_TYPECALCUL) { WriteCsvError("Le champ QGPARAM_TYPECALCUL est different", QGProd, csv); }

                                                        if (QGProd.QGPARAM_TOBECHECKED != QGMig.QGPARAM_TOBECHECKED) { WriteCsvError("Le champ QGPARAM_TOBECHECKED est different", QGProd, csv); }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        //Write result in file with Date
                        string ReportLocation = @"D:\eData_PII\MigrationReporting\debugQG\Result-QGParam-" + DateTime.Now.ToString("ddMMyyyy-HHmss") + ".csv";
                        File.WriteAllText(ReportLocation, csv.ToString());

                        */
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + ex.StackTrace.ToString());
            }


            Console.WriteLine("END");

        }


        public static void WriteCsvError(string message, CustomerParam CP, StringBuilder csv)
        {
            var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20}", CP.PC_CODEARTICLE, CP.PN, CP.SPEC, CP.INVENTORY_ITEM_STATUS_CODE, CP.TR_LIBELLE, CP.CUSTOMERPARAM_NIVEAUREPORTING, CP.CUSTOMERPARAM_NAME, CP.CUSTOMERPARAM_UNIT, CP.CUSTOMERPARAM_UNITCONVFACTOR, CP.SOIPARAM_NAME, CP.CUSTOMERPARAM_MIN_VALUE, CP.CUSTOMERPARAM_MAX_VALUE, CP.CUSTOMERPARAM_VALEURCONSTANTE, CP.CUSTOMERPARAM_TYPECALCUL, CP.CUSTOMERPARAM_TOBECHECKED, CP.CUSTOMERPARAM_MANDATORY, CP.PC_WAFER_SIZE, CP.PC_FAMILLE, CP.PC_SI_THICKNESS, CP.PC_BOX_THICKNESS, message);
            csv.AppendLine(newLine);

        }

        public static QGParamFile SerializeQGFile(string path)
        {
            //Serialize txt file into object
            //QGParamFile conatains a list of Part commercial
            //Each Part commercial contain a list of QG parameter
            QGParamFile MigrationQGFile = new QGParamFile();
            try
            {

                string PC_code = string.Empty;
                List<String> listPC = new List<String>();

                //Create the list of Part commercial readed inside the file
                using (StreamReader file = new StreamReader(path))
                {
                    //int counter = 0;
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        string[] cellOne = ln.Split(';');
                        PC_code = cellOne[0];
                        if (!listPC.Contains(PC_code))
                        {
                            listPC.Add(PC_code);
                        }
                    }
                }

                //ListPC done
                MigrationQGFile.FileName = path;
                // Add in MigrationCustomerFile all PC with all QG
                foreach (String entry in listPC)
                {
                    QGConfigs QGConf = new QGConfigs();
                    QGConf.PC_CODEARTICLE = entry;

                    using (StreamReader file = new StreamReader(path))
                    {
                        //int counter = 0;
                        string ln;
                        while ((ln = file.ReadLine()) != null)
                        {
                            string[] cellOne = ln.Split(';');
                            if (cellOne[0] == entry)
                            {
                                QGParam QG = new QGParam();
                                QG.PC_CODEARTICLE = cellOne[0];
                                QG.PN = cellOne[1];
                                QG.SPEC = cellOne[2];
                                QG.INVENTORY_ITEM_STATUS_CODE = cellOne[3];
                                QG.QGPARAM_UNIT = cellOne[4];
                                QG.QGPARAM_UNITCONVFACTOR = cellOne[5];
                                QG.SOIPARAM_NAME = cellOne[6];
                                QG.QGPARAM_MIN_VALUE = cellOne[7];
                                QG.QGPARAM_MAX_VALUE = cellOne[8];
                                QG.QGPARAM_TYPECALCUL = cellOne[9];
                                QG.QGPARAM_TOBECHECKED = cellOne[10];
                                QG.CONTROL_LIMIT_TYPE = cellOne[11];
                                QG.SIZE = cellOne[12];
                                QG.CUSTOMER_NAME = cellOne[13];
                                QG.PROCESS = cellOne[14];
                                QG.PC_DESCRIPTION = cellOne[15];
                                QG.EPAISSEUR = cellOne[16];

                                QGConf.QGParams.Add(QG);
                            }
                        }
                    }
                    MigrationQGFile.QGConfigs.Add(QGConf);
                    //Console.WriteLine("ajout PC : " + PC.PC_CODEARTICLE);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return MigrationQGFile;
        }

        public static CustomerParamFile SerializeFile(string path)
        {
            CustomerParamFile MigrationCustomerFile = new CustomerParamFile();
            try
            {
                string PC_code = string.Empty;
                List<String> listPC = new List<String>();
                using (StreamReader file = new StreamReader(path))
                {
                    //int counter = 0;
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {

                        string[] cellOne = ln.Split(';');

                        PC_code = cellOne[0];

                        if (!listPC.Contains(PC_code))
                        {
                            listPC.Add(PC_code);
                        }

                    }
                }

                MigrationCustomerFile.FileName = path;

                foreach (String entry in listPC)
                {
                    PCConfig PC = new PCConfig();
                    PC.PC_CODEARTICLE = entry;

                    using (StreamReader file = new StreamReader(path))
                    {
                        //int counter = 0;
                        string ln;

                        while ((ln = file.ReadLine()) != null)
                        {

                            string[] cellOne = ln.Split(';');

                            if (cellOne[0] == entry)
                            {
                                CustomerParam CP = new CustomerParam();
                                CP.PC_CODEARTICLE = cellOne[0];
                                CP.PN = cellOne[1];
                                CP.SPEC = cellOne[2];
                                CP.INVENTORY_ITEM_STATUS_CODE = cellOne[3];
                                CP.TR_LIBELLE = cellOne[4];
                                CP.CUSTOMERPARAM_NIVEAUREPORTING = cellOne[5];
                                CP.CUSTOMERPARAM_NAME = cellOne[6];
                                CP.CUSTOMERPARAM_UNIT = cellOne[7];
                                CP.CUSTOMERPARAM_UNITCONVFACTOR = cellOne[8];
                                CP.SOIPARAM_NAME = cellOne[9];
                                CP.CUSTOMERPARAM_MIN_VALUE = cellOne[10];
                                CP.CUSTOMERPARAM_MAX_VALUE = cellOne[11];
                                CP.CUSTOMERPARAM_VALEURCONSTANTE = cellOne[12];
                                CP.CUSTOMERPARAM_TYPECALCUL = cellOne[13];
                                CP.CUSTOMERPARAM_TOBECHECKED = cellOne[14];
                                CP.CUSTOMERPARAM_MANDATORY = cellOne[15];
                                CP.PC_WAFER_SIZE = cellOne[16];
                                CP.PC_FAMILLE = cellOne[17];
                                CP.PC_SI_THICKNESS = cellOne[18];
                                CP.PC_BOX_THICKNESS = cellOne[19];

                                PC.customerParams.Add(CP);
                            }
                        }
                    }

                    MigrationCustomerFile.PCConfigs.Add(PC);
                    Console.WriteLine("ajout PC : " + PC.PC_CODEARTICLE);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return MigrationCustomerFile;
        }

        public static void WriteCsvError(string message, QGParam QG, StringBuilder csv)
        {

            //Add in Result file the CP not correct and a message to describe the difference
            var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17}", QG.PC_CODEARTICLE, QG.PN, QG.SPEC, QG.INVENTORY_ITEM_STATUS_CODE, QG.QGPARAM_UNIT, QG.QGPARAM_UNITCONVFACTOR, QG.SOIPARAM_NAME, QG.QGPARAM_MIN_VALUE, QG.QGPARAM_MAX_VALUE, QG.QGPARAM_TYPECALCUL, QG.QGPARAM_TOBECHECKED, QG.CONTROL_LIMIT_TYPE, QG.SIZE, QG.CUSTOMER_NAME, QG.PROCESS, QG.PC_DESCRIPTION, QG.EPAISSEUR, message);
            csv.AppendLine(newLine);

        }

    }
}
