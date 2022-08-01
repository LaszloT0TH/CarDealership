using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InputForms;

namespace Car_Dealership
{
    public partial class Codec : Form
    {
        InputForm form;

        Label labelHelp, labelStatus, labelProgressBar;

        CancellationTokenSource cancellationTokenSource;

        CancellationToken cancellationToken;

        ProgressBar codecProgressBar;

        public static List<string> ApplicationSettings = new List<string>();

        /// <summary>
        /// Application encryption level if zero then no encryption. 
        /// Must be greater than -1
        /// </summary>
        public static int LevelOfAppEncryption = 0;

        /// <summary>
        /// Application setting the save type. 
        /// If true, the format is text with a ".csv" extension. 
        /// If false the format is binary with ".dat" extension.
        /// </summary>
        public static bool TextTrue_BinFalse = true;

        int LevelOfSource = 0;

        int LevelOfTarget = 0;

        string SourceFilename = String.Empty;

        string SourcExtension = String.Empty;

        string TargetExtension = String.Empty;

        const string DropDownMenuAllTables = "Alle Tabellen";

        const string DropDownMenuCostumer = "Costumer";

        const string DropDownMenuCar = "Car";

        const string DropDownMenuSale = "Sale";

        const string DropDownMenuSalesperson = "Salesperson";

        const string DropDownMenuSalesperson_Secret_Data = "Salesperson_Secret_Data";

        const string DropDownMenuUsernameAndPasswords = "UsernameAndPasswords";

        const string DropDownMenuLogin = "Login";

        const string DropDownMenuExtension1 = ".csv";

        const string DropDownMenuExtension2 = ".dat";

        public const string TheTableCompletedMessage = "Tabelle ist fertiggestellt";

        const string Help =
                         "Mehrstufige Ver- und Entschlüsselung von Tabellen speichern in Text- oder Binärformaten " +
                       "\n.csv oder .dat Erweiterung " +
                       "\n" +
                       "\nBevor Sie beginnen, vergewissern Sie sich, " +
                       "\ndass sich die von Ihnen gewählte Quelldatei im Ordner befindet" +
                       "\n" +
                       "\nDas fertige Dokument finden Sie in der Anwendungsordner mit Datum und Uhrzeit Titel" +
                       "\n" +
                       "\nAnwendungsverschlüsselungsstufe und Format" +
                       "\nEntfernen Sie das Datum aus dem Titel der neu erstellten neuen Verschlüsselungsstufentabellen" +
                       "\nNur ändern, wenn die neuen Verschlüsselungsstufentabellen verfügbar sind" +
                       "\nin \"Tabellename Verschlüsselungsstufe level.Erweiterung\" Format" +
                       "\n" +
                       "\nVon der Anwendung zur Laufzeit verwendete Tabellen" +
                       "\nDer Verschlüsselungsgrad und die Erweiterung der Tabellen können sich nicht voneinander unterscheiden" +
                       "\nDie Einstellung der Dateierweiterung und der Verschlüsselungsstufe gelten für alle Tabellen" +
                       //  "\n - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - " +
                       "\n";
        const string Status =
                         "Speichern läuft " +
                       //  "\n - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - " +
                       "\n";


