/*
Test Developer

Buatlah Object Oriented Program yang dapat menjalankan fungsi-fungsi sbb:
1. Terdapat 1 buah mesin vending machine
2. Pada mesin tersebut tersedia 5 buah jenis makanan: Biskuit, Chips, Oreo, Tango, Cokelat
3. Price list untuk tiap item sbb:

Biskuit: 6000
Chips: 8000
Oreo: 10000
Tango: 12000
Cokelat: 15000
4. Vending machine dapat menerima uang dengan pecahan : 2000, 5000, 10000, 20000, 50000.
5. Vending Machine dapat melakukan pembelian, pengembalian uang dan mendeteksi jika ada makanan yang stok-nya sedang habis.

*/

using System.Text.RegularExpressions;

namespace VendingMachineApp
{
    internal class Program
    {


        static void Main(string[] args)
        {

            var acceptedBankNotes = new string[] { "2000", "5000", "10000", "20000", "50000" };

            var foodContainer = new List<Food>
            {
                new Food() { ProductCodeId=220, Name = "Biskuit", Price = 6000 },
                new Food() { ProductCodeId=220, Name = "Biskuit", Price = 6000 },
                new Food() { ProductCodeId=220, Name = "Biskuit", Price = 6000 },
                new Food() { ProductCodeId=220, Name = "Biskuit", Price = 6000 },
                new Food() { ProductCodeId=220, Name = "Biskuit", Price = 6000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=112, Name = "Chips", Price = 8000 },
                new Food() { ProductCodeId=444, Name = "Oreo", Price = 10000 },
                new Food() { ProductCodeId=444, Name = "Oreo", Price = 10000 },
                new Food() { ProductCodeId=768, Name = "Tango", Price = 12000 },
                new Food() { ProductCodeId=941, Name = "Cokelat", Price = 15000 }
            };

            VendingMachine _vendingMachine = new VendingMachine(acceptedBankNotes, foodContainer);

            Console.WriteLine("Vending Machine Toko Firman is Running");
            Console.WriteLine();
            Console.WriteLine("Press ENTER to Buy Food");

            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                _vendingMachine.OrderFood();

                if (_vendingMachine.PurchasedFood == null || _vendingMachine.PurchasedFoodNumber < 1) return;              

                Console.WriteLine("Press ENTER to Confirm and make payment");

                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    var hasPaid = _vendingMachine.PayPurchase();
                    if (hasPaid) 
                    {
                        Console.WriteLine();
                        Console.WriteLine("Payment Complete, Please take your food from Pickup Box Below");
                        Console.WriteLine();
                        _vendingMachine.SendFood();
                        _vendingMachine.SendMoneyChange();
                        Console.WriteLine();
                        Console.WriteLine("Thank you - Enjoy the food");
                    }        
                }                
            }

