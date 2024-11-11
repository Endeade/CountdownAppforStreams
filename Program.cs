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
            countdownTime = LoadCountdownTime();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += UpdateCountdown;

            Console.WriteLine("Countdown Timer");
            Console.WriteLine("Press 'h' to select hours, 'm' to select minutes, 's' to select seconds.");
            Console.WriteLine("Enter a positive or negative number to add or subtract time for the selected unit.");
            Console.WriteLine("Press 'e' to exit.");
            Console.WriteLine("Press 'f' to save and start the countdown.");
            Console.WriteLine("Press 'r' to stop and save the current state of the countdown.");

            Console.WriteLine($"Remaining Time on Launch: {countdownTime:hh\\:mm\\:ss}");

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
                    Console.WriteLine("Failed to load saved time. Starting from 0:00:00.");
                }
            }
            return new TimeSpan(0, 0, 0);
        }

        static void UpdateCountdown(object sender, ElapsedEventArgs e)
        {
            if (countdownTime > TimeSpan.Zero)
            {
                countdownTime = countdownTime.Subtract(TimeSpan.FromSeconds(1));
            }
            else
            {
                timer.Stop();
                isTimerRunning = false;
                Console.WriteLine("Countdown Complete");
            }

            File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
            Console.WriteLine($"Remaining Time: {countdownTime:hh\\:mm\\:ss}");
        }

        static void HandleInput()
        {
            Console.WriteLine("\nEnter your choice:");
            var key = Console.ReadKey(true).KeyChar;

            switch (key)
            {
                case 'h':
                    selectedUnit = TimeUnit.Hours;
                    Console.WriteLine("Editing Hours");
                    ChangeTime();
                    break;
                case 'm':
                    selectedUnit = TimeUnit.Minutes;
                    Console.WriteLine("Editing Minutes");
                    ChangeTime();
                    break;
                case 's':
                    selectedUnit = TimeUnit.Seconds;
                    Console.WriteLine("Editing Seconds");
                    ChangeTime();
                    break;
                case 'e':
                    Console.WriteLine("Exiting the application...");
                    Environment.Exit(0);
                    break;
                case 'f':
                    SaveAndStartCountdown();
                    break;
                case 'r':
                    StopAndSaveCountdown();
                    break;
                default:
                    Console.WriteLine("Invalid input. Please enter 'h' for hours, 'm' for minutes, 's' for seconds, 'e' to exit, 'f' to start, or 'r' to stop.");
                    break;
            }
        }

        static void ChangeTime()
        {
            Console.Write("Enter time adjustment (positive or negative integer): ");
            if (int.TryParse(Console.ReadLine(), out int input))
            {
                switch (selectedUnit)
                {
                    case TimeUnit.Hours:
                        countdownTime = countdownTime.Add(TimeSpan.FromHours(input));
                        break;
                    case TimeUnit.Minutes:
                        countdownTime = countdownTime.Add(TimeSpan.FromMinutes(input));
                        break;
                    case TimeUnit.Seconds:
                        countdownTime = countdownTime.Add(TimeSpan.FromSeconds(input));
                        break;
                }
                Console.WriteLine($"New Time: {countdownTime:hh\\:mm\\:ss}");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter an integer.");
            }
        }

        static void SaveAndStartCountdown()
        {
            if (isTimerRunning)
            {
                Console.WriteLine("Countdown is already running.");
                return;
            }

            File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
            timer.Start();
            isTimerRunning = true;
            Console.WriteLine("Countdown started.");
        }

        static void StopAndSaveCountdown()
        {
            if (isTimerRunning)
            {
                timer.Stop();
                isTimerRunning = false;
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\\:ss"));
                Console.WriteLine($"Countdown stopped and saved at: {countdownTime:hh\\:mm\\:ss}");
            }
            else
            {
                Console.WriteLine("Countdown is not running.");
            }
        }
    }
}
