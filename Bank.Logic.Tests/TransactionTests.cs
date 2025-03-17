using Bank.Logic.Abstractions;
using FluentAssertions;
using System;
using Xunit;

namespace Bank.Logic.Tests
{
    public class TransactionTests
    {
        public ITransaction CreateTransaction(TransactionType type, double amount, DateTime date)
        {
            return new Transaction(type, amount, date);
        }

        [Fact]
        public void Transaction_ShouldHaveCorrectType()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);
            transaction.Type.Should().Be(TransactionType.Deposit, nameof(ITransaction.Type));
        }

        [Fact]
        public void Transaction_ShouldHaveCorrectAmount()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);
            transaction.Amount.Should().Be(100, nameof(ITransaction.Amount));
        }

        [Fact]
        public void Transaction_ShouldHaveCorrectDate()
        {
            var date = DateTime.UtcNow;
            var transaction = CreateTransaction(TransactionType.Deposit, 100, date);
            transaction.Date.Should().BeCloseTo(date, TimeSpan.FromMilliseconds(10), nameof(ITransaction.Date));
        }

        [Fact]
        public void Transaction_ShouldValidateAmountSign_Deposit()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);
            transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Deposit}");
        }

        [Fact]
        public void Transaction_ShouldValidateAmountSign_Withdraw()
        {
            var transaction = CreateTransaction(TransactionType.Withdraw, -50, DateTime.UtcNow);
            transaction.Amount.Should().BeNegative($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Withdraw}");
        }

        [Fact]
        public void Transaction_ShouldValidateAmountSign_FeeOverdraft()
        {
            var transaction = CreateTransaction(TransactionType.Fee_Overdraft, -35, DateTime.UtcNow);
            transaction.Amount.Should().BeNegative($"{nameof(ITransaction.Amount)} should be negative for {TransactionType.Fee_Overdraft}");
        }

        [Fact]
        public void Transaction_ShouldValidateAmountSign_Interest()
        {
            var transaction = CreateTransaction(TransactionType.Interest, 10, DateTime.UtcNow);
            transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Interest}");
        }

        [Fact]
        public void Transaction_ShouldValidateAmountSign_Unknown()
        {
            var transaction = CreateTransaction(TransactionType.Unknown, 0, DateTime.UtcNow);
            transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {TransactionType.Unknown}");
        }

        [Fact]
        public void CreateTransaction_WithManagementFee_ShouldSetCorrectType()
        {
            var transaction = CreateTransaction(TransactionType.Fee_Management, -5, DateTime.UtcNow);
            transaction.Type.Should().Be(TransactionType.Fee_Management);
            transaction.Amount.Should().Be(-5);
        }

        [Fact]
        public void CreateTransaction_WithPositiveManagementFee_ShouldThrowException()
        {
            Action act = () => CreateTransaction(TransactionType.Fee_Management, 5, DateTime.UtcNow);
            act.Should().Throw<ArgumentOutOfRangeException>("ManagementFee amount must be negative");
        }
    }
}