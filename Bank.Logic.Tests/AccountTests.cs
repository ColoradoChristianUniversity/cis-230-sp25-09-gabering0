using Bank.Logic;
using Bank.Logic.Abstractions;
using FluentAssertions;
using System;
using Xunit;

namespace Bank.Logic.Tests;

public class AccountTests
{
    private readonly IAccount account = new Account();

    [Fact]
    public void GetBalance_WithDepositsAndWithdrawals_ShouldReturnCorrectBalance()
    {
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 200, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 50, DateTime.UtcNow));

        double expectedBalance = 200 - 50;
        account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
    }

    [Fact]
    public void TryAddTransaction_WhenAddingInterest_ShouldReturnFalse()
    {
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Interest, 100, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_WhenAddingOverdraftFeeDirectly_ShouldReturnFalse()
    {
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Fee_Overdraft, -35, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_WhenWithdrawalExceedsBalance_ShouldReturnFalse()
    {
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_WhenOverdraftOccurs_ShouldApplyOverdraftFee()
    {
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
        account.GetTransactions().Should().Contain(t => t.Type == TransactionType.Fee_Overdraft && t.Amount == -35);
    }

    [Fact]
    public void TryAddTransaction_WhenDepositingNegativeAmount_ShouldReturnFalse()
    {
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, -100, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_WhenWithdrawingZeroAmount_ShouldReturnFalse()
    {
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 0, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void GetBalance_WithMultipleDepositsAndWithdrawals_ShouldReturnCorrectTotal()
    {
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 500, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 300, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 100, DateTime.UtcNow));

        double expectedBalance = 500 - 200 + 300 - 100;
        account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
    }

    public ITransaction CreateTransaction(TransactionType type, double amount, DateTime date)
    {
        return new Transaction(type, amount, date, skipSignValidation: true); // Add skipSignValidation
    }
}