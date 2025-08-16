using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGrading
{
    // Custom exception for invalid score format
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException() : base("Invalid score format. Score must be a valid integer.") { }
        public InvalidScoreFormatException(string message) : base(message) { }
        public InvalidScoreFormatException(string message, Exception inner) : base(message, inner) { }
    }

    // Custom exception for missing fields
    public class MissingFieldException : Exception
    {
        public MissingFieldException() : base("Missing required field in student record.") { }
        public MissingFieldException(string message) : base(message) { }
        public MissingFieldException(string message, Exception inner) : base(message, inner) { }
    }

    // Student class to hold student information
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100)
                return "A";
            else if (Score >= 70 && Score <= 79)
                return "B";
            else if (Score >= 60 && Score <= 69)
                return "C";
            else if (Score >= 50 && Score <= 59)
                return "D";
            else
                return "F";
        }
    }

    // StudentResultProcessor class to handle file operations
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    // Check if all required fields are present
                    if (parts.Length < 3)
                    {
                        throw new MissingFieldException($"Line {lineNumber}: Missing required fields. Expected format: ID,FullName,Score");
                    }

                    // Try to parse ID and Score
                    if (!int.TryParse(parts[0].Trim(), out int id))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format. ID must be a valid integer.");
                    }

                    if (!int.TryParse(parts[2].Trim(), out int score))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format. Score must be a valid integer.");
                    }

                    // Create and add student to the list
                    Student student = new Student
                    {
                        Id = id,
                        FullName = parts[1].Trim(),
                        Score = score
                    };

                    students.Add(student);
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("STUDENT GRADE REPORT");
                writer.WriteLine("====================");
                writer.WriteLine();

                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }

                writer.WriteLine();
                writer.WriteLine($"Total Students: {students.Count}");
                writer.WriteLine($"Report Generated: {DateTime.Now}");
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("School Grading System");
            Console.WriteLine("====================");

            try
            {
                // Use relative path from project root
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "students.txt");
                string outputFilePath = "grade_report.txt";

                // Allow custom file paths if provided
                if (args.Length >= 1)
                {
                    inputFilePath = args[0];
                }
                if (args.Length >= 2)
                {
                    outputFilePath = args[1];
                }

                Console.WriteLine($"Reading from: {inputFilePath}");
                Console.WriteLine($"Writing to: {outputFilePath}");

                StudentResultProcessor processor = new StudentResultProcessor();
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine($"Processing complete. {students.Count} student records processed.");
                Console.WriteLine($"Grade report written to {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: Input file not found. {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
