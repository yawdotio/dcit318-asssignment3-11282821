// Finance Management System
using System;

// a. Define a record type to represent financial data
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// b. Define an interface ITransactionProcessor
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// c. Create three concrete classes implementing ITransactionProcessor
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer: GHS {transaction.Amount} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing mobile money payment: GHS {transaction.Amount} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto payment: GHS {transaction.Amount} for {transaction.Category}");
    }
}

// d. Define a base class Account
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New balance: GHS {Balance}");
    }
}

// e. Define a sealed class SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Updated balance: GHS {Balance}");
        }
    }
}

// f. Create a class FinanceApp
public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // i. Instantiate a SavingsAccount
        SavingsAccount savingsAccount = new SavingsAccount("SA12345", 1000);
        Console.WriteLine($"Savings Account Created: {savingsAccount.AccountNumber} with initial balance ${savingsAccount.Balance}");

        // ii. Create three Transaction records
        Transaction transaction1 = new Transaction(1, DateTime.Now, 150, "Groceries");
        Transaction transaction2 = new Transaction(2, DateTime.Now, 200, "Utilities");
        Transaction transaction3 = new Transaction(3, DateTime.Now, 100, "Entertainment");

        // iii. Use processors for each transaction
        Console.WriteLine("\nProcessing transactions:");
        MobileMoneyProcessor mobileProcessor = new MobileMoneyProcessor();
        BankTransferProcessor bankProcessor = new BankTransferProcessor();
        CryptoWalletProcessor cryptoProcessor = new CryptoWalletProcessor();

        mobileProcessor.Process(transaction1);
        bankProcessor.Process(transaction2);
        cryptoProcessor.Process(transaction3);

        // iv. Apply each transaction to the SavingsAccount
        Console.WriteLine("\nApplying transactions to account:");
        savingsAccount.ApplyTransaction(transaction1);
        savingsAccount.ApplyTransaction(transaction2);
        savingsAccount.ApplyTransaction(transaction3);

        // v. Add all transactions to _transactions
        _transactions.Add(transaction1);
        _transactions.Add(transaction2);
        _transactions.Add(transaction3);

        Console.WriteLine($"\nTotal transactions recorded: {_transactions.Count}");
    }

    public static void Main()
    {
        FinanceApp app = new FinanceApp();
        app.Run();
    }
}
