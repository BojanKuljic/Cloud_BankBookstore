namespace Client.Models
{
    public class EnlistMoneyTransferModel
    {
        public long? UserSendId { get; set; }

        public long? UserReceiveId { get; set; }

        public double? Amount { get; set; }
    }
}
