using FitnessTrackerTask;
using System.Diagnostics;

namespace FitnessTrackerTask
{
    abstract class FitnessTracker
    {
        public string DeviceName { get; private set; }
        public string CurrentAccount { get; private set; }
        public string[] Accounts { get; private set; }
        public int AccountIndex { get; protected set; } = 0;
        public Activity CurrentActivity { get; private set; }
        public int TotalSteps { get; protected set; } = 0;
        public int BatteryLife { get; protected set; }
        protected FitnessTracker(string deviceName, int maxUsers, int defaultBattery)
        {
            DeviceName = deviceName;
            Accounts = new string[maxUsers];
            BatteryLife = defaultBattery;
        }
        public virtual void AddAccount(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                Console.WriteLine("Invalid name. Name cannot be empty.");
                return;
            }

            if (AccountIndex >= Accounts.Length)
            {
                Console.WriteLine("You cannot add more accounts, limit reached.");
                return;
            }

            for (int i = 0; i < AccountIndex; i++)
            {
                if (Accounts[i] != null && Accounts[i].Equals(accountName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Account '{accountName}' already exists.");
                    return;
                }
            }

            Accounts[AccountIndex] = accountName;
            AccountIndex++;
            Console.WriteLine($"Account '{accountName}' added successfully.");
        }
        public void SetCurrentAccount(string currentAccount)
        {
            for (int i = 0; i < AccountIndex; i++)
            {
                if (Accounts[i] != null && Accounts[i].Equals(currentAccount, StringComparison.OrdinalIgnoreCase))
                {
                    CurrentAccount = currentAccount;
                    Console.WriteLine($"Current user set to: {currentAccount}");
                    return;
                }
            }
            Console.WriteLine($"Account '{currentAccount}' not found.");
        }
        public void DeleteAccount(string accountName)
        {
            int foundIndex = -1;

            for (int i = 0; i < AccountIndex; i++)
            {
                if (Accounts[i] != null && Accounts[i].Equals(accountName, StringComparison.OrdinalIgnoreCase))
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex != -1)
            {
                for (int j = foundIndex; j < AccountIndex - 1; j++)
                {
                    Accounts[j] = Accounts[j + 1];
                }
                Accounts[AccountIndex - 1] = null;
                AccountIndex--;

                Console.WriteLine($"Account '{accountName}' deleted.");

                if (CurrentAccount == accountName)
                {
                    CurrentAccount = null;
                    Console.WriteLine("Logged out from deleted account.");
                }
            }
            else
            {
                Console.WriteLine($"Account '{accountName}' not found.");
            }
        }
        public void PrintAllAccounts()
        {
            if (AccountIndex == 0)
            {
                Console.WriteLine("No accounts to display.");
                return;
            }

            Console.WriteLine("--- Registered Accounts ---");
            for (int i = 0; i < AccountIndex; i++)
            {
                Console.WriteLine($"- {Accounts[i]}");
            }
        }
        public virtual void TrackSteps(int steps)
        {
            const int minSteps = 1;
            const int maxSteps = 50000;

            if (steps >= minSteps && steps <= maxSteps)
            {
                TotalSteps += steps;
                Console.WriteLine($"Tracked {steps} steps. Total: {TotalSteps}.");
            }
            else
            {
                Console.WriteLine($"Invalid step count. Must be between {minSteps} and {maxSteps}.");
            }
        }
        public void PrintInfo()
        {
            Console.WriteLine("--- Device Info ---");
            Console.WriteLine($"Device Type: {GetType().Name}");
            Console.WriteLine($"Model: {DeviceName}");
            Console.WriteLine($"Battery Life: {BatteryLife} hours");
            Console.WriteLine($"Total Steps: {TotalSteps}");
            Console.WriteLine($"Accounts: {AccountIndex}/{Accounts.Length}");
            Console.WriteLine($"Current User: {CurrentAccount ?? "None"}");
            if (CurrentActivity != null)
            {
                Console.WriteLine($"Current Activity: {CurrentActivity.ActivityName} ({CurrentActivity.DurationInMinutes} min)");
            }
        }
        public void SetCurrentActivity(Activity activity)
        {
            CurrentActivity = activity;
            Console.WriteLine($"Activity '{activity.ActivityName}' started.");
        }
        public void StopActivity()
        {
            if (CurrentActivity != null)
            {
                Console.WriteLine($"Activity '{CurrentActivity.ActivityName}' finished. Calories burned: {CurrentActivity.CaloriesBurned:F2} kcal.");
                CurrentActivity = null;
            }
            else
            {
                Console.WriteLine("No activity currently running.");
            }
        }

        public abstract class Activity
        {
            public string ActivityName { get; private set; }
            public double DurationInMinutes { get; private set; }
            public double CaloriesBurned { get; private set; }

            protected Activity(string name, double duration)
            {
                if (duration <= 0)
                    throw new ArgumentException("Duration must be positive.");

                ActivityName = name;
                DurationInMinutes = duration;
                CaloriesBurned = CalculateCalories();
            }

            public abstract double CalculateCalories();

            public void DisplayActivityInfo()
            {
                Console.WriteLine($"Activity: {ActivityName} | Duration: {DurationInMinutes} min | Calories: {CaloriesBurned:F2} kcal");
            }
        }
        public class Running : Activity
        {
            private const double CaloriesPerMinute = 10.0;

