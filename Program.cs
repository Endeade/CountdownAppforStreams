using System;
using System.IO;
using System.Timers;

namespace CountdownTimerApp
{
    class Program
    {
        static System.Timers.Timer timer;
        static TimeSpan countdownTime;
        static TimeUnit selectedUnit = TimeUnit.Seconds;
        static bool isTimerRunning = false;
        const string filePath = @"C:\countdown.txt";

        enum TimeUnit
        {
            Hours,
            Minutes,
            Seconds
        }

        static void Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Fancy title with a border and some animation
            PrintTitle();

            // Load countdown time or set default value
            countdownTime = LoadCountdownTime();

            // Setup the timer
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += UpdateCountdown;

            // Display the main menu
            DisplayMainMenu();

            // Main loop
            while (true)
            {
                HandleInput();
            }
        }

        static TimeSpan LoadCountdownTime()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var timeString = File.ReadAllText(filePath);
                    return TimeSpan.Parse(timeString);
                }
                catch
                {
                    Console.WriteLine("Error reading time from file. Starting with 0.");
                }
            }
            return new TimeSpan(0, 0, 0);
        }

        static void UpdateCountdown(object sender, ElapsedEventArgs e)
        {
            if (countdownTime > TimeSpan.Zero)
            {
                countdownTime = countdownTime.Subtract(TimeSpan.FromSeconds(1));
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
            }
            else
            {
                StopAndSaveCountdown();
            }

            // Clear the previous time and print the updated time on the same line
            Console.SetCursorPosition(0, 9);
            Console.WriteLine($"Remaining time: {countdownTime:hh\\:mm\\:ss}   ");
        }

        static void PrintTitle()
        {
            // Print a colorful title with some spacing
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("***************************************************");
            Console.WriteLine("*                                                 *");
            Console.WriteLine("*           Welcome to Countdown Timer!           *");
            Console.WriteLine("*                                                 *");
            Console.WriteLine("***************************************************");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nStarting the timer allows you to countdown time!");
            Console.WriteLine("---------------------------------------------------");
            System.Threading.Thread.Sleep(1000); // Simulate a little delay for effect
        }

        static void DisplayMainMenu()
        {
            // Clear the screen and display the main menu
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Select your option by pressing the corresponding key:");
            Console.WriteLine("[h] - Set hours   [m] - Set minutes   [s] - Set seconds");
            Console.WriteLine("[f] - Start Timer   [r] - Stop Timer");
            Console.WriteLine("[e] - Exit");
            Console.WriteLine($"Remaining time on launch: {countdownTime:hh\\:mm\\:ss}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nEnter your choice: ");
        }

        static void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            var key = Console.ReadKey(true).Key;
            Console.SetCursorPosition(0, 8); // Move cursor up to input area to prevent overlapping text

            switch (key)
            {
                case ConsoleKey.H:
                    selectedUnit = TimeUnit.Hours;
                    Console.WriteLine("Editing hours...");
                    break;
                case ConsoleKey.M:
                    selectedUnit = TimeUnit.Minutes;
                    Console.WriteLine("Editing minutes...");
                    break;
                case ConsoleKey.S:
                    selectedUnit = TimeUnit.Seconds;
                    Console.WriteLine("Editing seconds...");
                    break;
                case ConsoleKey.F:
                    SaveAndStartCountdown();
                    break;
                case ConsoleKey.R:
                    StopAndSaveCountdown();
                    break;
                case ConsoleKey.E:
                    Environment.Exit(0); // Exit the application
                    break;
                default:
                    AdjustTime(key); // Adjust time based on user input
                    break;
            }
        }

        static void AdjustTime(ConsoleKey key)
        {
            Console.WriteLine("Enter a value (positive or negative number) to adjust the time.");

            string input = Console.ReadLine();
            if (int.TryParse(input, out int value))
            {
                switch (selectedUnit)
                {
                    case TimeUnit.Hours:
                        countdownTime = countdownTime.Add(TimeSpan.FromHours(value));
                        break;
                    case TimeUnit.Minutes:
                        countdownTime = countdownTime.Add(TimeSpan.FromMinutes(value));
                        break;
                    case TimeUnit.Seconds:
                        countdownTime = countdownTime.Add(TimeSpan.FromSeconds(value));
                        break;
                }

                // Automatically save and update the countdown time
                try
                {
                    File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: The time format is invalid.");
                }

                Console.SetCursorPosition(0, 8); // Print the updated time on the same line
                Console.WriteLine($"Updated time: {countdownTime:hh\\:mm\\:ss}");
            }
            else
            {
                Console.WriteLine("Invalid input, please enter a valid number.");
            }

            DisplayMainMenu();  // Redisplay main menu after input
        }

        static void SaveAndStartCountdown()
        {
            // Prevent starting if the timer is already running
            if (isTimerRunning)
            {
                Console.SetCursorPosition(0, 10); // Prevent overlapping text
                Console.WriteLine("Countdown is already running.");
                return;
            }

            // Prevent starting if the countdown time is zero or negative
            if (countdownTime <= TimeSpan.Zero)
            {
                Console.SetCursorPosition(0, 10);
                Console.WriteLine("Cannot start. Set a positive countdown time first.");
                return;
            }

            // Start the timer
            timer.Start();
            isTimerRunning = true;

            // Save the current countdown time to the file
            try
            {
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: The time format is invalid.");
            }

            Console.SetCursorPosition(0, 10); // Prevent overlapping text
            Console.WriteLine("Countdown started.");
            DisplayMainMenu();
        }

        static void StopAndSaveCountdown()
        {
            // Prevent stopping if the timer isn't running
            if (!isTimerRunning)
            {
                Console.SetCursorPosition(0, 10); // Prevent overlapping text
                Console.WriteLine("Countdown is not running.");
                return;
            }

            // Stop the timer
            timer.Stop();
            isTimerRunning = false;

            // Save the current countdown time to the file
            try
            {
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: The time format is invalid.");
            }

            Console.SetCursorPosition(0, 10); // Prevent overlapping text
            Console.WriteLine($"Countdown stopped at: {countdownTime:hh\\:mm\\:ss}");
            DisplayMainMenu();
        }
    }
}
