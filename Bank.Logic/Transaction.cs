using Bank.Logic.Abstractions;

namespace Bank.Logic;

public class Transaction : ITransaction
{
    private TransactionType _type;
    private double _amount;
    private DateTime _date;

    public TransactionType Type
    {
        get => _type;
        set => _type = value;
    }

    public double Amount
    {
        get => _amount;
        set
        {
            if (_type == TransactionType.Fee_Management && value >= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "ManagementFee amount must be negative.");
            }
            _amount = value;
        }
    }

    public DateTime Date
    {
        get => _date;
        set => _date = value;
    }

    public Transaction()
    {
        _type = TransactionType.Unknown;
        _amount = 1.0; // Default to a positive value for Unknown
        _date = DateTime.Now;
    }

    public Transaction(TransactionType type, double amount, DateTime date, bool skipSignValidation = false)
    {
        _type = type;
        if (!skipSignValidation)
        {
            bool shouldBeNegative = Utilities.IndicatesNegativeAmount(type);
            if (shouldBeNegative && amount >= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be negative for this transaction type.");
            }
            if (!shouldBeNegative && amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive for this transaction type.");
            }
        }
        Amount = amount == 0 && type == TransactionType.Unknown ? 1.0 : amount;
        _date = date;
    }
}