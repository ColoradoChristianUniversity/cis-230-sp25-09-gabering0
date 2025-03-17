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

        // Allow ManagementFee transactions explicitly
        if (transaction.Type == TransactionType.Unknown || 
            transaction.Type == TransactionType.Interest || 
            transaction.Type == TransactionType.Fee_Overdraft)
        {
            return false;
        }

        if (transaction.Type == TransactionType.Deposit && transaction.Amount <= 0)
            return false;
        if (transaction.Type == TransactionType.Withdraw && transaction.Amount <= 0)
            return false;

        // Create the transaction with the original amount, bypassing sign validation initially
        ITransaction newTransaction = new Transaction(transaction.Type, transaction.Amount, transaction.Date, skipSignValidation: true);

        // Adjust the amount based on the transaction type
        double adjustedAmount = newTransaction.Amount;
        if (Utilities.IndicatesNegativeAmount(newTransaction.Type))
        {
            // For withdrawals, ManagementFee, etc., amount should be negative
            if (adjustedAmount > 0)
            {
                adjustedAmount = -adjustedAmount; // Ensure negative
            }
        }
        else
        {
            // For deposits, etc., amount should be positive
            if (adjustedAmount < 0)
            {
                adjustedAmount = -adjustedAmount; // Ensure positive
            }
        }

        // For ManagementFee, override with the configured value after initial validation
        if (newTransaction.Type == TransactionType.Fee_Management)
        {
            if (newTransaction.Amount >= 0) return false; // Reject if initial amount is not negative
            adjustedAmount = Settings.ManagementFee; // Should be -5.0
        }

        // Final validation: ensure the adjusted amount matches the expected sign
        if (Utilities.IndicatesNegativeAmount(newTransaction.Type) && adjustedAmount >= 0)
        {
            return false;
        }
        if (!Utilities.IndicatesNegativeAmount(newTransaction.Type) && adjustedAmount < 0)
        {
            return false;
        }

        // Update the transaction with the adjusted amount
        ITransaction finalTransaction = new Transaction(newTransaction.Type, adjustedAmount, newTransaction.Date, skipSignValidation: true);

        double currentBalance = GetBalance();
        double newBalance = currentBalance + finalTransaction.Amount;
        bool isWithdrawal = finalTransaction.Type == TransactionType.Withdraw;

        if (isWithdrawal && newBalance < 0)
        {
            var overdraftFee = new Transaction(TransactionType.Fee_Overdraft, -_settings.OverdraftFee, DateTime.Now, skipSignValidation: true);
            _transactions.Add(overdraftFee);
            return false;
        }

        _transactions.Add(finalTransaction);
        return true;
    }
}