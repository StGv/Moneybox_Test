using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;
        public const decimal NotificationLimit = 500m;

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
            protected set
            {
                if (value < 0m)
                {
                    throw new InvalidOperationException("Insufficient funds to make transfer");
                }

                if (value < NotificationLimit)
                {
                    _notificationService.NotifyFundsLow(this.User.Email);
                }

                _balance = value;
            }
        }

        public decimal PaidIn
        {
            get { return _paidIn; }
            protected set
            {
                if (value > PayInLimit)
                {
                    throw new InvalidOperationException("Account pay in limit reached");
                }

                if (PayInLimit - value < NotificationLimit)
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
            PaidIn = PaidIn + amount;
            Balance = Balance + amount;
       
            return Balance;
        }

    }
}