            public Running(double duration) : base("Running", duration) { }

            public override double CalculateCalories()
            {
                return DurationInMinutes * CaloriesPerMinute;
            }
        }
        public class Cycling : Activity
        {
            private const double CaloriesPerMinute = 8.0;

            public Cycling(double duration) : base("Cycling", duration) { }

            public override double CalculateCalories()
            {
                return DurationInMinutes * CaloriesPerMinute;
            }
        }


        public class FitTrack : FitnessTracker
        {
            public FitTrack(string name) : base(name, maxUsers: 3, defaultBattery: 24) { }
        }

        public class FitTrackPro : FitnessTracker
        {
            public FitTrackPro(string name) : base(name, maxUsers: 5, defaultBattery: 48) { }

            public override void TrackSteps(int steps)
            {
                const int minSteps = 1;
                const int maxSteps = 100000;

                if (steps >= minSteps && steps <= maxSteps)
                {
                    TotalSteps += steps;
                    Console.WriteLine($"PRO: Tracked {steps} steps. Total: {TotalSteps}.");
                }
                else
                {
                    Console.WriteLine($"PRO: Invalid steps. Must be between {minSteps} and {maxSteps}.");
                }
            }
        }

        class ProgramDevice
        {
            static FitnessTracker SelectDevice()
            {
                while (true)
                {
                    Console.WriteLine("Select a fitness device:");
                    Console.WriteLine("1) FitTrack");
                    Console.WriteLine("2) FitTrack Pro");

                    string choice = Console.ReadLine()!;

                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("You selected FitTrack Basic.");
                            return new FitTrack("BasicTracker");
                        case "2":
                            Console.WriteLine("You selected FitTrack Pro.");
                            return new FitTrackPro("ProTracker");
                        default:
                            Console.WriteLine("Invalid selection. Try again.");
                            break;
                    }
                }
            }

            static void HandleActivity(FitnessTracker tracker)
            {
                Console.WriteLine(" Start Activitiy");
                Console.WriteLine("1)Run (10 kcal/min)");
                Console.WriteLine("2)Cycle (8 kcal/min)");

                string activityChoice = Console.ReadLine()!;
                Console.Write("Enter duration (minutes): ");

                if (!double.TryParse(Console.ReadLine(), out double duration) || duration <= 0)
                {
                    Console.WriteLine("Invalid duration.");
                    return;
                }

                Activity newActivity = activityChoice switch
                {
                    "1" => new Running(duration),
                    "2" => new Cycling(duration),
                };

                if (newActivity == null)
                {
                    Console.WriteLine("Invalid activity choice.");
                    return;
                }

                tracker.SetCurrentActivity(newActivity);
                newActivity.DisplayActivityInfo();
            }

            static void Main(string[] args)
            {
                FitnessTracker tracker = SelectDevice();
                bool running = true;

                while (running)
                {
                    Console.WriteLine($"Menu({tracker.GetType().Name})");
                    Console.WriteLine(new string('-', 36));
                    Console.WriteLine("1) Add Account");
                    Console.WriteLine("2) Select User Account");
                    Console.WriteLine("3) Delete Account");
                    Console.WriteLine("4) Track Steps");
                    Console.WriteLine("5) Show Device Info");
                    Console.WriteLine("6) Start Activity");
                    Console.WriteLine("7) Stop Activity");
                    Console.WriteLine("8) Change Device");
                    Console.WriteLine("9) Exit");


                    string menu = Console.ReadLine()!;

                    switch (menu)
                    {
                        case "1":
                            Console.WriteLine(new string('-', 20));
                            Console.Write("Enter account name: ");

                            tracker.AddAccount(Console.ReadLine()!);
                            break;

                        case "2":
                            tracker.PrintAllAccounts();
                            Console.WriteLine(new string('-', 20));
                            Console.Write("Enter account name to select: ");
                            tracker.SetCurrentAccount(Console.ReadLine()!);
                            break;

                        case "3":
                            Console.WriteLine(new string('-', 20));
                            Console.Write("Enter account name to delete: ");
                            tracker.DeleteAccount(Console.ReadLine()!);
                            break;

                        case "4":
                            Console.WriteLine(new string('-', 20));
                            Console.Write("Enter steps: ");
                            if (int.TryParse(Console.ReadLine(), out int steps))
                                tracker.TrackSteps(steps);
                            else
                                Console.WriteLine("Invalid number.");
                            break;

                        case "5":
                            tracker.PrintInfo();
                            break;

                        case "6":
                            if (tracker.CurrentActivity != null)

                                Console.WriteLine("Activity already running. Stop it first.");
                            else
                                HandleActivity(tracker);
                            break;

                        case "7":
                            tracker.StopActivity();
                            break;

                        case "8":
                            Console.WriteLine(new string('-', 20));
                            Console.WriteLine("Changing device...");
                            tracker = SelectDevice();
                            break;

                        case "9":
                            Console.WriteLine(new string('-', 20));
                            Console.WriteLine("Exiting...");
                            running = false;
                            break;

                        default:
                            Console.WriteLine(new string('-', 20));
                            Console.WriteLine("Invalid option");
                            break;
                    }
                }
            }
        }
    }

}