        private void Start()
        {
            form = new InputForm(this);
            form.Add("Source filename", new InputSelect("Quelltabellename:", DropDownMenuTables()).SetSize(200))
                .Add("Source level of encryption", new InputSelect("Quellebene der Verschlüsselung:", DropDownMenuNumbers()).SelectIndex(LevelOfAppEncryption).SetSize(200))
                .Add("Source file extension", new InputSelect("Quelldateierweiterung:", DropDownMenuExtensions()).SelectIndex(SetOfSelectedIndex()).SetSize(200))
                .Add("Target level of encryption", new InputSelect("Zielebene der Verschlüsselung:", DropDownMenuNumbers()).SetSize(130))
                .Add("Target file extension", new InputSelect("Zieldateierweiterung:", DropDownMenuExtensions()).SetSize(130))
                .Add("Level Of App Encryption", new InputSelect("Appsverschlüsselungsstufe:", DropDownMenuNumbers()).SelectIndex(LevelOfAppEncryption).SetSize(70))
                .Add("Text or Binary", new InputSelect("Appsverschlüsselungsstufe:", DropDownMenuExtensions()).SelectIndex(SetOfSelectedIndex()).SetSize(70))
                .MoveTo(10, 10)
                .SetButtonZero("Tabelle speichern")
                .SetButtonFirst("Einstellen App", Visible = true)
                .SetButtonSecond("Hilfe", Visible = true)
                .SetButtonThird("Menu", Visible = true)
                .OnSubmitZero(() =>
                {
                    StorageInVariables();

                    if (labelHelp.Visible == true) labelHelp.Visible = false;

                    CallSave();
                })
                .OnSubmitFirst(() =>
                {
                    int setLevel = Convert.ToInt32(form["Level Of App Encryption"]);

                    string extensionFormat = form["Text or Binary"];

                    AppSettingsModifies(name.EncryptionLevel, setLevel);

                    AppSettingsModifies(name.Extension, extensionFormat: extensionFormat);

                    SettingAppEncryptionAndExtension();

                    MessageBox.Show($"Die neue Stufe ({setLevel}) und format ({extensionFormat}) ist eingestellt");
                })
                .OnSubmitSecond(() =>
                {
                    labelHelp.Visible = true;
                    labelHelp.Text = Help;
                })
                .OnSubmitThird(() =>
                {
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }

        private static string[] DropDownMenuTables()
        {
            string[] MenuTables = new string[8]
            {
                DropDownMenuAllTables,
                DropDownMenuCostumer,
                DropDownMenuCar,
                DropDownMenuSale,
                DropDownMenuSalesperson,
                DropDownMenuSalesperson_Secret_Data,
                DropDownMenuUsernameAndPasswords,
                DropDownMenuLogin
            };

            return MenuTables;
        }

        private static string[] DropDownMenuNumbers()
        {
            string[] MenuNumbers = new string[10];
            for (int i = 0; i < MenuNumbers.Length; i++)
            {
                MenuNumbers[i] = i.ToString();
            }
            return MenuNumbers;
        }

        private static string[] DropDownMenuExtensions()
        {
            string[] MenuExtensions = new string[]
            {
                DropDownMenuExtension1,
                DropDownMenuExtension2
            };
            return MenuExtensions;
        }

        /// <summary>
        /// The async method can't access it, so store it in variables
        /// </summary>
        private void StorageInVariables()
        {
            LevelOfSource = Convert.ToInt32(form["Source level of encryption"]);

            LevelOfTarget = Convert.ToInt32(form["Target level of encryption"]);

            SourceFilename = form["Source filename"];

            SourcExtension = form["Source file extension"];

            TargetExtension = form["Target file extension"];
        }

        private async void CallSave()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            Task.Run(Waiting);
            await SaveTable();
            labelProgressBar.Invoke(() => labelProgressBar.Text = String.Empty);
            cancellationTokenSource.Cancel(); 
            codecProgressBar.Invoke(() => codecProgressBar.Visible = false);
            MessageBox.Show(TheTableCompletedMessage);
            cancellationTokenSource = null;
        }

        private Task Waiting()
        {
            return Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    labelStatus.Invoke(() => labelStatus.Text = Status);
                    Thread.Sleep(350);
                    labelStatus.Invoke(new Action(() => labelStatus.Text = String.Empty));
                    Thread.Sleep(350);
                }
            });
        }
        
        private Task SaveTable()
        {
            return Task.Factory.StartNew(() =>
            {
                int waitingTime = 700;
                int ProgBarMax = 100;
                int ProgBarValueCounter = 1;
                
                codecProgressBar.Invoke(() => codecProgressBar.Visible = true);
                codecProgressBar.Invoke(() => codecProgressBar.Value = 0);
                codecProgressBar.Invoke(() => codecProgressBar.Maximum = ProgBarMax);

                void ProgressBarSettings(string FileName)
                {
                    labelProgressBar.Invoke(() => labelProgressBar.Text = FileName);
                    for (int i = 0; i < (ProgBarMax / ProgBarValueCounter); i++)
                    {
                        if (codecProgressBar.Value >= 100)
                        {
                            codecProgressBar.Invoke(() => codecProgressBar.Value--);
                        }
                        codecProgressBar.Invoke(() => codecProgressBar.Value++);
                    }
                    Thread.Sleep(waitingTime);
                }
                void NumberOfTables(int Count = 1)
                {
                    if (codecProgressBar.Value == 100)
                    {
                        codecProgressBar.Invoke(() => codecProgressBar.Value--);
                    }
                    codecProgressBar.Invoke(() => codecProgressBar.Value = ProgBarMax % Count);
                    ProgBarValueCounter = Count;
                }

                switch (SourceFilename)
                {
                    case DropDownMenuAllTables:
                        {
                            NumberOfTables(7);
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionTextToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionTextToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Sales.FileName); 
                                                    Sales.EncryptionTextToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionTextToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionTextToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionTextToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionTextToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionTextToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Sales.FileName);
                                                    Sales.EncryptionTextToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionTextToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionTextToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionTextToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    codecProgressBar.Invoke(() => codecProgressBar.Value++);
                                                    Thread.Sleep(waitingTime);
                                                    Customers.EncryptionBinToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Cars.FileName);
                                                    codecProgressBar.Invoke(() => codecProgressBar.Value++);
                                                    Thread.Sleep(waitingTime);
                                                    Cars.EncryptionBinToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Sales.FileName);
                                                    codecProgressBar.Invoke(() => codecProgressBar.Value++);
                                                    Thread.Sleep(waitingTime);
                                                    Sales.EncryptionBinToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionBinToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionBinToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionBinToText(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionBinToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionBinToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Sales.FileName);
                                                    Sales.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    
                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionBinToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionBinToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionBinToBin(LevelOfSource, LevelOfTarget);

                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuCostumer:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Customers.FileName);
                                                    Customers.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuCar:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Cars.FileName);
                                                    Cars.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuSale:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Sales.FileName);
                                                    Sales.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Sales.FileName);
                                                    Sales.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Sales.FileName);
                                                    Sales.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Sales.FileName);
                                                    Sales.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuSalesperson:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(SalesPersons.FileName);
                                                    SalesPersons.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuSalesperson_Secret_Data:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(SalesPersons_Secret_Data.FileName);
                                                    SalesPersons_Secret_Data.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuUsernameAndPasswords:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Usernames_And_Passwords.FileName);
                                                    Usernames_And_Passwords.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case DropDownMenuLogin:
                        {
                            switch (SourcExtension)
                            {
                                case DropDownMenuExtension1:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionTextToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionTextToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                case DropDownMenuExtension2:
                                    {
                                        switch (TargetExtension)
                                        {
                                            case DropDownMenuExtension1:
                                                {
                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionBinToText(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            case DropDownMenuExtension2:
                                                {
                                                    ProgressBarSettings(Logins.FileName);
                                                    Logins.EncryptionBinToBin(LevelOfSource, LevelOfTarget);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }
            });
        }


        private void CreateDisplayItem()
        {
            labelHelp = new Label();

            labelHelp.Visible = true;

            labelHelp.Location = new Point(280, 80);

            labelHelp.Size = new Size(25, 245);

            labelHelp.Font = new Font("sans-serif", 11f);

            labelHelp.AutoSize = true;

            this.Controls.Add(labelHelp);


            labelStatus = new Label();

            labelStatus.Visible = true;

            labelStatus.Location = new Point(400, 300);

            labelStatus.Size = new Size(25, 245);

            labelStatus.Font = new Font("sans-serif", 25f);

            labelStatus.AutoSize = true;

            this.Controls.Add(labelStatus);


            labelProgressBar= new Label();

            labelProgressBar.Visible = true;

            labelProgressBar.Location = new Point(400, 100);

            labelProgressBar.Size = new Size(25, 245);

            labelProgressBar.Font = new Font("sans-serif", 16f);

            labelProgressBar.AutoSize = true;

            this.Controls.Add(labelProgressBar);


            codecProgressBar = new ProgressBar();

            Controls.Add(codecProgressBar);

            codecProgressBar.Location = new Point(300, 150);

            codecProgressBar.Size = new Size(440, 23);

            codecProgressBar.TabIndex = 1;

            codecProgressBar.Visible = false;
        }


        struct Document
        {
            public name Name;
            public int LineIndex;
        }

        public enum name : short
        {
            CarId,
            CustomerId,
            SaleId,
            SalesPersonId,
            EncryptionLevel,
            Extension
        }

        public static void CreatOrReadAppSettings()
        {
            string[] CreatText = new string[]
            {
                "automatic implementation carId(Line 1)", // 148
                "1",
                "",
                "automatic implementation customerId(Line 4)", // 109
                "1",
                "",
                "automatic implementation saleId(Line 7)", // 71
                "1",
                "",
                "automatic implementation salesPersonId(Line 10)", // 30
                "1",
                "",
                "application encryption level(Line 13)",
                "0",
                "",
                "application extension(Line 16)",
                "True"
            };

            if (!File.Exists("AppSettings.dat"))
            {
                FileStream streamCreat = new FileStream("AppSettings.dat", FileMode.OpenOrCreate, FileAccess.Write);

                BinaryFormatter formatterCreat = new BinaryFormatter();

                foreach (string line in CreatText)
                {
                    formatterCreat.Serialize(streamCreat, line);
                    Console.WriteLine(line);
                }
                streamCreat.Close();
            }

            if (File.Exists("AppSettings.dat"))
            {
                FileStream streamRead = new FileStream("AppSettings.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatterRead = new BinaryFormatter();
                while (streamRead.Position != streamRead.Length)
                {
                    string line = (string)formatterRead.Deserialize(streamRead);
                    ApplicationSettings.Add(line);
                    Console.WriteLine(line);
                }
                streamRead.Close();
            }
        }

        public static int ReadId(name input)
        {
            int Id = 0;
            Dictionary<name, Document> dictionaryModifies = DictionaryModifies();
            foreach (KeyValuePair<name, Document> item in dictionaryModifies)
            {
                if (item.Key == input)
                {
                    Document document = item.Value;
                    switch (item.Key)
                    {
                        case name.CarId:
                            Id = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            break;
                        case name.CustomerId:
                            Id = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            break;
                        case name.SaleId:
                            Id = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            break;
                        case name.SalesPersonId:
                            Id = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            break;
                        case name.EncryptionLevel:
                            Id = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            break;
                        default:
                            break;
                    }
                }
            }
            return Id;
        }

        public static bool ReadExtension(name input)
        {
            bool Extension = false;
            Dictionary<name, Document> dictionaryModifies = DictionaryModifies();
            foreach (KeyValuePair<name, Document> item in dictionaryModifies)
            {
                if (item.Key == input)
                {
                    Document document = item.Value;
                    Extension = Convert.ToBoolean(ApplicationSettings[document.LineIndex]);
                }
            }
            return Extension;
        }

        static Dictionary<name, Document> DictionaryModifies()
        {
            Document CarId = new Document() { Name = name.CarId, LineIndex = 1 };
            Document CustomerId = new Document() { Name = name.CustomerId, LineIndex = 4 };
            Document SaleId = new Document() { Name = name.SaleId, LineIndex = 7 };
            Document SalesPersonId = new Document() { Name = name.SalesPersonId, LineIndex = 10 };
            Document EncryptionLevel = new Document() { Name = name.EncryptionLevel, LineIndex = 13 };
            Document Extension = new Document() { Name = name.Extension, LineIndex = 16 };

            Dictionary<name, Document> dictionaryModifies = new Dictionary<name, Document>();
            dictionaryModifies.Add(CarId.Name, CarId);
            dictionaryModifies.Add(CustomerId.Name, CustomerId);
            dictionaryModifies.Add(SaleId.Name, SaleId);
            dictionaryModifies.Add(SalesPersonId.Name, SalesPersonId);
            dictionaryModifies.Add(EncryptionLevel.Name, EncryptionLevel);
            dictionaryModifies.Add(Extension.Name, Extension);

            return dictionaryModifies;
        }

        public static void AppSettingsModifies(name setname, int setLevel = 0, string extensionFormat = "")
        {
            if (extensionFormat == DropDownMenuExtension1)
            {
                extensionFormat = "True";
            }
            else
            {
                extensionFormat = "False";
            }
            Dictionary<name, Document> dictionaryModifies = DictionaryModifies();

            int newValue = 0;
            string TextToBeModified = String.Empty;
            foreach (KeyValuePair<name, Document> item in dictionaryModifies)
            {
                if (item.Key == setname)
                {
                    Document document = item.Value;
                    switch (item.Key)
                    {
                        case name.CarId:
                            newValue = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            TextToBeModified = Convert.ToString(newValue + 1);
                            ApplicationSettings.Remove(ApplicationSettings[document.LineIndex]);
                            ApplicationSettings.Insert(document.LineIndex, TextToBeModified);
                            break;
                        case name.CustomerId:
                            newValue = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            TextToBeModified = Convert.ToString(newValue + 1);
                            ApplicationSettings.Remove(ApplicationSettings[document.LineIndex]);
                            ApplicationSettings.Insert(document.LineIndex, TextToBeModified);
                            break;
                        case name.SaleId:
                            newValue = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            TextToBeModified = Convert.ToString(newValue + 1);
                            ApplicationSettings.Remove(ApplicationSettings[document.LineIndex]);
                            ApplicationSettings.Insert(document.LineIndex, TextToBeModified);
                            break;
                        case name.SalesPersonId:
                            newValue = Convert.ToInt32(ApplicationSettings[document.LineIndex]);
                            TextToBeModified = Convert.ToString(newValue + 1);
                            ApplicationSettings.Remove(ApplicationSettings[document.LineIndex]);
                            ApplicationSettings.Insert(document.LineIndex, TextToBeModified);
                            break;
                        case name.EncryptionLevel:
                            TextToBeModified = Convert.ToString(setLevel);
                            ApplicationSettings.Remove(ApplicationSettings[document.LineIndex]);
                            ApplicationSettings.Insert(document.LineIndex, TextToBeModified);
                            break;
                        case name.Extension:
                            TextToBeModified = extensionFormat;
                            ApplicationSettings.Remove(ApplicationSettings[document.LineIndex]);
                            ApplicationSettings.Insert(document.LineIndex, TextToBeModified);
                            break;
                        default:
                            break;
                    }
                }
            }

            FileStream stream = new FileStream("AppSettings.dat", FileMode.Create, FileAccess.Write);

            BinaryFormatter formatter = new BinaryFormatter();

            foreach (string line in ApplicationSettings)
            {
                formatter.Serialize(stream, line);
                Console.WriteLine(line);
            }
            stream.Close();
        }

        public static void SettingAppEncryptionAndExtension()
        {
            LevelOfAppEncryption = ReadId(name.EncryptionLevel);
            TextTrue_BinFalse = ReadExtension(name.Extension);
        }

        static int SetOfSelectedIndex()
        {
            if (TextTrue_BinFalse)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public Codec()
        {
            InitializeComponent();

            CreateDisplayItem();

            Start();
        }

    }
}
