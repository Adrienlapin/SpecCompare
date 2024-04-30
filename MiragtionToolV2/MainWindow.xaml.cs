using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiragtionToolV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool IsCustomer = false;
        private bool IsQualityGate = false;
        private bool IsQualitative = false;
        private bool FieldsKO = false;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Title = "MigrationTool V2";
            this.Width = 700;
            this.Height = 700;

            ProdFile.Visibility = Visibility.Hidden;
            MigrationFile.Visibility = Visibility.Hidden;
            ReportLocation.Visibility = Visibility.Hidden;
            ShipmentPlan.Visibility = Visibility.Hidden;
            InjectionLastResult.Visibility = Visibility.Hidden;



            LabelMigrationFile.Visibility = Visibility.Hidden;
            LabelProdFile.Visibility = Visibility.Hidden;
            LabelReportLocation.Visibility = Visibility.Hidden;
            LabelShipmentPlanLocation.Visibility = Visibility.Hidden;
            LabelInjectionLastResult.Visibility = Visibility.Hidden;

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MigrationFile.Text = string.Empty;
            ProdFile.Text = string.Empty;
            ReportLocation.Text = string.Empty;
            ShipmentPlan.Text = string.Empty;
            InjectionLastResult.Text = string.Empty;

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            FieldsKO = false;
            String message = "Erreur :\n";
            Succed back = new Succed();

            if (!File.Exists(ProdFile.Text))
            {
                FieldsKO = true;
                message = message + "Le fichier Migration File PROD n'existe pas, vérifiez le chemin.\n";
            }

            if (!File.Exists(MigrationFile.Text))
            {
                FieldsKO = true;
                message = message + "Le fichier Migration File n'existe pas, vérifiez le chemin.\n";
            }
            if (!Directory.Exists(ReportLocation.Text))
            {
                FieldsKO = true;
                message = message + "Le chemin " + ReportLocation.Text + " n'existe pas, vérifiez le chemin.\n";
            }
            if (FieldsKO)
            {
                MessageBox.Show(message);
            } else
            {
                CompareFile Comp = new CompareFile();
              

                if (IsCustomer)
                {
                    if (Comp.GenerationCustomerReport(back, ProdFile.Text, MigrationFile.Text, ReportLocation.Text, ShipmentPlan.Text, InjectionLastResult.Text).IsSucced)
                    {
                        MessageBox.Show("Report généré avec succes !");
                    } 
                    else
                    {
                        MessageBox.Show("Report erreur :( " + " \n" + back.message);
                    }

                }
                else if(IsQualityGate)
                {
                    if (Comp.GenerationQGParamReport(back, ProdFile.Text, MigrationFile.Text, ReportLocation.Text, ShipmentPlan.Text, InjectionLastResult.Text).IsSucced)
                    {
                        MessageBox.Show("Report généré avec succes !");
                    }
                    else
                    {
                        MessageBox.Show("Report erreur :( ");
                    }
                }
                else if (IsQualitative)
                {
                    MessageBox.Show("ERROR : Fonction non-implémentée !");

                }
                else
                {
                    MessageBox.Show("Nothing selected");
                }
            }
        }


        void ClickCustomerParam(Object sender, RoutedEventArgs args) {
            ProdFile.Visibility = Visibility.Visible;
            MigrationFile.Visibility = Visibility.Visible;
            ReportLocation.Visibility = Visibility.Visible;
            ShipmentPlan.Visibility = Visibility.Visible;
            InjectionLastResult.Visibility = Visibility.Visible;


            LabelProdFile.Content = "Customer Migration File PROD";
            LabelMigrationFile.Content = "Customer Migration File";

            LabelMigrationFile.Visibility = Visibility.Visible;
            LabelProdFile.Visibility = Visibility.Visible;
            LabelReportLocation.Visibility = Visibility.Visible;
            LabelShipmentPlanLocation.Visibility = Visibility.Visible;
            LabelInjectionLastResult.Visibility = Visibility.Visible;

            IsCustomer = true;
            IsQualityGate = false;
            IsQualitative = false;

        }
        void ClickQualityGateParam(Object sender, RoutedEventArgs args) {
            ProdFile.Visibility = Visibility.Visible;
            MigrationFile.Visibility = Visibility.Visible;
            ReportLocation.Visibility = Visibility.Visible;
            ShipmentPlan.Visibility = Visibility.Visible;
            InjectionLastResult.Visibility = Visibility.Visible;


            LabelProdFile.Content = "QualityGate Migration File PROD";
            LabelMigrationFile.Content = "QualityGate Migration File";

            LabelMigrationFile.Visibility = Visibility.Visible;
            LabelProdFile.Visibility = Visibility.Visible;
            LabelReportLocation.Visibility = Visibility.Visible;
            LabelShipmentPlanLocation.Visibility = Visibility.Visible;
            LabelInjectionLastResult.Visibility = Visibility.Visible;

            IsCustomer = false;
            IsQualityGate = true;
            IsQualitative = false;
        }
        void ClickQualitativeParam(Object sender, RoutedEventArgs args) {

            ProdFile.Visibility = Visibility.Visible;
            MigrationFile.Visibility = Visibility.Visible;
            ReportLocation.Visibility = Visibility.Visible;
            ShipmentPlan.Visibility = Visibility.Visible;
            InjectionLastResult.Visibility = Visibility.Hidden;


            LabelProdFile.Content = "Qualitative Migration File PROD";
            LabelMigrationFile.Content = "Qualitative Migration File";

            LabelMigrationFile.Visibility = Visibility.Visible;
            LabelProdFile.Visibility = Visibility.Visible;
            LabelReportLocation.Visibility = Visibility.Visible;
            LabelShipmentPlanLocation.Visibility = Visibility.Visible;
            LabelInjectionLastResult.Visibility = Visibility.Hidden;

            IsCustomer = false;
            IsQualityGate = false;
            IsQualitative = true;

        }


    }
}
