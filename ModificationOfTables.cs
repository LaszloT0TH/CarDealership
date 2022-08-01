using InputForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Car_Dealership
{
    public partial class ModificationOfTables : Form
    {
        InputForm form;

        List<Cars> cars;

        List<Customers> customers;

        List<Sales> sales;

        List<SalesPersons> salesPersons;

        List<SalesPersons_Secret_Data> salespersons_Secret_Datas;

        ProgressBar gridChangesProgressBar;

        const string DropDownMenuCars = "Cars";

        const string DropDownMenuCustomers = "Customers";

        const string DropDownMenuSales = "Sales";

        const string DropDownMenuSalesPersons = "SalesPersons";

        const string DropDownMenuSalesPersons_Secret_Data = "SalesPersons_Secret_Data";

        enum TablesListName
        {
            cars,
            customers, 
            sales, 
            salesPersons,
            salespersons_Secret_Datas
        }

        private Task DeleteFromTheList(TablesListName name)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (int item in GridChanges.UserSelectedIndexList)
                {
                    switch (name)
                    {
                        case TablesListName.cars:
                            {
                                cars.RemoveAt(item);
                                break;
                            }
                        case TablesListName.customers:
                            {
                                customers.RemoveAt(item);
                                break;
                            }
                        case TablesListName.sales:
                            {
                                sales.RemoveAt(item);
                                break;
                            }
                        case TablesListName.salesPersons:
                            {
                                salesPersons.RemoveAt(item);
                                break;
                            }
                        case TablesListName.salespersons_Secret_Datas:
                            {
                                salespersons_Secret_Datas.RemoveAt(item);
                                break;
                            }
                        default:
                            break;
                    }
                }
            });
        }

        private Task ResetTheList(TablesListName name)
        {
            return Task.Factory.StartNew(() =>
            {
                int Count = (GridChanges.dataGridViewRowsCount - 1);
                for (; 0 <= Count; Count--)
                {
                    switch (name)
                    {
                        case TablesListName.cars:
                            {
                                cars.RemoveAt(Count);
                                break;
                            }
                        case TablesListName.customers:
                            {
                                customers.RemoveAt(Count);
                                break;
                            }
                        case TablesListName.sales:
                            {
                                sales.RemoveAt(Count);
                                break;
                            }
                        case TablesListName.salesPersons:
                            {
                                salesPersons.RemoveAt(Count);
                                break;
                            }
                        case TablesListName.salespersons_Secret_Datas:
                            {
                                salespersons_Secret_Datas.RemoveAt(Count);
                                break;
                            }
                        default:
                            break;
                    }
                }
            });
        }

        private Task GridProgressbar()
        {
            return (Task.Factory.StartNew(() =>
            {
                int ProgBarMax = (int)(GridChanges.dataGridViewRowsCount * 1.8);

                gridChangesProgressBar.Invoke(() => gridChangesProgressBar.Visible = true);
                gridChangesProgressBar.Invoke(() => gridChangesProgressBar.Value = 0);
                gridChangesProgressBar.Invoke(() => gridChangesProgressBar.Maximum = ProgBarMax);

                for (int i = 0; i < (ProgBarMax); i++)
                {
                    if (gridChangesProgressBar.Value >= ProgBarMax)
                    {
                        gridChangesProgressBar.Invoke(() => gridChangesProgressBar.Value--);
                    }
                    gridChangesProgressBar.Invoke(() => gridChangesProgressBar.Value++);
                }
            }));
        }

        private async void Delete(TablesListName name)
        {
            GridChanges.Delete();
            await DeleteFromTheList(name);
        }

        private async void Reset(TablesListName name)
        {
            GridChanges.Reset();
            await ResetTheList(name);
            await GridProgressbar();
            gridChangesProgressBar.Invoke(() => gridChangesProgressBar.Visible = false);
        }

        private void Start()
        {
            form = new InputForm(this);
            form.Add("Tables", new InputSelect("Tabellenname:", new string[] {
                DropDownMenuCars,
                DropDownMenuCustomers,
                DropDownMenuSales,
                DropDownMenuSalesPersons,
                DropDownMenuSalesPersons_Secret_Data
            }).SetSize(250))
                .MoveTo(10, 10)
                .SetButtonZero("OK")
                .SetButtonFirst("Menü", Visible = true)
                .SetButtonSecond()
                .SetButtonThird()
                .OnSubmitZero(() =>
                {
                    switch (form["Tables"])
                    {
                        case DropDownMenuCars:
                            form.Visible = false;
                            CarTable();
                            break;
                        case DropDownMenuCustomers:
                            form.Visible = false;
                            CustomerTable();
                            break;
                        case DropDownMenuSales:
                            form.Visible = false;
                            SaleTable();
                            break;
                        case DropDownMenuSalesPersons:
                            form.Visible = false;
                            SalesPersonsTable();
                            break;
                        case DropDownMenuSalesPersons_Secret_Data:
                            form.Visible = false;
                            SalesPersons_Secret_DataTable();
                            break;
                        default:
                            break;
                    }
                })
                .OnSubmitFirst(() =>
                {
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }

        private void CarTable()
        {
            object[] rows = new object[cars.Count];
            int counter = 0;
            foreach (Cars car in cars)
            {
                string[] row = new string[]
                {
                    Convert.ToString(car.CarNumber),
                    car.Model,
                    car.Color,
                    Convert.ToString(car.Number_of_seats),
                    Convert.ToString(car.Cubic_capacity),
                    Convert.ToString(car.Mileage),
                    Convert.ToString(car.Year_of_production),
                    car.Chassis_number,
                    Convert.ToString(car.Engine_power),
                    car.Gearbox,
                    car.Fuel,
                    Convert.ToString(car.Own_Weight),
                    Convert.ToString(car.Sold)
                };
                rows[counter++] = row;
            }
            form = new InputForm(this);
            form.Add("Grid", new DisplayGrid(DropDownMenuCars, new string[] {
                        "Auto Id",
                        "Auto Modell",
                        "Auto Farbe",
                        "Auto Anzahl der Sitzplätze",
                        "Auto Hubraum cm3",
                        "Auto Fahrleistung KM",
                        "Auto Baujahr",
                        "Auto Fahrgestellnummer",
                        "Auto Motorleistung KW",
                        "Auto Getriebe",
                        "Auto Brennstoff",
                        "Auto Eigengewicht kg",
                        "Verkauft"
                    })
                .AddGridRows(rows))
                .MoveTo(10, 10)
                .SetButtonPosition(1170)
                .SetButtonZero("Löschen")
                .SetButtonFirst("Zurücksetzen", Visible = true)
                .SetButtonSecond("Drücken", Visible = true)
                .SetButtonThird("Speichern, Menü", Visible = true)
                .OnSubmitZero(() =>
                {
                    Delete(TablesListName.cars);
                    MessageBox.Show("Die Zeile wurde gelöscht");
                })
                .OnSubmitFirst(() =>
                {
                    Reset(TablesListName.cars);
                    MessageBox.Show("Alle Daten wurden gelöscht");
                })
                .OnSubmitSecond(() =>
                {
                    Print.Printing();
                })
                .OnSubmitThird(() =>
                {
                    Cars.WriteListTextOrBin(cars, Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);
                    MessageBox.Show("Alle Daten wurden gespeichert");
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }
        
        private void CustomerTable()
        {
            object[] rows = new object[customers.Count];
            int counter = 0;
            foreach (Customers customer in customers)
            {
                string[] row = new string[]
                {
                    Convert.ToString(customer.CustomerId),
                    customer.Sex,
                    customer.LastName,
                    customer.FirstName,
                    customer.Street,
                    Convert.ToString(customer.PostalCcode),
                    customer.Location,
                    customer.Country,
                    Convert.ToString(customer.TelNr),
                    customer.Email
                };
                rows[counter++] = row;
            }
            form = new InputForm(this);
            form.Add("Grid", new DisplayGrid(DropDownMenuCustomers, new string[] {
                    "Kunde Nummer",
                    "Kunde Geschlecht",
                    "Kunde Familiename",
                    "Kunde Vorname",
                    "Kunde Adresse",
                    "Kunde Postleitzahl",
                    "Kunde WohnOrt",
                    "Kunde Land",
                    "Kunde Telefonnummer",
                    "Kunde E-Mail-Adresse"
                })
                .AddGridRows(rows))
                .MoveTo(10, 10)
                .SetButtonPosition(1170)
                .SetButtonZero("Löschen")
                .SetButtonFirst("Zurücksetzen", Visible = true)
                .SetButtonSecond("Drücken", Visible = true)
                .SetButtonThird("Speichern, Menü", Visible = true)
                .OnSubmitZero(() =>
                {
                    Delete(TablesListName.customers);
                    MessageBox.Show("Die Zeile wurde gelöscht");
                })
                .OnSubmitFirst(() =>
                {
                    Reset(TablesListName.customers);
                    MessageBox.Show("Alle Daten wurden gelöscht");
                })
                .OnSubmitSecond(() =>
                {
                    Print.Printing();
                })
                .OnSubmitThird(() =>
                {
                    Customers.WriteListTextOrBin(customers, Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);
                    MessageBox.Show("Alle Daten wurden gespeichert");
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }

        private void SaleTable()
        {
            object[] rows = new object[sales.Count];
            int counter = 0;
            foreach (Sales sale in sales)
            {
                string[] row = new string[]
                {
                    Convert.ToString(sale._saleId),
                    Convert.ToString(sale._costumerId),
                    Convert.ToString(sale._salesId),
                    Convert.ToString(sale._carNumber),
                    Convert.ToString(sale._date)
            };
                rows[counter++] = row;
            }
            string saleId = "Verkauf Nummer";
            string costumerId = "Kunde Nummer";
            string salesId = "Verkaufer Nummer";
            string carNumber = "Auto Nummer";
            string date = "Datum";
            form = new InputForm(this);
            form.Add("Grid", new DisplayGrid(DropDownMenuCars, new string[] {
                        saleId,
                        costumerId,
                        salesId,
                        carNumber,
                        date
                })
                .AddGridRows(rows))
                .MoveTo(10, 10)
                .SetButtonPosition(1170)
                .SetButtonZero("Löschen")
                .SetButtonFirst("Zurücksetzen", Visible = true)
                .SetButtonSecond("Drücken", Visible = true)
                .SetButtonThird("Speichern, Menü", Visible = true)
                .OnSubmitZero(() =>
                {
                    Delete(TablesListName.sales);
                    MessageBox.Show("Die Zeile wurde gelöscht");
                })
                .OnSubmitFirst(() =>
                {
                    Reset(TablesListName.sales);
                    MessageBox.Show("Alle Daten wurden gelöscht");
                })
                .OnSubmitSecond(() =>
                {
                    Print.Printing();
                })
                .OnSubmitThird(() =>
                {
                        // Save Table
                    Sales.WriteListTextOrBin(sales, Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);
                    MessageBox.Show("Alle Daten wurden gespeichert");
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }

        private void SalesPersonsTable()
        {
            object[] rows = new object[salesPersons.Count];
            int counter = 0;
            foreach (SalesPersons salesPerson in salesPersons)
            {
                string[] row = new string[]
                {
                    Convert.ToString(salesPerson._salesId),
                    salesPerson._lastName,
                    salesPerson._firstName,
                    Convert.ToString(salesPerson._provision),
                    Convert.ToString(salesPerson._entry),
                    Convert.ToString(salesPerson._active)
                };
                rows[counter++] = row;
            }
            form = new InputForm(this);
            form.Add("Grid", new DisplayGrid(DropDownMenuSales, new string[] {
                        "Verkaufer Id",
                        "Verkaufer Familienname",
                        "Verkaufer Vorname",
                        "Verkaufer Provision in %",
                        "Verkaufer Eintrittsdatum",
                        "Verkaufer Aktiv"
                })
                .AddGridRows(rows))
                .MoveTo(10, 10)
                .SetButtonPosition(1170)
                .SetButtonZero("Löschen")
                .SetButtonFirst("Zurücksetzen", Visible = true)
                .SetButtonSecond("Drücken", Visible = true)
                .SetButtonThird("Speichern, Menü", Visible = true)
                .OnSubmitZero(() =>
                {
                    Delete(TablesListName.salesPersons);
                    MessageBox.Show("Die Zeile wurde gelöscht");
                })
                .OnSubmitFirst(() =>
                {
                    Reset(TablesListName.salesPersons);
                    MessageBox.Show("Alle Daten wurden gelöscht");
                })
                .OnSubmitSecond(() =>
                {
                    Print.Printing();
                })
                .OnSubmitThird(() =>
                {
                    SalesPersons.WriteListTextOrBin(salesPersons, Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);
                    MessageBox.Show("Alle Daten wurden gespeichert");
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }

        private void SalesPersons_Secret_DataTable()
        {
            object[] rows = new object[salespersons_Secret_Datas.Count];
            int counter = 0;
            foreach (SalesPersons_Secret_Data salespersons_Secret_Data in salespersons_Secret_Datas)
            {
                string[] row = new string[]
                {
                    Convert.ToString(salespersons_Secret_Data._salesId),
                    Convert.ToString(salespersons_Secret_Data._wage),
                    Convert.ToString(salespersons_Secret_Data._date_of_birth),
                    Convert.ToString(salespersons_Secret_Data._postalCcode),
                    Convert.ToString(salespersons_Secret_Data._location),
                    Convert.ToString(salespersons_Secret_Data._street)
                };
                rows[counter++] = row;
            }
            form = new InputForm(this);
            form.Add("Grid", new DisplayGrid(DropDownMenuSalesPersons, new string[] {
                    "Verkaufer Lohn in €",
                    "Verkaufer Geburtsdatum",
                    "Verkaufer Postleitzahl",
                    "Verkaufer WohnOrt",
                    "Verkaufer Adresse"
                })
                .AddGridRows(rows))
                .MoveTo(10, 10)
                .SetButtonPosition(1170)
                .SetButtonZero("Löschen")
                .SetButtonFirst("Zurücksetzen", Visible = true)
                .SetButtonSecond("Drücken", Visible = true)
                .SetButtonThird("Speichern, Menü", Visible = true)
                .OnSubmitZero(() =>
                {
                    Delete(TablesListName.salespersons_Secret_Datas);
                    MessageBox.Show("Die Zeile wurde gelöscht");
                })
                .OnSubmitFirst(() =>
                {
                    Reset(TablesListName.salespersons_Secret_Datas);
                    MessageBox.Show("Alle Daten wurden gelöscht");
                })
                .OnSubmitSecond(() =>
                {
                    Print.Printing();
                })
                .OnSubmitThird(() =>
                {
                    SalesPersons_Secret_Data.WriteListTextOrBin(salespersons_Secret_Datas, Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);
                    MessageBox.Show("Alle Daten wurden gespeichert");
                    Menu menu = new Menu();
                    this.Visible = false;
                    menu.Visible = false;
                    menu.ShowDialog();
                });
        }

        private void CreateList()
        {
            cars = Cars.ReadListTextOrBin(Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);

            customers = Customers.ReadListTextOrBin(Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);

            sales = Sales.ReadListTextOrBin(Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);

            salesPersons = SalesPersons.ReadListTextOrBin(Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);

            salespersons_Secret_Datas = SalesPersons_Secret_Data.ReadListTextOrBin(Codec.LevelOfAppEncryption, Codec.TextTrue_BinFalse);
        }

        private void CreatProgressBar()
        {

            gridChangesProgressBar = new ProgressBar();

            Controls.Add(gridChangesProgressBar);

            gridChangesProgressBar.Location = new Point(300, 200);

            gridChangesProgressBar.Size = new Size(650, 83);

            gridChangesProgressBar.TabIndex = 1;

            gridChangesProgressBar.Visible = false;
        }

        public ModificationOfTables()
        {
            InitializeComponent();

            CreateList();

            CreatProgressBar();

            Start();
        }
    }
}