            while (Console.ReadKey().Key != ConsoleKey.Enter) 
            {
                Console.WriteLine("Press ENTER to Exit");
            }    

        }
    }

    internal class VendingMachine {

        public List<List<Food>>? FoodList { get; set; }

        private double _totalAmount = 0;

        private Food? _foodToPurchase;
        public Food? PurchasedFood  { get { return _foodToPurchase; } }

        private Regex _regex = new Regex(@"-?\d+(?:\.\d+)?");

        private int _foodNumberToPurchase = 0;
        public int PurchasedFoodNumber { get { return _foodNumberToPurchase; } }

        private List<Food> _foodContainer = new();

        private IDictionary<string, double> _moneyStock = new System.Collections.Generic.Dictionary<string, double>();

        private string[] _acceptedBankNotes;

        private double _totalAmountPaid = 0;


        public VendingMachine(string[] acceptedBankNotes, List<Food>? foodContainer = null)
        {

            _acceptedBankNotes = acceptedBankNotes;

            if (foodContainer != null)
            {
                _foodContainer = foodContainer;
            }

            foreach (var bankNotes in acceptedBankNotes)
            {
                _moneyStock.Add(bankNotes, 0);
            }

            if (_foodContainer.Count > 0)
            {
                GenerateFoodList();
            }
        }

        public bool PayPurchase()
        {
            bool payComplete = false;

            while (_totalAmountPaid < _totalAmount)
            {
                Console.WriteLine();
                Console.WriteLine("Please Insert Money (Insert Number to Simulate eg. 500000), Press ENTER to Confirm");
                ShowAccpetedbanknotes();
                Console.WriteLine();
                var moneyAmountString = Console.ReadLine();

                if (moneyAmountString == null) break;

                if (!_regex.IsMatch(moneyAmountString))
                {
                    Console.WriteLine("Money Not Recognized, Purchase Cancelled");
                    Console.WriteLine("Press ENTER to Exit");
                    break;
                }

                if (!Checkdenomination(moneyAmountString))
                {
                    Console.WriteLine();
                    Console.WriteLine("Money Denomination Not Accepted");
                }
                else 
                {
                    _totalAmountPaid += Double.Parse(moneyAmountString);

                    if (_totalAmountPaid < _totalAmount)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Money not enouh");
                    }
                    else
                    {
                        payComplete = true;
                    }
                }              
            }

            return payComplete;
        }

        public void OrderFood() {

            if (FoodList != null)
            {

                ShowAvailableFood();

                Console.WriteLine();
                Console.WriteLine("Insert Food Code To Purchase and Press ENTER");
                Console.WriteLine();

                var selectedFoodId = Console.ReadLine();

                if (selectedFoodId!=null && FoodList.Any(c => c.Any(f => f.ProductCodeId.ToString() == selectedFoodId)))
                {

                    SelectFood(int.Parse(selectedFoodId));

                    if (PurchasedFood == null) return;
     
                    Console.WriteLine();
                    Console.WriteLine("Insert Food Number Purchase (1,2,3) and Press ENTER");
                    Console.WriteLine();

                    var foodNumberString = Console.ReadLine();
                    Console.WriteLine();

                    if (foodNumberString==null) return;
          

                    if (!_regex.IsMatch(foodNumberString))
                    {
                        Console.WriteLine("Only can insert Number");
                        return;
                    }

                    _foodNumberToPurchase = int.Parse(foodNumberString);

                    if (!CheckFoodStockAvailability())
                    {
                        Console.WriteLine("Food Stock Not Enough");
                        _foodToPurchase = null;
                        _foodNumberToPurchase = 0;

                        return;
                    }                    


                    _totalAmount = PurchasedFood.Price * _foodNumberToPurchase;
                    ShowPurchaseDetail();

                }
                else
                {
                    Console.WriteLine($"Product Code Wrong or Not Available");
                }

            }
            else
            {
                Console.WriteLine("Sorry, No Food Available To Purchase Right Now");

            }

        }

        private bool CheckFoodStockAvailability()
        {
            var foodStock = _foodContainer.Where(c => c.ProductCodeId == _foodToPurchase.ProductCodeId).Count();
            
            if (foodStock<this.PurchasedFoodNumber)
                return false;
            
            return true;
        }

        private void ShowAvailableFood()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Available Food To Purchase");
            Console.WriteLine("======================================");

            foreach (var foodgroup in FoodList)
            {
                Console.WriteLine($"Code: {foodgroup.First().ProductCodeId} - {foodgroup.First().Name} - Rp.{foodgroup.First().Price} -  Available: {foodgroup.Count}");
            }
        }

        private void ShowPurchaseDetail()
        {
            Console.WriteLine("======================================");
            Console.WriteLine($"Purchase Detail:");
            Console.WriteLine("======================================");
            Console.WriteLine($"Food Name:{PurchasedFood.Name}");
            Console.WriteLine($"Food Number:{_foodNumberToPurchase}");
            Console.WriteLine($"Total Payment:{_totalAmount}");
            Console.WriteLine("======================================");
            Console.WriteLine();
        }

        private void ShowAccpetedbanknotes()
        {
            string banknotesString = "Vending Machine Only Accepted: ";
            foreach (var banknotes in _acceptedBankNotes)
            {
                banknotesString += $"Rp.{banknotes},";
            }
            Console.WriteLine();
            Console.WriteLine(banknotesString);
        }



        public string AddFood(Food food)
        {
            try
            {
                _foodContainer.Add(food);
                return ($"{food.Name} Added to VendingMachine Successfully");
            }
            catch (Exception ex)
            {
                return ($"{food.Name} Has Failed Added to VendingMachine");
            }

        }

        private void GenerateFoodList() {

            var foodList = _foodContainer
            .GroupBy(u => u.ProductCodeId)
            .Select(grp => grp.ToList())
            .ToList();

            this.FoodList = foodList;
        }
        private void SelectFood(int foodCode)
        {
            _foodToPurchase = _foodContainer.First(c => c.ProductCodeId == foodCode);
        }

        private bool Checkdenomination(string moneyAmount)
        {
            if (_acceptedBankNotes.Contains(moneyAmount)) return true;
            return false;

        }

        internal void SendFood()
        {

            for (int i = 0; i < _foodNumberToPurchase; i++)
            {
                Console.WriteLine("--Send Food to Pickup Box--");
                Console.WriteLine();
                Task.Delay(1000).Wait();

            } 
        }

        internal void SendMoneyChange()
        {
            var diff = _totalAmountPaid - _totalAmount;
            if (diff>0)
            {
                Console.WriteLine("Take Your change on Coin Changer");
                Console.WriteLine();
                Task.Delay(1000).Wait();
                Console.WriteLine($"--Send Rp.{ diff }--");
            }
        }
    }


    internal class Food {
        public int ProductCodeId { get; set; }
        public string Name { get; set; } = String.Empty;
        public double Price { get; set; }

    }


}