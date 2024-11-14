using System;
using System.IO;
using System.Timers;

namespace CountdownTimerApp
{
    class Program
    {
        // Timer for countdown interval
        static System.Timers.Timer timer;

        // Countdown time to track remaining time
        static TimeSpan countdownTime;

        // Selected time unit (hours, minutes, or seconds)
        static TimeUnit selectedUnit = TimeUnit.Seconds;

        // Flag to check if timer is running
        static bool isTimerRunning = false;

        // Path for the countdown file to save/load the timer value
        const string filePath = @"C:\countdown.txt";

        // Enum for time units to adjust
        enum TimeUnit
        {
            Hours,
            Minutes,
            Seconds
        }

        static void Main(string[] args)
        {
            Console.Clear(); // Clears the console at launch

            // Load countdown time from file or set to zero if no file exists
            countdownTime = LoadCountdownTime();

            // Initialize timer with a 1-second interval
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += UpdateCountdown; // Event to update countdown every second

            // Instructions for the user
            Console.WriteLine("Countdown Timer");
            Console.WriteLine("Press 'h' to select hours, 'm' to select minutes, 's' to select seconds.");
            Console.WriteLine("Enter a positive or negative number to add or subtract time for the selected unit.");
            Console.WriteLine("Press 'e' to exit.");
            Console.WriteLine("Press 'f' to save and start the countdown.");
            Console.WriteLine("Press 'r' to stop and save the current state of the countdown.");

            // Display the initial countdown time loaded from file
            DisplayInitialTime();
            DisplayRemainingTime(); // Display current remaining time on screen

            // Main loop to handle user input
            while (true)
            {
                HandleInput();
            }
        }

        // Loads saved countdown time from the file
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
            return new TimeSpan(0, 0, 0); // Default to zero if file not found or parsing fails
        }

        // Updates countdown timer every second
        static void UpdateCountdown(object sender, ElapsedEventArgs e)
        {
            if (countdownTime > TimeSpan.Zero)
            {
                countdownTime = countdownTime.Subtract(TimeSpan.FromSeconds(1)); // Decrease by 1 second
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss")); // Save updated time to file
                DisplayRemainingTime(); // Update displayed remaining time
            }
            else
            {
                timer.Stop(); // Stop timer if countdown reaches zero
                isTimerRunning = false;
                Console.WriteLine("Countdown Complete"); // Display completion message
            }
        }

        // Displays the loaded time from file as initial time on the console
        static void DisplayInitialTime()
        {
            Console.SetCursorPosition(0, 8); // Set position for display
            Console.WriteLine($"Remaining Time on Launch: {countdownTime:hh\\:mm\\:ss}   ");
        }

        // Displays the current remaining countdown time on the console
        static void DisplayRemainingTime()
        {
            Console.SetCursorPosition(0, 9); // Set position for display
            Console.WriteLine($"Remaining Time: {countdownTime:hh\\:mm\\:ss}   ");
        }

        // Handles user input to adjust or control the countdown timer
        static void HandleInput()
        {
            Console.SetCursorPosition(0, 11); // Position for input prompt
            Console.Write("Enter your choice: ");
            var input = Console.ReadLine()?.Trim(); // Read user input

            ClearInputLines(); // Clear input line and additional lines after input

            // Handle input based on user's choice
            switch (input)
            {
                case "h":
                    selectedUnit = TimeUnit.Hours;
                    EditTime(); // Call method to edit hours
                    break;
                case "m":
                    selectedUnit = TimeUnit.Minutes;
                    EditTime(); // Call method to edit minutes
                    break;
                case "s":
                    selectedUnit = TimeUnit.Seconds;
                    EditTime(); // Call method to edit seconds
                    break;
                case "e":
                    Console.WriteLine("Exiting the application...");
                    Environment.Exit(0); // Exit the application
                    break;
                case "f":
                    SaveAndStartCountdown(); // Start countdown
                    break;
                case "r":
                    StopAndSaveCountdown(); // Stop countdown and save time
                    break;
                default:
                    Console.WriteLine("Invalid input. Please enter 'h', 'm', 's', 'e', 'f', or 'r'.");
                    break;
            }
        }

        // Clears input prompt lines from the console to prevent clutter
        static void ClearInputLines()
        {
            Console.SetCursorPosition(0, 11); // First line used for input prompt
            Console.Write(new string(' ', Console.WindowWidth)); // Clear line
            Console.SetCursorPosition(0, 12); // Additional line for "Enter time adjustment"
            Console.Write(new string(' ', Console.WindowWidth)); // Clear line
            Console.SetCursorPosition(0, 11); // Move cursor back to first input line
        }

        // Method to edit time based on selected unit and user input
        static void EditTime()
        {
            Console.SetCursorPosition(0, 12); // Position for "Enter time adjustment" prompt
            Console.Write("Enter time adjustment (positive or negative integer): ");

            // Parse integer input for time adjustment
            if (int.TryParse(Console.ReadLine(), out int input))
            {
                // Adjust countdown time based on selected unit and input value
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

                // Auto-save and update display after adjusting time
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss"));
                DisplayInitialTime(); // Update initial time display
                DisplayRemainingTime(); // Update remaining time display
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter an integer.");
            }
            ClearInputLines(); // Clear input lines after editing
        }

        // Starts the countdown timer and saves the current time to the file
        static void SaveAndStartCountdown()
        {
            if (isTimerRunning)
            {
                Console.WriteLine("Countdown is already running.");
                return;
            }

            File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss")); // Save initial time to file
            timer.Start(); // Start countdown timer
            isTimerRunning = true;
            Console.WriteLine("Countdown started."); // Notify user
        }

        // Stops the countdown timer and saves the current time to the file
        static void StopAndSaveCountdown()
        {
            if (isTimerRunning)
            {
                timer.Stop(); // Stop the timer
                isTimerRunning = false;
                File.WriteAllText(filePath, countdownTime.ToString(@"hh\:mm\:ss")); // Save current time
                Console.WriteLine($"Countdown stopped and saved at: {countdownTime:hh\\:mm\\:ss}");
            }
            else
            {
                Console.WriteLine("Countdown is not running."); // Notify if timer wasn't running
            }
        }
    }
}
