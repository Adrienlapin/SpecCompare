using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiragtionToolV2
{
    class CompareFile
    { 
     public Succed GenerationCustomerReport(Succed back,string ProdFile, string MigrationFile, string ReportLocation, string ShipmentPlanLocation, string InjectionLastResult)
        {
            try
            {              
                // Vocabulary :
                // PC = Part Commercial & CP = Customer Parameter
                // 1 Part commercial contain a list of Customer Parameter

                //Start
                var csv = new StringBuilder();

                //Serialized CSV FILE in CustomerParamFile
                //Add data from file to object
                CustomerParamFile MigrationCustomerFile = SerializeCustomerFile(MigrationFile, ShipmentPlanLocation);
                CustomerParamFile ProdCustomerFile = SerializeCustomerFile(ProdFile, ShipmentPlanLocation);


                //add Header in Result file
                WriteCsvHeader("ENV","", "", ProdCustomerFile.PCConfigs[0].customerParams[0], csv);

                //Permier resultat Prod vs Migration
                csv = CompareCP(ProdCustomerFile, MigrationCustomerFile, csv, "PROD");

                //Deuxieme resultat Migration vs Prod
                csv = CompareCP(MigrationCustomerFile, ProdCustomerFile, csv,"SpecMngr");


                //Write result in file with Date
                string ReportOutLocation = ReportLocation + @"\Result-Customer_Param-" + DateTime.Now.ToString("ddMMyyyy-HHmss") + ".csv";
                string ReportOutLocation2 = ReportLocation + @"\Result-CustomerParam-" + DateTime.Now.ToString("ddMMyyyy-HHmss") + ".csv";

                // Write result in file with Date
                using (StreamWriter outputFile = new StreamWriter(ReportOutLocation, false, System.Text.Encoding.Default))
                {
                    outputFile.WriteLine(csv.ToString());
                }


                csv.Clear();


                //Option : Completer le fichier avec l'injection des resultats précedents
                if (!String.IsNullOrEmpty(InjectionLastResult))
                {
                    //1 recupérer les resltats prédents
                    List<String> DicKey = new List<String>();
                    List<String> DicKey2 = new List<String>();
                    var CsvOut = new StringBuilder();

                    using (StreamReader reader = new StreamReader(ReportOutLocation, System.Text.Encoding.Default))
                    {
                        string line;

                        while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                        {
                            string[] split = line.Split(';');
                            string key = split[0] + split[1] + split[7] + split[5] + split[14] + split[21];
                            bool Commfound = false;

                            using (StreamReader fileLastResult = new StreamReader(InjectionLastResult, System.Text.Encoding.Default))
                            {
                                string ligne;
                                Commfound = false;
                                while (!String.IsNullOrEmpty(ligne = fileLastResult.ReadLine()))
                                {
                                    string[] cell = ligne.Split(';');
                                    try
                                    {
                                        if ((split[0] == cell[0]) && (split[1] == cell[1]) && (split[7] == cell[7]) && (split[5] == cell[5]) && (split[14] == cell[14]) && (split[21] == cell[21]) && !DicKey2.Contains(key))
                                        {
                                            string Add = string.Format(";{0};{1};{2};{3}", cell[22], cell[23], cell[24], cell[25]);
                                            line = line + Add;
                                            DicKey2.Add(key);
                                            Commfound = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        back.IsSucced = false;
                                        string error = "Erreur Nb colonne sur PC : " + split[1] + "  Param : " + split[7];
                                        back.message = ex.Message.ToString() + "\n" + error;
                                        return back;
                                    }
                                }

                            }
                            if (!Commfound)
                            {
                                line = line + ";;;;";
                            }
                            if (!DicKey.Contains(key))
                            {
                                DicKey.Add(key);
                                CsvOut.AppendLine(line);
                            }
                        }
                        
                    }

                    DicKey.Clear();

                    // Write result in file with Date
                    using (StreamWriter outputFile = new StreamWriter(ReportOutLocation2, false, System.Text.Encoding.Default))
                    {                         
                            outputFile.WriteLine(CsvOut.ToString());
                    }

                    //delete first ReportFinal 
                    File.Delete(ReportOutLocation);
                }
                back.IsSucced = true;     
            }
            catch (Exception ex)
            {
                back.IsSucced = false;
                if (String.IsNullOrEmpty(back.message))
                {
                    back.message = ex.Message.ToString() + "\n" + ex.StackTrace.ToString();
                }
                return back;
            }
            return back;
        }

         public static CustomerParamFile SerializeCustomerFile(string path, string ShipmentPlanLocation)
            {
            //Serialize txt file into object
            //CustomerParamFile conatains a list of Part commercial
            //Each Part commercial contain a list of customer parameter
            CustomerParamFile MigrationCustomerFile = new CustomerParamFile();
            try
            {
                    string PC_code = string.Empty;
                    List<String> listPC = new List<String>();
                //Create the list of Part commercial readed inside the file
                    using (StreamReader file = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        //int counter = 0;
                        string ln;
                        while (!String.IsNullOrEmpty(ln = file.ReadLine()))
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
                    MigrationCustomerFile.FileName = path;

                //Create the list of Part commercial readed inside the file Shipment Plan 
                List<String> listPCHaveShipmentPlan = new List<String>();
                if (!String.IsNullOrEmpty(ShipmentPlanLocation)) {
                    using (StreamReader file = new StreamReader(ShipmentPlanLocation, System.Text.Encoding.Default))
                    {
                        //int counter = 0;
                        string ln;
                        while (!String.IsNullOrEmpty(ln = file.ReadLine()))
                        {
                            string[] cellOne = ln.Split(';');
                            PC_code = cellOne[0];
                            if (!listPCHaveShipmentPlan.Contains(PC_code))
                            {
                                listPCHaveShipmentPlan.Add(PC_code);
                            }
                        }
                    }
                }

                // Add in MigrationCustomerFile all PC with all CP
                foreach (String entry in listPC)
                    {
                        PCConfig PC = new PCConfig();
                        PC.PC_CODEARTICLE = entry;

                    if (String.IsNullOrEmpty(ShipmentPlanLocation)) 
                    {
                        PC.PC_HaveShipmentPlan = "";
                    } 
                    else
                    {
                        if (listPCHaveShipmentPlan.Contains(entry))
                        {
                            PC.PC_HaveShipmentPlan = "Yes";
                        } 
                        else 
                        {
                            PC.PC_HaveShipmentPlan = "No";
                        }
                    }
                        

                        using (StreamReader file = new StreamReader(path, System.Text.Encoding.Default))
                        {
                            //int counter = 0;
                            string ln;

                            while (!String.IsNullOrEmpty(ln = file.ReadLine()))
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
                        //Console.WriteLine("ajout PC : " + PC.PC_CODEARTICLE);
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString() + "\n" + ex.StackTrace.ToString());
                }
                return MigrationCustomerFile;
            }


        public StringBuilder CompareCP(CustomerParamFile ProdCustomerFile, CustomerParamFile MigrationCustomerFile, StringBuilder csv, string environement)
        {
            //At this step we have two object contains all data from Prod file and Migration file.
            bool PCFound = false;
            bool CPFound = false;


            //Start treatment with 3 level of control
            //If PC exist
            //If all CP exist for each PC
            //If we have some difference between each CP for each PC
            foreach (PCConfig PCprod in ProdCustomerFile.PCConfigs)
            {
                PCFound = false;
                //Search PC in migration file
                foreach (PCConfig PCmigration in MigrationCustomerFile.PCConfigs)
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
                    WriteCsvError(environement,"PC not found !", PCprod.PC_HaveShipmentPlan, PCprod.customerParams[0], csv);
                }
                else
                {
                    //PC found, then search all CP for thie PC
                    foreach (CustomerParam CPProd in PCprod.customerParams)
                    {
                        CPFound = false;
                        foreach (PCConfig PCmigration in MigrationCustomerFile.PCConfigs)
                        {
                            if (PCprod.PC_CODEARTICLE == PCmigration.PC_CODEARTICLE)
                            {
                                foreach (CustomerParam CPMig in PCmigration.customerParams)
                                {
                                    //NB a CP is define by "Name + Type_Report + Type_Calcul"
                                    if (CPProd.CUSTOMERPARAM_NAME == CPMig.CUSTOMERPARAM_NAME && CPProd.CUSTOMERPARAM_TYPECALCUL == CPMig.CUSTOMERPARAM_TYPECALCUL && CPProd.TR_LIBELLE == CPMig.TR_LIBELLE)
                                    {
                                        //CP found in migration
                                        CPFound = true;
                                    }
                                }
                            }
                        }
                        if (!CPFound)
                        {
                            //CP not Found !
                            WriteCsvError(environement,"CP not found ! (Type_Report or Type_Calcul can be different)", PCprod.PC_HaveShipmentPlan, CPProd, csv);
                        }
                        else
                        {
                            foreach (PCConfig PCmigration in MigrationCustomerFile.PCConfigs)
                            {
                                if (PCprod.PC_CODEARTICLE == PCmigration.PC_CODEARTICLE)
                                {
                                    foreach (CustomerParam CPMig in PCmigration.customerParams)
                                    {
                                        //CP found : search difference between value =>
                                        if (CPProd.CUSTOMERPARAM_NAME == CPMig.CUSTOMERPARAM_NAME && CPProd.CUSTOMERPARAM_TYPECALCUL == CPMig.CUSTOMERPARAM_TYPECALCUL && CPProd.TR_LIBELLE == CPMig.TR_LIBELLE)
                                        {
                                            if (CPProd.CUSTOMERPARAM_NIVEAUREPORTING != CPMig.CUSTOMERPARAM_NIVEAUREPORTING) { WriteCsvError(environement,"Le champ CUSTOMERPARAM_NIVEAUREPORTING est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.CUSTOMERPARAM_UNIT != CPMig.CUSTOMERPARAM_UNIT)
                                            {
                                                //Add nomenclature testing, make difference bettween value and difference with upper
                                                //Example : 'Ang' & 'ANG' is the same value

                                                //Step 1 : is same nomencalture ?
                                                bool IsSameNom = IsSameNomenclature(CPProd.CUSTOMERPARAM_UNIT, CPMig.CUSTOMERPARAM_UNIT);

                                                if (IsSameNom)
                                                {
                                                    WriteCsvError(environement,"Le champ CUSTOMERPARAM_UNIT a une nomenclature differente", PCprod.PC_HaveShipmentPlan, CPProd, csv);
                                                }
                                                else
                                                {
                                                    WriteCsvError(environement, "Le champ CUSTOMERPARAM_UNIT est different", PCprod.PC_HaveShipmentPlan, CPProd, csv);
                                                }
                                            }

                                            if (CPProd.CUSTOMERPARAM_UNITCONVFACTOR != CPMig.CUSTOMERPARAM_UNITCONVFACTOR) { WriteCsvError(environement, "Le champ CUSTOMERPARAM_UNITCONVFACTOR est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.SOIPARAM_NAME != CPMig.SOIPARAM_NAME) { WriteCsvError(environement, "Le champ SOIPARAM_NAME est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.CUSTOMERPARAM_MIN_VALUE != CPMig.CUSTOMERPARAM_MIN_VALUE) { WriteCsvError(environement, "Le champ CUSTOMERPARAM_MIN_VALUE est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.CUSTOMERPARAM_MAX_VALUE != CPMig.CUSTOMERPARAM_MAX_VALUE) { WriteCsvError(environement, "Le champ CUSTOMERPARAM_MAX_VALUE est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.CUSTOMERPARAM_VALEURCONSTANTE != CPMig.CUSTOMERPARAM_VALEURCONSTANTE) { WriteCsvError(environement, "Le champ CUSTOMERPARAM_VALEURCONSTANTE est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.CUSTOMERPARAM_TOBECHECKED != CPMig.CUSTOMERPARAM_TOBECHECKED) { WriteCsvError(environement, "Le champ CUSTOMERPARAM_TOBECHECKED est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                            if (CPProd.CUSTOMERPARAM_MANDATORY != CPMig.CUSTOMERPARAM_MANDATORY) { WriteCsvError(environement, "Le champ CUSTOMERPARAM_MANDATORY est different", PCprod.PC_HaveShipmentPlan, CPProd, csv); }

                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return csv;
        }

        public Succed GenerationQGParamReport(Succed back,string ProdFile, string MigrationFile, string ReportLocation, string ShipmentPlanLocation, string InjectionLastResult)
        {
            try
            {

                // Vocabulary :
                // PC = Part Commercial & QG = GG Parameter
                // 1 Part commercial contain a list of QG Parameter

                //Start
                var csv = new StringBuilder();

                //Serialized CSV FILE in QGParamFile
                //Add data from file to object
                QGParamFile MigrationQGFile = SerializeQGFile(MigrationFile, ShipmentPlanLocation);
                QGParamFile ProdQGFile = SerializeQGFile(ProdFile, ShipmentPlanLocation);

                
                //add Header in Result file
                WriteCsvError("ENV","","", ProdQGFile.QGConfigs[0].QGParams[0], csv);

                //Permier resultat Prod vs Migration
                csv = CompareQG(ProdQGFile, MigrationQGFile, csv, "PROD");

                //Deuxieme resultat Migration vs Prod
                csv = CompareQG(MigrationQGFile, ProdQGFile, csv, "SpecMngr");



                //Write result in file with Date
                string ReportOutLocation = ReportLocation + @"\Result_QGParam-" + DateTime.Now.ToString("ddMMyyyy-HHmss") + ".csv";
                string ReportOutLocation2 = ReportLocation + @"\Result-QGParam-" + DateTime.Now.ToString("ddMMyyyy-HHmss") + ".csv";

                // Write result in file with Date
                using (StreamWriter outputFile = new StreamWriter(ReportOutLocation, false, System.Text.Encoding.Default))
                {
                    outputFile.WriteLine(csv.ToString());
                }


                csv.Clear();

                //Option : Completer le fichier avec l'injection des resultats précedents
                if (!String.IsNullOrEmpty(InjectionLastResult))
                {
                    //1 recupérer les resltats prédents
                    List<String> DicKey = new List<String>();
                    List<String> DicKey2 = new List<String>();
                    var CsvOut = new StringBuilder();

                    using (StreamReader reader = new StreamReader(ReportOutLocation, System.Text.Encoding.Default))
                    {
                        string line;

                        while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                        {
                            string[] split = line.Split(';');
                            string key = split[0] + split[1] + split[7] + split[10];
                            bool Commfound = false;

                            using (StreamReader fileLastResult = new StreamReader(InjectionLastResult, System.Text.Encoding.Default))
                            {
                                string ligne;
                                Commfound = false;
                                while (!String.IsNullOrEmpty(ligne = fileLastResult.ReadLine()))
                                {
                                    string[] cell = ligne.Split(';');
                                    try
                                    {
                                        if ((split[0] == cell[0]) && (split[1] == cell[1]) && (split[7] == cell[7]) && (split[10] == cell[10]) && !DicKey2.Contains(key))
                                        {
                                            string Add = string.Format(";{0};{1};{2};{3}", cell[19], cell[20], cell[21], cell[22]);
                                            line = line + Add;
                                            DicKey2.Add(key);
                                            Commfound = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        back.IsSucced = false;
                                        string error = "Erreur Nb colonne sur PC : " + split[1] + "  Param : " + split[7];
                                        back.message = ex.Message.ToString() + "\n" + error;
                                        return back;
                                    }
                                }

                            }
                            if (!Commfound)
                            {
                                line = line + ";;;;";
                            }
                            if (!DicKey.Contains(key))
                            {
                                DicKey.Add(key);
                                CsvOut.AppendLine(line);
                            }
                        }

                    }

                    DicKey.Clear();

                    // Write result in file with Date
                    using (StreamWriter outputFile = new StreamWriter(ReportOutLocation2, false, System.Text.Encoding.Default))
                    {
                        outputFile.WriteLine(CsvOut.ToString());
                    }

                    //delete first ReportFinal 
                    File.Delete(ReportOutLocation);

                }
                back.IsSucced = true;
            }
            catch (Exception ex)
            {
                back.IsSucced = false;
                if (String.IsNullOrEmpty(back.message))
                {
                    back.message = ex.Message.ToString() + "\n" + ex.StackTrace.ToString();
                }
                return back;
            }
            return back;
        }

        public StringBuilder CompareQG(QGParamFile  ProdQGFile, QGParamFile MigrationQGFile,StringBuilder csv,string environement)
        {
            //At this step we have two object contains all data from Prod file and Migration file.
            bool PCFound = false;
            bool QGFound = false;

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
                    WriteCsvError(environement,"PC not found !", PCprod.PC_HaveShipmentPlan, PCprod.QGParams[0], csv);
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
                            WriteCsvError(environement, "QG not found ! (Type_Calcul can be different)", PCprod.PC_HaveShipmentPlan, QGProd, csv);
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
                                            if (QGProd.CUSTOMER_NAME != QGMig.CUSTOMER_NAME) { WriteCsvError(environement, "Le champ CUSTOMER_NAME est different", PCprod.PC_HaveShipmentPlan, QGProd, csv); }

                                            if (QGProd.QGPARAM_UNIT != QGMig.QGPARAM_UNIT)
                                            {

                                                //Add nomenclature testing, make difference bettween value and difference with upper
                                                //Example : 'Ang' & 'ANG' is the same value

                                                //Step 1 : is same nomencalture ?
                                                bool IsSameNom = IsSameNomenclature(QGProd.QGPARAM_UNIT, QGMig.QGPARAM_UNIT);

                                                if (IsSameNom)
                                                {
                                                    WriteCsvError(environement, "Le champ QGPARAM_UNIT a une nomenclature differente", PCprod.PC_HaveShipmentPlan, QGProd, csv);
                                                }
                                                else
                                                {
                                                    WriteCsvError(environement, "Le champ QGPARAM_UNIT est different", PCprod.PC_HaveShipmentPlan, QGProd, csv);
                                                }

                                            }

                                            if (QGProd.QGPARAM_UNITCONVFACTOR != QGMig.QGPARAM_UNITCONVFACTOR) { WriteCsvError(environement, "Le champ QGPARAM_UNITCONVFACTOR est different", PCprod.PC_HaveShipmentPlan, QGProd, csv); }

                                            if (QGProd.QGPARAM_MIN_VALUE != QGMig.QGPARAM_MIN_VALUE) { WriteCsvError(environement, "Le champ QGPARAM_MIN_VALUE est different", PCprod.PC_HaveShipmentPlan, QGProd, csv); }

                                            if (QGProd.QGPARAM_MAX_VALUE != QGMig.QGPARAM_MAX_VALUE) { WriteCsvError(environement, "Le champ QGPARAM_MAX_VALUE est different", PCprod.PC_HaveShipmentPlan, QGProd, csv); }

                                            if (QGProd.QGPARAM_TYPECALCUL != QGMig.QGPARAM_TYPECALCUL) { WriteCsvError(environement, "Le champ QGPARAM_TYPECALCUL est different", PCprod.PC_HaveShipmentPlan, QGProd, csv); }

                                            if (QGProd.QGPARAM_TOBECHECKED != QGMig.QGPARAM_TOBECHECKED) { WriteCsvError(environement, "Le champ QGPARAM_TOBECHECKED est different", PCprod.PC_HaveShipmentPlan, QGProd, csv); }

                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return csv;
        }

        public static QGParamFile SerializeQGFile(string path, string ShipmentPlanLocation)
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
                using (StreamReader file = new StreamReader(path, System.Text.Encoding.Default))
                {
                    //int counter = 0;
                    string ln;

                    while (!String.IsNullOrEmpty(ln = file.ReadLine()))
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


                //Create the list of Part commercial readed inside the file Shipment Plan 
                List<String> listPCHaveShipmentPlan = new List<String>();
                if (!String.IsNullOrEmpty(ShipmentPlanLocation))
                {
                    using (StreamReader file = new StreamReader(ShipmentPlanLocation, System.Text.Encoding.Default))
                    {
                        //int counter = 0;
                        string ln;
                        while (!String.IsNullOrEmpty(ln = file.ReadLine()))
                        {
                            string[] cellOne = ln.Split(';');
                            PC_code = cellOne[0];
                            if (!listPCHaveShipmentPlan.Contains(PC_code))
                            {
                                listPCHaveShipmentPlan.Add(PC_code);
                            }
                        }
                    }
                }

                MigrationQGFile.FileName = path;
                // Add in MigrationCustomerFile all PC with all QG
                foreach (String entry in listPC)
                {
                    QGConfigs QGConf = new QGConfigs();
                    QGConf.PC_CODEARTICLE = entry;

                    if (String.IsNullOrEmpty(ShipmentPlanLocation))
                    {
                        QGConf.PC_HaveShipmentPlan = "";
                    }
                    else
                    {
                        if (listPCHaveShipmentPlan.Contains(entry))
                        {
                            QGConf.PC_HaveShipmentPlan = "Yes";
                        }
                        else
                        {
                            QGConf.PC_HaveShipmentPlan = "No";
                        }
                    }

                    using (StreamReader file = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        //int counter = 0;
                        string ln;
                        while (!String.IsNullOrEmpty(ln = file.ReadLine()))
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


        public static void WriteCsvHeader(string environement, string message, string HaveShipmentPlan, CustomerParam CP, StringBuilder csv)
        {

            //Add in Result file the CP not correct and a message to describe the difference
            var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};;;;", environement, CP.PC_CODEARTICLE, CP.PN, CP.SPEC, CP.INVENTORY_ITEM_STATUS_CODE, CP.TR_LIBELLE, CP.CUSTOMERPARAM_NIVEAUREPORTING, CP.CUSTOMERPARAM_NAME, CP.CUSTOMERPARAM_UNIT, CP.CUSTOMERPARAM_UNITCONVFACTOR, CP.SOIPARAM_NAME, CP.CUSTOMERPARAM_MIN_VALUE, CP.CUSTOMERPARAM_MAX_VALUE, CP.CUSTOMERPARAM_VALEURCONSTANTE, CP.CUSTOMERPARAM_TYPECALCUL, CP.CUSTOMERPARAM_TOBECHECKED, CP.CUSTOMERPARAM_MANDATORY, CP.PC_WAFER_SIZE, CP.PC_FAMILLE, CP.PC_SI_THICKNESS, CP.PC_BOX_THICKNESS, message, HaveShipmentPlan);
            csv.AppendLine(newLine);

        }

        public static void WriteCsvError(string environement,string message, string HaveShipmentPlan, CustomerParam CP, StringBuilder csv)
        {

            //Add in Result file the CP not correct and a message to describe the difference
            var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22}", environement, CP.PC_CODEARTICLE, CP.PN, CP.SPEC, CP.INVENTORY_ITEM_STATUS_CODE, CP.TR_LIBELLE, CP.CUSTOMERPARAM_NIVEAUREPORTING, CP.CUSTOMERPARAM_NAME, CP.CUSTOMERPARAM_UNIT, CP.CUSTOMERPARAM_UNITCONVFACTOR, CP.SOIPARAM_NAME, CP.CUSTOMERPARAM_MIN_VALUE, CP.CUSTOMERPARAM_MAX_VALUE, CP.CUSTOMERPARAM_VALEURCONSTANTE, CP.CUSTOMERPARAM_TYPECALCUL, CP.CUSTOMERPARAM_TOBECHECKED, CP.CUSTOMERPARAM_MANDATORY, CP.PC_WAFER_SIZE, CP.PC_FAMILLE, CP.PC_SI_THICKNESS, CP.PC_BOX_THICKNESS, message, HaveShipmentPlan);
            csv.AppendLine(newLine);

        }

        public static void WriteCsvError(string environement,string message, string HaveShipmentPlan, QGParam QG, StringBuilder csv)
        {

            //Add in Result file the CP not correct and a message to describe the difference
            var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19}", environement, QG.PC_CODEARTICLE, QG.PN, QG.SPEC, QG.INVENTORY_ITEM_STATUS_CODE, QG.QGPARAM_UNIT, QG.QGPARAM_UNITCONVFACTOR, QG.SOIPARAM_NAME, QG.QGPARAM_MIN_VALUE, QG.QGPARAM_MAX_VALUE, QG.QGPARAM_TYPECALCUL, QG.QGPARAM_TOBECHECKED, QG.CONTROL_LIMIT_TYPE, QG.SIZE, QG.CUSTOMER_NAME, QG.PROCESS, QG.PC_DESCRIPTION, QG.EPAISSEUR, message, HaveShipmentPlan);
            csv.AppendLine(newLine);

        }

        public bool IsSameNomenclature(String CPProdUNIT, String CPMigUNIT) 
        {
            bool IsSameNom = false;

            CPProdUNIT.Replace((char)0xB2, (char)0x32);
            CPMigUNIT.Replace((char)0xB2, (char)0x32);

            //Test /cm^2
            if (
                (CPProdUNIT.Equals("/cm^2") || CPProdUNIT.Equals("cm²") || CPProdUNIT.Equals("/cm2")) 
                && 
                (CPMigUNIT.Equals("/cm^2") || CPMigUNIT.Equals("cm²") || CPMigUNIT.Equals("/cm2")))
            {
                IsSameNom = true;
            }

            //Test /cm2/eV
            if (
                (CPProdUNIT.Equals("/cm2/eV") || CPProdUNIT.Equals("/cm^2/eV"))
                &&
                (CPMigUNIT.Equals("/cm2/eV") || CPMigUNIT.Equals("/cm^2/eV")))
            {
                IsSameNom = true;
            }

            //Test /cm³
            if (
                (CPProdUNIT.Equals("/cm³") || CPProdUNIT.Equals("/cm3") || CPProdUNIT.Equals("/cm^3"))
                &&
                (CPMigUNIT.Equals("/cm³") || CPMigUNIT.Equals("/cm3") || CPMigUNIT.Equals("/cm^3")))
            {
                IsSameNom = true;
            }

            //Test Ang
            if (
                (CPProdUNIT.Equals("Ang") || CPProdUNIT.Equals("ANG") || CPProdUNIT.Equals("A") || CPProdUNIT.Equals("/cm^3"))
                &&
                (CPMigUNIT.Equals("Ang") || CPMigUNIT.Equals("ANG") || CPMigUNIT.Equals("A") || CPMigUNIT.Equals("Å")))
            {
                IsSameNom = true;
            }

            //Test CM2/(VSEC)
            if (
                (CPProdUNIT.Equals("CM2/(VSEC)") || CPProdUNIT.Equals("CM^2/(VSEC)"))
                &&
                (CPMigUNIT.Equals("CM2/(VSEC)") || CPMigUNIT.Equals("CM^2/(VSEC)")))
            {
                IsSameNom = true;
            }

            //Test COUNT
            if (
                (CPProdUNIT.Equals("count") || CPProdUNIT.Equals("COUNT"))
                &&
                (CPMigUNIT.Equals("count") || CPMigUNIT.Equals("COUNT")))
            {
                IsSameNom = true;
            }

            //Test def/cm2
            if (
                (CPProdUNIT.Equals("def/cm2") || CPProdUNIT.Equals("DEF/cm2") || CPProdUNIT.Equals("def/cm²"))
                &&
                (CPMigUNIT.Equals("def/cm2") || CPMigUNIT.Equals("DEF/cm2") || CPMigUNIT.Equals("def/cm²")))
            {
                IsSameNom = true;
            }

            //Test degree
            if (
                (CPProdUNIT.Equals("Deg") || CPProdUNIT.Equals("DEG") || CPProdUNIT.Equals("degree"))
                &&
                (CPMigUNIT.Equals("Deg") || CPMigUNIT.Equals("DEG") || CPMigUNIT.Equals("degree")))
            {
                IsSameNom = true;
            }

            //Test E-10dfts/cm3
            if (
                (CPProdUNIT.Equals("e-10df/cm3") || CPProdUNIT.Equals("E-10dfts/cm^3") || CPProdUNIT.Equals("E-10dfts/cm3"))
                &&
                (CPMigUNIT.Equals("e-10df/cm3") || CPMigUNIT.Equals("E-10dfts/cm^3") || CPMigUNIT.Equals("E-10dfts/cm3")))
            {
                IsSameNom = true;
            }

            //Test e10at/cm3
            if (
                (CPProdUNIT.Equals("e10at/cm3") || CPProdUNIT.Equals("e10 at/cm3"))
                &&
                (CPMigUNIT.Equals("e10at/cm3") || CPMigUNIT.Equals("e10 at/cm3")))
            {
                IsSameNom = true;
            }

            //Test µm
            if (
                (CPProdUNIT.ToUpper().Equals("UM") || CPProdUNIT.Equals("µm"))
                &&
                (CPMigUNIT.ToUpper().Equals("UM") || CPMigUNIT.Equals("µm")))
            {
                IsSameNom = true;
            }

            //Test PPMA
            if (
                (CPProdUNIT.Equals("PPMA") || CPProdUNIT.Equals("ppma (SEMI MF 1391)") || CPProdUNIT.Equals("ppma"))
                &&
                (CPMigUNIT.Equals("PPMA") || CPMigUNIT.Equals("ppma (SEMI MF 1391)") || CPMigUNIT.Equals("ppma")))
            {
                IsSameNom = true;
            }

            //Test New ppma
            if (
                CPProdUNIT.ToUpper().Equals("NEW PPMA") 
                &&
                 CPMigUNIT.ToUpper().Equals("NEW PPMA"))
            {
                IsSameNom = true;
            }

            //Test old ppma
            if (
                (CPProdUNIT.Equals("old ppma ASTM F121 (ed.1979)") || CPProdUNIT.Equals("old ppma"))
                &&
                (CPMigUNIT.Equals("old ppma ASTM F121 (ed.1979)") || CPMigUNIT.Equals("old ppma")))
            {
                IsSameNom = true;
            }
            


            //Test nm/mm^2
            if (
                (CPProdUNIT.Equals("nm/mm^2") || CPProdUNIT.Equals("nm/mm²") || CPProdUNIT.Equals("nm/mm2"))
                &&
                (CPMigUNIT.Equals("nm/mm^2") || CPMigUNIT.Equals("nm/mm²") || CPMigUNIT.Equals("nm/mm2")))
            {
                IsSameNom = true;
            }

            //Test mm2
            if (
                (CPProdUNIT.Equals("mm2") || CPProdUNIT.Equals("mm²") || CPProdUNIT.Equals("mm^2"))
                &&
                (CPMigUNIT.Equals("mm2") || CPMigUNIT.Equals("mm²") || CPMigUNIT.Equals("mm^2")))
            {
                IsSameNom = true;
            }

            //Test minute
            if (
                (CPProdUNIT.Equals("minute") || CPProdUNIT.Equals("minutes"))
                &&
                (CPMigUNIT.Equals("minute") || CPMigUNIT.Equals("minutes")))
            {
                IsSameNom = true;
            }

            //Test e17at/cm3
            if (
                (CPProdUNIT.Equals("e17at/cm3") || CPProdUNIT.Equals("e17 at/cm3") || CPProdUNIT.Equals("E17 atoms/cm3"))
                &&
                (CPMigUNIT.Equals("e17at/cm3") || CPMigUNIT.Equals("e17 at/cm3") || CPMigUNIT.Equals("E17 atoms/cm3")))
            {
                IsSameNom = true;
            }

            //Test e12 At/cm2
            if (
                (CPProdUNIT.Equals("e12 At/cm2") || CPProdUNIT.Equals("e12at/cm2") || CPProdUNIT.Equals("e12 at/cm2"))
                &&
                (CPMigUNIT.Equals("e12 At/cm2") || CPMigUNIT.Equals("e12at/cm2") || CPMigUNIT.Equals("e12 at/cm2")))
            {
                IsSameNom = true;
            }

            //Test e9 At/cm2
            if (
                (CPProdUNIT.Equals("e9at/cm2") || CPProdUNIT.Equals("e9 At/cm2") || CPProdUNIT.Equals("E9 atoms / cm2"))
                &&
                (CPMigUNIT.Equals("e9at/cm2") || CPMigUNIT.Equals("e9 At/cm2") || CPMigUNIT.Equals("E9 atoms / cm2")))
            {
                IsSameNom = true;
            }

            //Test E10at/cm²
            if (
                (CPProdUNIT.Equals("E10at/cm²") || CPProdUNIT.Equals("e10 at/cm²") || CPProdUNIT.Equals("e10 at/cm2") || CPProdUNIT.Equals("e10at/cm²") || CPProdUNIT.Equals("E10 at/cm²") || CPProdUNIT.Equals("E10 at/cm2") || CPProdUNIT.Equals("E10 at/cm²") || CPProdUNIT.Equals("e10At/cm2") || CPProdUNIT.Equals("E10At/cm²") || CPProdUNIT.Equals("e10 At/cm²") || CPProdUNIT.Equals("e10 At/cm2") || CPProdUNIT.Equals("e10At/cm²") || CPProdUNIT.Equals("E10 At/cm²") || CPProdUNIT.Equals("E10 At/cm2") || CPProdUNIT.Equals("E10 At/cm²") || CPProdUNIT.Equals("E10 atoms/cm2") || CPProdUNIT.Equals("E10 atom/cm2") || CPProdUNIT.Equals("e10at/cm2") || CPProdUNIT.Equals("E10 atoms/cm²"))
                &&
                (CPMigUNIT.Equals("E10at/cm²") || CPMigUNIT.Equals("e10 at/cm²") || CPMigUNIT.Equals("e10 at/cm2") || CPMigUNIT.Equals("e10at/cm²") || CPMigUNIT.Equals("E10 at/cm²") || CPMigUNIT.Equals("E10 at/cm2") || CPMigUNIT.Equals("E10 at/cm²") || CPMigUNIT.Equals("e10At/cm2") || CPMigUNIT.Equals("E10At/cm²") || CPMigUNIT.Equals("e10 At/cm²") || CPMigUNIT.Equals("e10 At/cm2") || CPMigUNIT.Equals("e10At/cm²") || CPMigUNIT.Equals("E10 At/cm²") || CPMigUNIT.Equals("E10 At/cm2") || CPMigUNIT.Equals("E10 At/cm²") || CPMigUNIT.Equals("E10 atoms/cm2") || CPMigUNIT.Equals("E10 atom/cm2") || CPMigUNIT.Equals("e10at/cm2") || CPMigUNIT.Equals("E10 atoms/cm²")))
            {
                IsSameNom = true;
            }

            //Test DegCollat
            if (
                (CPProdUNIT.Equals("DegCollat") || CPProdUNIT.Equals("DEG. COLLAT."))
                &&
                (CPMigUNIT.Equals("DegCollat") || CPMigUNIT.Equals("DEG. COLLAT.")))
            {
                IsSameNom = true;
            }

            //Test DegLong
            if (
                (CPProdUNIT.Equals("DegLong") || CPProdUNIT.Equals("DEG. LONG."))
                &&
                (CPMigUNIT.Equals("DegLong") || CPMigUNIT.Equals("DEG. LONG.")))
            {
                IsSameNom = true;
            }

            //Test cm2/VSEC
            if (
                (CPProdUNIT.Equals("cm2/VSEC") || CPProdUNIT.Equals("CM^2/(VSEC)") || CPProdUNIT.Equals("cm²/(VSEC)"))
                &&
                (CPMigUNIT.Equals("cm2/VSEC") || CPMigUNIT.Equals("CM^2/(VSEC)") || CPMigUNIT.Equals("cm²/(VSEC)")))
            {
                IsSameNom = true;
            }

            return IsSameNom;
        }

    }
}
