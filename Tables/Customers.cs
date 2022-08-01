using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Car_Dealership
{
    [Serializable]
    public class Customers
    {
        public static string FileName = "Customer ";

        public int CustomerId { get; set; }

        public string Sex { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Street { get; set; }

        public int PostalCcode { get; set; }

        public string Location { get; set; }

        public string Country { get; set; }

        public decimal TelNr { get; set; }

        public string Email { get; set; }


        // Writing user input Creating a new record
        public static void WriteAddTextOrBin(Customer customer, int LevelOfAppEncryption, bool TextTrue_BinFalse)
        {
            if (TextTrue_BinFalse)
            {
                StreamWriter writer = new StreamWriter(FileName + LevelOfAppEncryption + " level.csv", true);
                string line =
                $"{Convert.ToString(customer.Number).Encrypt(LevelOfAppEncryption)};" +
                $"{customer.Sex.Encrypt(LevelOfAppEncryption)};" +
                $"{customer.LastName.Encrypt(LevelOfAppEncryption)};" +
                $"{customer.FirstName.Encrypt(LevelOfAppEncryption)};" +
                $"{customer.Street.Encrypt(LevelOfAppEncryption)};" +
                $"{Convert.ToString(customer.PostalCcode).Encrypt(LevelOfAppEncryption)};" +
                $"{customer.Location.Encrypt(LevelOfAppEncryption)};" +
                $"{customer.Country.Encrypt(LevelOfAppEncryption)};" +
                $"{Convert.ToString(customer.TelNr).Encrypt(LevelOfAppEncryption)};" +
                $"{customer.Email.Encrypt(LevelOfAppEncryption)}";
                writer.WriteLine(line);
                writer.Close();
            }

            else
            {
                FileStream stream = new FileStream(FileName + LevelOfAppEncryption + " level.dat", FileMode.Append, FileAccess.Write);

                BinaryFormatter formatter = new BinaryFormatter();

                CostumersBinary customerBinary = new CostumersBinary()
                {
                    CostumerId = Convert.ToString(customer.Number).Encrypt(LevelOfAppEncryption),
                    Sex = Convert.ToString(customer.Sex).Encrypt(LevelOfAppEncryption),
                    LastName = Convert.ToString(customer.LastName).Encrypt(LevelOfAppEncryption),
                    FirstName = Convert.ToString(customer.FirstName).Encrypt(LevelOfAppEncryption),
                    Street = Convert.ToString(customer.Street).Encrypt(LevelOfAppEncryption),
                    PostalCcode = Convert.ToString(customer.PostalCcode).Encrypt(LevelOfAppEncryption),
                    Location = Convert.ToString(customer.Location).Encrypt(LevelOfAppEncryption),
                    Country = Convert.ToString(customer.Country).Encrypt(LevelOfAppEncryption),
                    TelNr = Convert.ToString(customer.TelNr).Encrypt(LevelOfAppEncryption),
                    Email = Convert.ToString(customer.Email).Encrypt(LevelOfAppEncryption)
                };

                formatter.Serialize(stream, customerBinary);
                stream.Close();
            }
        }

        // Reading List 
        public static List<Customers> ReadListTextOrBin(int LevelOfAppEncryption, bool TextTrue_BinFalse)
        {
            if (TextTrue_BinFalse)
            {
                return ReadFromText(LevelOfAppEncryption);
            }

            else
            {
                return ReadFromBin(LevelOfAppEncryption);
            }
        }

        // Writing from List to TextList
        static void WriteListToText(List<Customers> customers, int LevelOfEncrypt)
        {
            StreamWriter writer = new StreamWriter(FileName + LevelOfEncrypt + " level.csv");

            foreach (Customers customer in customers)
            {
                string lines = $"{Convert.ToString(customer.CustomerId).Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Sex.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.LastName.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.FirstName.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Street.Encrypt(LevelOfEncrypt)};" +
                    $"{Convert.ToString(customer.PostalCcode).Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Location.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Country.Encrypt(LevelOfEncrypt)};" +
                    $"{Convert.ToString(customer.TelNr).Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Email.Encrypt(LevelOfEncrypt)}";
                writer.WriteLine(lines);
            }
            writer.Close();
        }

        // Writing from List to BinaryList
        static void WriteListToBin(List<Customers> customers, int LevelOfEncrypt)
        {
            FileStream stream = new FileStream(FileName + LevelOfEncrypt + " level.dat", FileMode.Append, FileAccess.Write);

            BinaryFormatter formatter = new BinaryFormatter();

            foreach (Customers customer in customers)
            {
                CostumersBinary customerBinary = new CostumersBinary()
                {
                    CostumerId = Convert.ToString(customer.CustomerId).Encrypt(LevelOfEncrypt),
                    Sex = Convert.ToString(customer.Sex).Encrypt(LevelOfEncrypt),
                    LastName = Convert.ToString(customer.LastName).Encrypt(LevelOfEncrypt),
                    FirstName = Convert.ToString(customer.FirstName).Encrypt(LevelOfEncrypt),
                    Street = Convert.ToString(customer.Street).Encrypt(LevelOfEncrypt),
                    PostalCcode = Convert.ToString(customer.PostalCcode).Encrypt(LevelOfEncrypt),
                    Location = Convert.ToString(customer.Location).Encrypt(LevelOfEncrypt),
                    Country = Convert.ToString(customer.Country).Encrypt(LevelOfEncrypt),
                    TelNr = Convert.ToString(customer.TelNr).Encrypt(LevelOfEncrypt),
                    Email = Convert.ToString(customer.Email).Encrypt(LevelOfEncrypt)
                };
                formatter.Serialize(stream, customerBinary);
            }
            stream.Close();
        }

        public static void WriteListTextOrBin(List<Customers> customers, int LevelOfAppEncryption, bool TextTrue_BinFalse)
        {
            if (TextTrue_BinFalse)
            {
                WriteListToText(customers, LevelOfAppEncryption);
            }

            else
            {
                WriteListToBin(customers, LevelOfAppEncryption);
            }
        }


        static List<Customers> ReadFromText(int LevelOfDecrypt)
        {
            List<Customers> customers = new List<Customers>();

            if (File.Exists(FileName + LevelOfDecrypt + " level.csv"))
            {
                StreamReader reader = new StreamReader(FileName + LevelOfDecrypt + " level.csv", Encoding.Default);
                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] record = line.Split(';');
                    int costumerId = Convert.ToInt32(record[0].Decrypt(LevelOfDecrypt));
                    string sex = record[1].Decrypt(LevelOfDecrypt);
                    string lastName = record[2].Decrypt(LevelOfDecrypt);
                    string firstName = record[3].Decrypt(LevelOfDecrypt);
                    string street = record[4].Decrypt(LevelOfDecrypt);
                    int postalCcode = Convert.ToInt32(record[5].Decrypt(LevelOfDecrypt));
                    string location = record[6].Decrypt(LevelOfDecrypt);
                    string country = record[7].Decrypt(LevelOfDecrypt);
                    decimal telNr = Convert.ToDecimal(record[8].Decrypt(LevelOfDecrypt));
                    string email = record[9].Decrypt(LevelOfDecrypt);
                    customers.Add(new Customers
                    {
                        CustomerId = costumerId,
                        Sex = sex,
                        LastName = lastName,
                        FirstName = firstName,
                        Street = street,
                        PostalCcode = postalCcode,
                        Location = location,
                        Country = country,
                        TelNr = telNr,
                        Email = email
                    });
                    line = reader.ReadLine();
                }
                reader.Close();
            }

            return customers;
        }

        static List<Customers> ReadFromBin(int LevelOfDecrypt)
        {
            List<Customers> customers = new List<Customers>();
            if (File.Exists(FileName + LevelOfDecrypt + " level.dat"))
            {
                FileStream stream = new FileStream(FileName + LevelOfDecrypt + " level.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                while (stream.Position != stream.Length)
                {
                    CostumersBinary customersBinary = (CostumersBinary)formatter.Deserialize(stream);

                    int customerId = Convert.ToInt32(customersBinary.CostumerId.Decrypt(LevelOfDecrypt));
                    string sex = customersBinary.Sex.Decrypt(LevelOfDecrypt);
                    string lastName = customersBinary.LastName.Decrypt(LevelOfDecrypt);
                    string firstName = customersBinary.FirstName.Decrypt(LevelOfDecrypt);
                    string street = customersBinary.Street.Decrypt(LevelOfDecrypt);
                    int postalCcode = Convert.ToInt32(customersBinary.PostalCcode.Decrypt(LevelOfDecrypt));
                    string location = customersBinary.Location.Decrypt(LevelOfDecrypt);
                    string country = customersBinary.Country.Decrypt(LevelOfDecrypt);
                    decimal telNr = Convert.ToDecimal(customersBinary.TelNr.Decrypt(LevelOfDecrypt));
                    string email = customersBinary.Email.Decrypt(LevelOfDecrypt);
                    customers.Add(new Customers
                    {
                        CustomerId = customerId,
                        Sex = sex,
                        LastName = lastName,
                        FirstName = firstName,
                        Street = street,
                        PostalCcode = postalCcode,
                        Location = location,
                        Country = country,
                        TelNr = telNr,
                        Email = email
                    });
                }
                stream.Close();
            }
            return customers;
        }


        // WriteListToText + DateTime.Now
        static void WriteToText(List<Customers> custuomers, int LevelOfEncrypt)
        {
            string date = DateTime.Now.ToString().Replace(':', '-');

            StreamWriter writer = new StreamWriter(date + " " + FileName + LevelOfEncrypt + " level.csv");

            foreach (Customers customer in custuomers)
            {
                string lines =
                    $"{Convert.ToString(customer.CustomerId).Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Sex.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.LastName.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.FirstName.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Street.Encrypt(LevelOfEncrypt)};" +
                    $"{Convert.ToString(customer.PostalCcode).Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Location.Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Country.Encrypt(LevelOfEncrypt)};" +
                    $"{Convert.ToString(customer.TelNr).Encrypt(LevelOfEncrypt)};" +
                    $"{customer.Email.Encrypt(LevelOfEncrypt)}";
                writer.WriteLine(lines);
            }
            writer.Close();
        }

        //  WriteListToBin + DateTime.Now
        static void WriteToBin(List<Customers> customers, int LevelOfEncrypt)
        {
            string date = DateTime.Now.ToString().Replace(':', '-');

            FileStream stream = new FileStream(date + " " + FileName + LevelOfEncrypt + " level.dat", FileMode.Append, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();

            foreach (Customers customer in customers)
            {
                CostumersBinary customerBinary = new CostumersBinary()
                {
                    CostumerId = Convert.ToString(customer.CustomerId).Encrypt(LevelOfEncrypt),
                    Sex = Convert.ToString(customer.Sex).Encrypt(LevelOfEncrypt),
                    LastName = Convert.ToString(customer.LastName).Encrypt(LevelOfEncrypt),
                    FirstName = Convert.ToString(customer.FirstName).Encrypt(LevelOfEncrypt),
                    Street = Convert.ToString(customer.Street).Encrypt(LevelOfEncrypt),
                    PostalCcode = Convert.ToString(customer.PostalCcode).Encrypt(LevelOfEncrypt),
                    Location = Convert.ToString(customer.Location).Encrypt(LevelOfEncrypt),
                    Country = Convert.ToString(customer.Country).Encrypt(LevelOfEncrypt),
                    TelNr = Convert.ToString(customer.TelNr).Encrypt(LevelOfEncrypt),
                    Email = Convert.ToString(customer.Email).Encrypt(LevelOfEncrypt)
                };
                formatter.Serialize(stream, customerBinary);
            }
            stream.Close();
        }


        // Conversion to Codec Class
        public static void EncryptionTextToText(int LevelOfSource, int LevelOfTarget)
        {
            List<Customers> customers = ReadFromText(LevelOfSource);
            if (customers.Count > 0)
            {
                WriteToText(customers, LevelOfTarget);
            }
            else
            {
                MessageBox.Show("Bitte kopieren Sie " + FileName + LevelOfSource + " level.csv in den Ordner");
            }
        }

        public static void EncryptionTextToBin(int LevelOfSource, int LevelOfTarget)
        {
            List<Customers> customers = ReadFromText(LevelOfSource);

            if (customers.Count > 0)
            {
                WriteToBin(customers, LevelOfTarget);
            }
            else
            {
                MessageBox.Show("Bitte kopieren Sie " + FileName + LevelOfSource + " level.csv in den Ordner");
            }
        }

        public static void EncryptionBinToBin(int LevelOfSource, int LevelOfTarget)
        {
            List<Customers> customers = ReadFromBin(LevelOfSource);
            if (customers.Count > 0)
            {
                WriteToBin(customers, LevelOfTarget);
            }
            else
            {
                MessageBox.Show("Bitte kopieren Sie " + FileName + LevelOfSource + " level.csv in den Ordner");
            }
        }

        public static void EncryptionBinToText(int LevelOfSource, int LevelOfTarget)
        {
            List<Customers> customers = ReadFromBin(LevelOfSource);

            if (customers.Count > 0)
            {
                WriteToText(customers, LevelOfTarget);
            }
            else
            {
                MessageBox.Show("Bitte kopieren Sie " + FileName + LevelOfTarget + " level.dat in den Ordner");
            }
        }

        [Serializable]
        class CostumersBinary
        {
            public string CostumerId { get; set; }

            public string Sex { get; set; }

            public string LastName { get; set; }

            public string FirstName { get; set; }

            public string Street { get; set; }

            public string PostalCcode { get; set; }

            public string Location { get; set; }

            public string Country { get; set; }

            public string TelNr { get; set; }

            public string Email { get; set; }
        }

        [Serializable]
        public class Customer
        {
            private static int _customerId = 0;

            private string _number;

            private string _sex;

            private string _lastName;

            private string _firstName;

            private string _street;

            private int _postalCcode;

            private string _location;

            private string _country;

            private decimal _telNr;

            private string _email;

            public Customer(string sex, string lastName, string firstName, string street,
                int postalCcode, string location, string country, decimal telNr, string email)
            {
                Codec.AppSettingsModifies(Codec.name.CustomerId);
                CostumerId = Codec.ReadId(Codec.name.CustomerId);
                this._number = CostumerId.ToString();
                Sex = sex;
                LastName = lastName;
                FirstName = firstName;
                Street = street;
                PostalCcode = postalCcode;
                Location = location;
                Country = country;
                TelNr = telNr;
                Email = email;
            }

            public static int CostumerId { get => _customerId; set => _customerId = value; }

            public string Number { get => _number; set => _number = value; }

            public string Sex { get => _sex; set => _sex = value; }

            public string LastName { get => _lastName; set => _lastName = value; }

            public string FirstName { get => _firstName; set => _firstName = value; }

            public string Street { get => _street; set => _street = value; }

            public int PostalCcode { get => _postalCcode; set => _postalCcode = value; }

            public string Location { get => _location; set => _location = value; }

            public string Country { get => _country; set => _country = value; }

            public decimal TelNr { get => _telNr; set => _telNr = value; }

            public string Email { get => _email; set => _email = value; }
        }
    }
}