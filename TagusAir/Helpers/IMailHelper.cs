namespace TagusAir.Helpers
{
    public interface IMailHelper
    {
        Response SendEmail(string to, string subject, string body, byte[] attachment = null, string attachmentName = null);
    }
}
