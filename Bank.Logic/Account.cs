using Bank.Logic.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bank.Logic;

public class Account : IAccount
{
    private readonly List<ITransaction> _transactions = new();
    private AccountSettings _settings = new();

    public AccountSettings Settings
    {
        get => _settings;
        set => _settings = value ?? new AccountSettings();
    }

    public double GetBalance()
    {
        return _transactions.Sum(t => t.Amount);
    }

    public IReadOnlyList<ITransaction> GetTransactions()
    {
        return _transactions.AsReadOnly();
    }

    public bool TryAddTransaction(ITransaction transaction)
    {
        if (transaction == null) return false;

        // Reject Unknown, Interest, and Fee_Overdraft (Fee_Management is allowed)
        if (transaction.Type == TransactionType.Unknown || 
            transaction.Type == TransactionType.Interest || 
            transaction.Type == TransactionType.Fee_Overdraft) // Re-added Fee_Overdraft
        {
            return false;
        }

        if (transaction.Type == TransactionType.Deposit && transaction.Amount <= 0)
            return false;
        if (transaction.Type == TransactionType.Withdraw && transaction.Amount <= 0)
            return false;

        double adjustedAmount = transaction.Amount;
        if (transaction.Type == TransactionType.Fee_Management)
        {
            adjustedAmount = -Settings.ManagementFee; // Use configurable value from AccountSettings
        }
        else if (Utilities.IndicatesNegativeAmount(transaction.Type) && adjustedAmount > 0)
        {
            adjustedAmount = -adjustedAmount;
        }
        else if (!Utilities.IndicatesNegativeAmount(transaction.Type) && adjustedAmount < 0)
        {
            adjustedAmount = -adjustedAmount;
        }

        if (Utilities.IndicatesNegativeAmount(transaction.Type) && adjustedAmount >= 0)
        {
            return false;
        }
        if (!Utilities.IndicatesNegativeAmount(transaction.Type) && adjustedAmount < 0)
        {
            return false;
        }

        ITransaction newTransaction = new Transaction(transaction.Type, adjustedAmount, transaction.Date, skipSignValidation: true);

        double currentBalance = GetBalance();
        double newBalance = currentBalance + newTransaction.Amount;
        bool isWithdrawal = newTransaction.Type == TransactionType.Withdraw;

        if (isWithdrawal && newBalance < 0)
        {
            var overdraftFee = new Transaction(TransactionType.Fee_Overdraft, -_settings.OverdraftFee, DateTime.Now, skipSignValidation: true);
            _transactions.Add(overdraftFee);
            return false;
        }

        _transactions.Add(newTransaction);
        return true;
    }
}