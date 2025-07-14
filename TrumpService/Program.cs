namespace TrumpService
{
    class Program
    {
        static void Main(string[] args)
        {
            MailClass mail = new MailClass();
            mail.AppointmentMail();
            mail.VisitorAcceptance();
            mail.OutWord();
            mail.PO();
            mail.SuppliearAdd();
        }
    }
}
