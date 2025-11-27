using FitnessTrackerTask;

namespace FitnessTrackerTask
{
    abstract class FitnessTracker
    {
        public string DeviceName { get; set; }
        public string CurrentAccount { get; private set; }
        protected int AccountIndex { get; set; } = 0;
        public string[] Accounts { get; set; }

        public int BatteryLife { get; set; }
        public int TotalSteps { get; set; }

         protected FitnessTracker(string deviceName)
        {
            DeviceName = deviceName;
        }




        public abstract void AddAccount(string accountName);

        public void PrintAllAccounts()
        {
            if (AccountIndex == 0)
            {
                Console.WriteLine("No user accounts to display");

                return;
            }

            for (int i = 0; i < AccountIndex; i++)
            {
                Console.WriteLine(Accounts[i]);
            }
        }

        public void PrintInfo()
        {
            Console.WriteLine($"FitTrack ’{DeviceName}’{AccountIndex} user accounts.");
            Console.WriteLine($"Current user account:{CurrentAccount}");
        }

        public void SetCurrentAccount(string currentAccount)
        {
            CurrentAccount = currentAccount;
        }

        public void DeleteAccount(string accountName)
        {

           
            for (int i = 0; i < AccountIndex; i++)
            {
                if (Accounts[i] == accountName)
                {
                    for (int j = i; j < AccountIndex - 1; j++)
                    {
                        Accounts[j] = Accounts[j + 1];
                    }
                    Accounts[AccountIndex - 1] = default;
                    AccountIndex--;
                    Console.WriteLine($"Account ’{accountName}’ was deleted");
                    return;
                }
            }

        }
    }

    class FitTrack : FitnessTracker
    {
        public FitTrack(string deviceName) : base(deviceName)
        {

        }
        public override void AddAccount(string accountName)
        {
            if (AccountIndex >= Accounts.Length)
            {
                Console.WriteLine("You cannot add more accounts, limit is reached");

                return;
            }

            Accounts[AccountIndex++] = accountName;
            Console.WriteLine($"Account ’{accountName}’ was added");
        }
    }
}



class FitTrackPro : FitnessTracker
{

    public FitTrackPro(string deviceName) : base(deviceName)
    {
       
    }

    public override void AddAccount(string accountName)
    {




    }
}