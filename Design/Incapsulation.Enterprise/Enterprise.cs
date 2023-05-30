using System;
using System.Linq;

namespace Incapsulation.EnterpriseTask
{
    public class Enterprise
    {
        public Guid Guid { get; }
        public string Name { get; set; }
        
        private string _inn;
        public string Inn
        {
            get { return _inn; }
            set { _inn = CheckInn(value); }
        }

        public DateTime EstablishDate { get; set; }
        
        public TimeSpan ActiveTimeSpan
        {
            get { return DateTime.Now - EstablishDate; }
        }

        public Enterprise(Guid guid)
        {
            Guid = guid;
        }

        public double GetTotalTransactionsAmount()
        {
            DataBase.OpenConnection();
            return DataBase.Transactions()
                .Where(z => z.EnterpriseGuid == Guid)
                .Sum(x => x.Amount);
        }

        private static string CheckInn(string inn)
        {
            if (inn.Length != 10 || !inn.All(z => char.IsDigit(z)))
                throw new ArgumentException(nameof(inn));
            return inn;
        }
    }
}