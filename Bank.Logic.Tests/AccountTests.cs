using Bank.Logic;
using Bank.Logic.Abstractions;
using FluentAssertions;
using System;
using Xunit;

namespace Bank.Logic.Tests;

public class AccountTests
{
    [Fact]
    public void GetBalance_ShouldReturnCorrectBalance()
    {
        IAccount account = new Account();
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 200, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 50, DateTime.UtcNow));

        double expectedBalance = 200 - 50;
        account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
    }

    [Fact]
    public void TryAddTransaction_ShouldReturnFalse_WhenAddingInterestDirectly()
    {
        IAccount account = new Account();
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Interest, 100, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_ShouldReturnFalse_WhenAddingOverdraftFeeDirectly()
    {
        IAccount account = new Account();
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Fee_Overdraft, -35, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_ShouldReturnFalse_WhenOverdraftOccurs()
    {
        IAccount account = new Account();
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_ShouldApplyOverdraftFee_WhenOverdraftOccurs()
    {
        IAccount account = new Account();
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
        account.GetTransactions().Should().Contain(t => t.Type == TransactionType.Fee_Overdraft && t.Amount == -35);
    }

    [Fact]
    public void TryAddTransaction_ShouldReturnFalse_WhenDepositingNegativeAmount()
    {
        IAccount account = new Account();
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, -100, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void TryAddTransaction_ShouldReturnFalse_WhenWithdrawingZeroAmount()
    {
        IAccount account = new Account();
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 0, DateTime.UtcNow));
        result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
    }

    [Fact]
    public void GetBalance_ShouldHandleMultipleDepositsAndWithdrawals()
    {
        IAccount account = new Account();
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 500, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 300, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 100, DateTime.UtcNow));

        double expectedBalance = 500 - 200 + 300 - 100;
        account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
    }

    [Fact]
    public void TryAddTransaction_WhenAddingManagementFee_ShouldReturnTrue()
    {
        IAccount account = new Account();
        bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Fee_Management, -5, DateTime.UtcNow));
        result.Should().BeTrue(nameof(IAccount.TryAddTransaction));
        account.GetTransactions().Should().Contain(t => t.Type == TransactionType.Fee_Management && t.Amount == -5);
    }

    [Fact]
    public void GetBalance_WithManagementFeeDeduction_ShouldReturnCorrectTotal()
    {
        IAccount account = new Account();
        account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow));
        account.TryAddTransaction(CreateTransaction(TransactionType.Fee_Management, -5, DateTime.UtcNow));

        double expectedBalance = 100 - 5;
        account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
    }

    public ITransaction CreateTransaction(TransactionType type, double amount, DateTime date)
    {
        return new Transaction(type, amount, date, skipSignValidation: true);
    }
}