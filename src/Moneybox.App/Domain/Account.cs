using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;
        private readonly INotificationService _notificationService;

        private decimal _balance;
        private decimal _paidIn;

        public Account(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal Balance
        {
            get { return _balance; }
            set
            {
                if (value < 0m)
                {
                    throw new InvalidOperationException("Insufficient funds to make transfer");
                }

                if (value < 500m)
                {
                    _notificationService.NotifyFundsLow(this.User.Email);
                }

                _balance = value;
            }
        }

        public decimal PaidIn
        {
            get { return _paidIn; }
            set
            {
                if (value > PayInLimit)
                {
                    throw new InvalidOperationException("Account pay in limit reached");
                }

                if (PayInLimit - value < 500m)
                {
                    _notificationService.NotifyApproachingPayInLimit(this.User.Email);
                }

                _paidIn = value;
            }
        }

        public decimal Withdraw(decimal amount)
        {
            Balance = Balance - amount;
            Withdrawn = Withdrawn - amount;
            return Balance;
        }

        public decimal PayIn(decimal amount)
        {
            Balance = Balance + amount;
            PaidIn = PaidIn + amount;

            return Balance;
        }

    }
}
